using System.Runtime.InteropServices;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Statements;

public record MethodCallStatement(ManiaScriptGenerator Generator, IScriptValue.ScriptMethod Method, List<ManiaScriptStatement> Statements) : BodyStatement(Statements),
    IColonLessStatement
{
    // If true, this call has been put outside of parent (because of inlining)
    private bool OutsideOfParent { get; set; }

    private bool RecursiveMethodIsInlined(ManiaScriptStatement statement)
    {
        if (statement is not MethodCallStatement call)
            return false;
        if (call.Method.IsInlined)
        {
            return true;
        }

        foreach (var arg in call.Statements)
        {
            if (RecursiveMethodIsInlined(arg))
                return true;
        }

        return false;
    }

    private void GenerateInlined(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        var block = new BlockStatement(new List<ManiaScriptStatement>());

        DeclareStatement? returnStatement = null;
        if (Method.runtimeReturn != null)
        {
            builder.VerboseComment($"Inlined '{Method.Name}' method");

            Generator._varCount += Statements.Count;
            if (Method.runtimeReturn.Bottom() is not IScriptValue.Void)
            {
                returnStatement = new DeclareStatement(
                    $"ret_{Generator._varCount++}",
                    Method.runtimeReturn.GetManiaScriptTypeNameNonGeneric()
                );
                returnStatement.Generate(builder);
                builder.StringBuilder.Append(';');
                builder.AppendLine();
            }

            foreach (ref var child in CollectionsMarshal.AsSpan(Method.Body))
                RecursivelyUpdateStatement(
                    ref child,
                    s => s is ReturnStatement,
                    s =>
                    {
                        if (returnStatement != null)
                        {
                            var original = ((ReturnStatement) s).Value;
                            // remove the ; at the end
                            if (original is CollectionStatement collection)
                                original = collection with {IsList = true};

                            return new AssignStatement(
                                "=",
                                new RawStatement(returnStatement.Name),
                                original
                            );
                        }

                        return new EmptyStatement();
                    });
        }

        for (var i = 0; i < Statements.Count; i++)
        {
            var argStatement = Statements[i];
            if (Method.runtimeArgs[i].Bottom() is null or IScriptValue.IDisableValueRepresentation)
                continue;

            var declare = new DeclareStatement(
                $"arg_{Generator._varCount - (Statements.Count - i) - 1}",
                Method.runtimeArgs[i].GetManiaScriptTypeNameNonGeneric(),
                argStatement
            );
            block.Statements.Add(declare);

            foreach (ref var child in CollectionsMarshal.AsSpan(Method.Body))
                RecursivelyUpdateStatement(ref child, Method.runtimeArgs[i], new RawStatement(declare.Name));
        }

        var isEmpty = block.Statements.Count == 0 && (Method.Body.Count == 0 || Method.Body[0] is EmptyStatement);
        block.Statements.AddRange(Method.Body);
        if (!isEmpty)
        {
            block.Generate(builder);
        }
        else
        {
            builder.VerboseComment("Not generated since empty");
        }
    }

    private void GenerateMacro(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        builder.VerboseComment($"Macro '{Method.Name}' method");

        // Remove returns
        foreach (ref var child in CollectionsMarshal.AsSpan(Method.Body))
            RecursivelyUpdateStatement(
                ref child,
                s => s is ReturnStatement,
                s =>
                {
                    var val = ((ReturnStatement) s).Value;
                    return val;
                });

        var first = true;
        foreach (var statement in Method.Body)
        {
            if (first) first = false;
            else builder.AppendLine();
            statement.Generate(builder);
            if (!statement.IsColonLess()) builder.StringBuilder.Append(';');
        }
    }

    private void GenerateNormal(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        DisableColonLess = true;

        // Generate inlined method calls before us
        var inlinedCalls = 0;
        foreach (var arg in Statements)
        {
            // If the argument is an inlined method call, then we need to generate it before us
            if (RecursiveMethodIsInlined(arg))
            {
                if (arg is MethodCallStatement callStatement)
                {
                    builder.VerboseComment($"Method '{Method.Name}' has put outside '{callStatement.Method.Name}'");
                    callStatement.OutsideOfParent = true;
                }

                arg.Generate(builder);

                inlinedCalls++;
            }
        }

        if (inlinedCalls > 0) builder.AppendLine();

        DeclareStatement? returnStatement = null;
            
        // If we're outside of the parent, then we must create our own return value
        if (OutsideOfParent && Method.runtimeReturn is not IScriptValue.Void)
        {
            returnStatement = new DeclareStatement(
                $"ret_{Generator._varCount++}",
                Method.runtimeReturn.GetManiaScriptTypeNameNonGeneric()
            );
            returnStatement.Generate(builder);
            sb.Append(" = ");
        }
        sb.Append(Method.Name);
        sb.Append('(');

        var oldRoot = Generator.Root;
        var body = Generator.Root = new CollectionStatement(new List<ManiaScriptStatement>(), ',', true);

        var first = true;
        foreach (var arg in Statements)
        {
            if (first) first = false;
            else builder.StringBuilder.Append(", ");

            // If the argument is an inlined method call, then we need to generate it before us
            if (RecursiveMethodIsInlined(arg))
            {
                sb.Append("ret_");
                sb.Append(Generator._varCount - (returnStatement != null ? 1 : 0) - inlinedCalls);
            }
            else
            {
                arg.Generate(builder);
            }
        }

        Generator.Root.Generate(builder);
        Generator.Root = oldRoot;
            
        sb.Append(')');

        if (returnStatement != null)
            sb.Append(';');
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        if (Method.IsMacro)
        {
            GenerateMacro(builder);
            return;
        }

        if (Method.IsInlined)
        {
            GenerateInlined(builder);
            return;
        }

        GenerateNormal(builder);
    }

    public bool DisableColonLess { get; set; }
}
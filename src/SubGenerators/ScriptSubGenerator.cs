using System.Collections.Concurrent;
using System.Diagnostics.SymbolStore;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace HandyTwenty.ManialinkGenerator;

public class ScriptSubGenerator : SubGenerator
{
    private Thread mainThread;

    private ConcurrentDictionary<Thread, List<string>> _logMap = new();

    protected override void Log<T>(T obj, int indent = 0)
    {
        if (Thread.CurrentThread == mainThread)
            base.Log(obj, indent);
        else
        {
            var list = _logMap.GetOrAdd(Thread.CurrentThread, new List<string>());
            list.Add(string.Join(null, Enumerable.Repeat("\t", indent)) + obj);
        }
    }

    protected override void Generate()
    {
        mainThread = Thread.CurrentThread;
        
        // Parallelize the work to reduce the generator time
        var tasks = new List<Action>();
        foreach (var tree in Compilation.SyntaxTrees)
        {
            var semanticModel = Compilation.GetSemanticModel(tree);
            foreach (var declare in tree
                         .GetRoot()
                         .DescendantNodesAndSelf()
                         .OfType<MethodDeclarationSyntax>())
            {
                tasks.Add(() =>
                {
                    if (string.IsNullOrEmpty(tree.FilePath)
                        || tree.FilePath.Contains("Generated/"))
                        return;

                    var symbol = (IMethodSymbol) ModelExtensions.GetDeclaredSymbol(semanticModel, declare)!;
                    if (!symbol.GetAttributes()
                            .Any(data => data.AttributeClass!.Name.StartsWith("ManiaScriptMethod")))
                        return;

                    Log($"Found Method {symbol.Name}");
                    try
                    {
                        FoundMethod(tree, semanticModel, symbol, default);
                    }
                    catch (Exception ex)
                    {
                        Log($"Exception when operating on {tree.FilePath}\n{ex}");
                    }
                });

            }
        }
        
        Parallel.Invoke(tasks.ToArray());

        foreach (var list in _logMap)
        {
            Receiver.Log.AddRange(list.Value);
        }
    }

    public record struct Setup(bool forceInline)
    {
        
    }

    private void FoundMethod(SyntaxTree tree, SemanticModel semanticModel,
        IMethodSymbol symbol, Setup setup)
    {
        var bodySyntax = (symbol.DeclaringSyntaxReferences
            .First()
            .GetSyntax() as MethodDeclarationSyntax)!;

        void GetNodes<T>(SyntaxNode up, List<T> list, int indent = 0)
        {
            foreach (var node in up.ChildNodes())
            {
                Log(string.Join("", Enumerable.Repeat("  ", indent)) + node.GetType().Name, 1);
                if (node is T s)
                {
                    list.Add(s);
                    GetNodes(node, list, indent + 1);
                }
                else
                    GetNodes(node, list, indent + 1);
            }
        }

        var bodyNodes = bodySyntax.ChildNodes();
        var blockSyntaxNode = bodyNodes.Last();

        if (blockSyntaxNode.Kind() != SyntaxKind.Block)
            throw new InvalidOperationException("Expected a block but had " + blockSyntaxNode.Kind());

        var b = new CodeBuilder();
        b.Encapsulate(symbol);
        b.AppendLine($"private{(symbol.IsStatic ? " static" : "")} readonly object MSToken_{symbol.Name} = new();");
        b.AppendLine(
            $"{symbol.DeclaredAccessibility.ToString().ToLower()}{(symbol.IsStatic ? " static" : "")} Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_{symbol.Name} => gen =>");
        b.BeginBracket();

        if (!setup.forceInline)
        {
            b.AppendLine($"if (gen.GetLinkedValue(MSToken_{symbol.Name}) is not {{ }} m_{symbol.Name})");
            b.BeginScope();
            b.AppendLine($"m_{symbol.Name} = gen.LinkObject(MSToken_{symbol.Name}, gen.CreateMethod(args =>");
            b.BeginBracket();
        }

        var generator = new MethodGenerator(b, semanticModel, symbol, blockSyntaxNode, Log);
        generator.Generate();

        if (!setup.forceInline)
        {
            if (symbol.ReturnType.SpecialType == SpecialType.System_Void)
                b.AppendLine("return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);");

            b.EndScope();
            b.AppendLine("}, new Type[] {");
            b.BeginScope();
            b.AppendLine($"typeof({generator.SpecialCase_GetMsTypeNameFromSymbol(symbol.ReturnType)}),");
            foreach (var param in symbol.Parameters)
            {
                b.AppendLine($"typeof({generator.SpecialCase_GetMsTypeNameFromSymbol(param.Type)}),");
            }

            var allowGeneric = symbol.Parameters.Any(p => p.Type is ITypeParameterSymbol);

            b.EndScope();
            b.AppendLine($"}}, allowGeneric: {(allowGeneric ? "true" : "false")}));");
            b.EndScope();

            b.AppendLine($"return m_{symbol.Name};");
        }

        b.EndBracket();
        b.StringBuilder.Append(";");

        Log(b);

        var hint = $"Script.{System.IO.Path.GetFileNameWithoutExtension(tree.FilePath)}.{symbol.Name}";

        lock (this) {
            Context.AddSource(hint, b.ToString());
        }
    }
}
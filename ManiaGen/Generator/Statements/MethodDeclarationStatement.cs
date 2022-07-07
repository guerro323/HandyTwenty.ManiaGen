using System.Runtime.InteropServices;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Statements;

public sealed record MethodDeclarationStatement
    (string Name, (string Type, IScriptValue Original)[] Args, List<ManiaScriptStatement> Statements) : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        builder.AppendLine(Args[0].Type);
        builder.StringBuilder.Append(' ');
        builder.StringBuilder.Append(Name);
        builder.StringBuilder.Append('(');
        
        var i = 0;
        foreach (var (type, original) in Args[1..])
        {
            if (i != 0) builder.StringBuilder.Append(", ");

            builder.StringBuilder.Append(type);
            builder.StringBuilder.Append(" marg_");
            builder.StringBuilder.Append(i);

            foreach (ref var child in CollectionsMarshal.AsSpan(Statements))
                RecursivelyUpdateStatement(ref child, original, new RawStatement($"marg_{i}"));

            i += 1;
        }

        builder.StringBuilder.Append(')');
        new BlockStatement(Statements).Generate(builder);
    }
}
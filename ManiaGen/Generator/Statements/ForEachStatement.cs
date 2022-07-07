using System.Runtime.InteropServices;

namespace ManiaGen.Generator.Statements;

public sealed record ForEachStatement() : BlockStatement(new List<ManiaScriptStatement>())
{
    public ManiaScriptStatement Value => Statements[0];
    public ManiaScriptStatement Enumerable => Statements[1];
    public Span<ManiaScriptStatement> Body => CollectionsMarshal.AsSpan(Statements)[2..];

    public ForEachStatement(ManiaScriptStatement enumerable, ManiaScriptStatement value, List<ManiaScriptStatement> body) : this()
    {
        Statements.Add(enumerable);
        Statements.Add(value);
        Statements.AddRange(body);
    }
    
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;

        sb.Append("foreach (");
        Enumerable.Generate(builder);
        sb.Append(" in ");
        Value.Generate(builder);
        sb.Append(") ");
        builder.BeginBracket();
        foreach (var statement in Body)
        {
            builder.AppendLine();
            statement.Generate(builder);
            if (!statement.IsColonLess()) sb.Append(';');
        }
        
        builder.EndBracket();
    }
}
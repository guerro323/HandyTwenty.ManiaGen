namespace ManiaGen.Generator.Statements;

public sealed record WhileStatement(List<ManiaScriptStatement> Conditions, List<ManiaScriptStatement> Statements)
    : BlockStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;

        sb.Append("while (");
        foreach (var statement in Conditions)
        {
            statement.Generate(builder);
        }

        sb.Append(") ");
        builder.BeginBracket();
        foreach (var statement in Statements)
        {
            builder.AppendLine();
            statement.Generate(builder);
            if (!statement.IsColonLess()) sb.Append(';');
        }
        
        builder.EndBracket();
    }
}
namespace ManiaGen.Generator.Statements;

public sealed record IfStatement(List<ManiaScriptStatement> Conditions, List<ManiaScriptStatement> Statements)
    : BlockStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;

        sb.Append("if (");
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

public sealed record ElseIfStatement(List<ManiaScriptStatement> Conditions, List<ManiaScriptStatement> Statements)
    : BlockStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;

        sb.Append("else if (");
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

public sealed record ElseStatement(List<ManiaScriptStatement> Statements)
    : BlockStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;

        sb.Append("else ");
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
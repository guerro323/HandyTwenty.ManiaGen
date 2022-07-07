namespace ManiaGen.Generator.Statements;

public record LabelStatement(string Name, List<ManiaScriptStatement> Statements) : CollectionStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        if (Statements.Count == 0)
        {
            
            builder.VerboseComment($"Label '{Name}' has not been processed because it was empty.");
            return;
        }

        var sb = builder.StringBuilder;

        sb.Append("***");
        sb.Append(Name);
        sb.Append("***");
        builder.AppendLine();
        sb.Append("***");
        builder.BeginScope();
        builder.AppendLine();
        foreach (var statement in Statements)
        {
            statement.Generate(builder);
            if (!statement.IsColonLess()) sb.Append(';');
        }

        builder.EndScope();
        builder.AppendLine();
        sb.Append("***");
    }
}
namespace ManiaGen.Generator.Statements;

public record BlockStatement(List<ManiaScriptStatement> Statements) : CollectionStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        
        builder.BeginBracket();
        foreach (var statement in Statements)
        {
            if (statement is EmptyStatement)
                continue;

            var start = builder.StringBuilder.Length;
            builder.AppendLine();
            var lengthBeforeGen = builder.StringBuilder.Length;
            statement.Generate(builder);
            if (lengthBeforeGen == builder.StringBuilder.Length)
                builder.StringBuilder.Remove(start, lengthBeforeGen - start);

            if (!statement.IsColonLess()) sb.Append(';');
        }
        
        builder.EndBracket();
    }
}
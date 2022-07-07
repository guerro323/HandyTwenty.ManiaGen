namespace ManiaGen.Generator.Statements;

public record CollectionStatement(List<ManiaScriptStatement> Statements, char Separator = ';', bool IsList = false, bool ForceSingleLine = false) : BodyStatement(Statements), 
    IColonLessStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var first = true;
        foreach (var statement in Statements)
        {
            if (!ForceSingleLine) builder.AppendLine();
            if (first) first = false;
            else if (IsList) builder.StringBuilder.Append(Separator);
            
            statement.Generate(builder);
            
            if (!IsList && !statement.IsColonLess()) builder.StringBuilder.Append(Separator);
        }
    }

    public bool DisableColonLess { get; set; }
}
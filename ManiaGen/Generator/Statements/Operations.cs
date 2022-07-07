namespace ManiaGen.Generator.Statements;

public record OperationStatement() : BodyStatement(new List<ManiaScriptStatement>()), IColonLessStatement
{
    public string Sign;
    public ManiaScriptStatement Left => Statements[0];
    public ManiaScriptStatement Right => Statements[1];
    
    public OperationStatement(string sign, ManiaScriptStatement left, ManiaScriptStatement right) : this()
    {
        Sign = sign;
        Statements.Add(left);
        Statements.Add(right);
    }
    
    public override void Generate(ManiaStringBuilder builder)
    {
        Left.Generate(builder);
        if (!builder.Compact) builder.StringBuilder.Append(' ');
        builder.StringBuilder.Append(Sign);
        if (!builder.Compact) builder.StringBuilder.Append(' ');
        Right.Generate(builder);
    }

    public bool DisableColonLess { get; set; }
}

public record PrefixStatement() : BodyStatement(new List<ManiaScriptStatement>())
{
    public ManiaScriptStatement Statement => Statements[0];

    public string Sign;

    public PrefixStatement(string sign, ManiaScriptStatement statement) : this()
    {
        Sign = sign;
        Statements.Add(statement);
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        builder.StringBuilder.Append(Sign);
        Statement.Generate(builder);
    }
}
namespace ManiaGen.Generator.Statements;

public sealed record AssignStatement() : BodyStatement(new List<ManiaScriptStatement>())
{
    public string Sign;

    public ManiaScriptStatement Left => Statements[0];
    public ManiaScriptStatement Right => Statements[1];

    public AssignStatement(string sign, ManiaScriptStatement left, ManiaScriptStatement right) : this()
    {
        Sign = sign;
        Statements.Add(left);
        Statements.Add(right);
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        Left.Generate(builder);
        if (!builder.Compact) sb.Append(' ');
        sb.Append(Sign);
        if (!builder.Compact) sb.Append(' ');
        Right.Generate(builder);
    }
}
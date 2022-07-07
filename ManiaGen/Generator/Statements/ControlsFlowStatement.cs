namespace ManiaGen.Generator.Statements;

public sealed record ReturnStatement() : BodyStatement(new List<ManiaScriptStatement>())
{
    public ManiaScriptStatement Value => Statements[0];

    public ReturnStatement(ManiaScriptStatement value) : this()
    {
        Statements.Add(value);
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        sb.Append("return ");
        Value.Generate(builder);
    }
}

public sealed record BreakStatement : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        builder.StringBuilder.Append("break");
    }
}

public sealed record ContinueStatement : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        builder.StringBuilder.Append("continue");
    }
}
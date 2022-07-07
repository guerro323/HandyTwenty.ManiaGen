namespace ManiaGen.Generator.Statements;

public sealed record ArrayAccessStatement() : BodyStatement(new List<ManiaScriptStatement>())
{
    public ManiaScriptStatement Array => Statements[0];
    public ManiaScriptStatement Index => Statements[1];

    public ArrayAccessStatement(ManiaScriptStatement array, ManiaScriptStatement index) : this()
    {
        Statements.Add(array);
        Statements.Add(index);
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        Array.Generate(builder);
        builder.StringBuilder.Append('[');
        Index.Generate(builder);
        builder.StringBuilder.Append(']');
    }
}
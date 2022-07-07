namespace ManiaGen.Generator.Statements;

public sealed record RawStatement(string Text) : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        builder.StringBuilder.Append(Text);
    }
}
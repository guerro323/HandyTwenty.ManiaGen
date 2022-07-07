namespace ManiaGen.Generator.Statements;

public sealed record PropertyStatement(ManiaScriptStatement Root, string Path) : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        Root.Generate(builder);
        sb.Append('.');
        sb.Append(Path);
    }
}
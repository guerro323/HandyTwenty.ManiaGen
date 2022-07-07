namespace ManiaGen.Generator.Statements;

public record CallLabelStatement(string Name, bool LatestExtensionOnly) : ManiaScriptStatement,
    IColonLessStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        var keyword = LatestExtensionOnly ? "---" : "+++";
        sb.Append(keyword);
        sb.Append(Name);
        sb.Append(keyword);
    }

    public bool DisableColonLess { get; set; }
}
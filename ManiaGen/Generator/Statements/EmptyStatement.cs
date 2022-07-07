namespace ManiaGen.Generator.Statements;

public record EmptyStatement : ManiaScriptStatement, IColonLessStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
    }

    public bool DisableColonLess { get; set; }
}
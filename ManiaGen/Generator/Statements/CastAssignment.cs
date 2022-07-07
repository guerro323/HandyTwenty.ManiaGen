namespace ManiaGen.Generator.Statements;

public sealed record CastAssignment() : BodyStatement(new List<ManiaScriptStatement>())
{
    public ManiaScriptStatement Value => Statements[0];
    public string Type { get; }

    public CastAssignment(ManiaScriptStatement value, string type) : this()
    {
        Statements.Add(value);
        Type = type;
    }

    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        sb.Append('(');
        Value.Generate(builder);
        sb.Append(" as ");
        sb.Append(Type);
        sb.Append(')');
    }
}
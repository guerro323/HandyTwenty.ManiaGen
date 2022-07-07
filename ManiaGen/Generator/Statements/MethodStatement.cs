namespace ManiaGen.Generator.Statements;

public sealed record MethodStatement(string Name, List<ManiaScriptStatement> Args) : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        sb.Append(Name);
        sb.Append('(');
        
        var i = 0;
        foreach (var arg in Args)
        {
            if (i != 0) sb.Append(", ");
            
            arg.Generate(builder);
            i += 1;
        }

        sb.Append(')');
    }
}
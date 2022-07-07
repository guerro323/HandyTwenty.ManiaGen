using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Statements;

public sealed record DeclareStatement() : BodyStatement(new List<ManiaScriptStatement>())
{
    public string Name { get; }
    public string Type { get; }
    public ManiaScriptStatement? Value => Statements.Count == 0 ? null : Statements[0];
    

    public DeclareStatement(string name, string type, ManiaScriptStatement? value = null) : this()
    {
        Name = name;
        Type = type;

        if (value != null)
            Statements.Add(value);
    }
    
    public override void Generate(ManiaStringBuilder builder)
    {
        if (Value?.Creator is IScriptValue.IDisableAllRepresentation)
        {
            builder.VerboseComment($"Variable '{Name}' wasn't put into the runtime since the type '{Type}' and value cannot be represented");
            return;
        }
        
        var sb = builder.StringBuilder;
        sb.Append("declare ");
        if (Value == null)
        {
            sb.Append(Type);
            sb.Append(' ');
        }

        sb.Append(Name);

        /*if (Value?.Creator is IScriptValue.IDisableValueRepresentation)
        {
            builder.VerboseComment($"Variable value of '{Name}' wasn't put into the runtime since the original value can't be represented");
        }
        else */
        if (Value != null)
        {
            if (Value.Creator?.Bottom() is IScriptValue.IIsClass) sb.Append(" <=> ");
            else sb.Append(" = ");
            Value.Generate(builder);
        }
    }
}
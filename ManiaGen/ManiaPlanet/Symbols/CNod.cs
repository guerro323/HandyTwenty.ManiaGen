using ManiaGen.Generator.Statements;

namespace ManiaGen.ManiaPlanet.Symbols;

public abstract partial class CNod : IScriptValue, 
    IScriptValue.IDisableValueRepresentation,
    IScriptValue.IIsClass
{
    public static string TypeRepresentation => string.Empty;
    public static IScriptValue Default => throw new InvalidOperationException($"Cannot call 'default' on 'CNod'");
    public bool IsConstant { get; set; }

    ManiaScriptStatement IScriptValue.ToStatement()
    {
        throw new InvalidOperationException($"Cannot call 'ToStatement()' on '{GetType()}'");
    }

    bool IScriptValue.ConstantEquals(IScriptValue other)
    {
        return this == other.Bottom();
    }

    IScriptValue IScriptValue.Bottom()
    {
        return this;
    }

    public virtual string RuntimeTypeRepresentation => GetType().Name;
}
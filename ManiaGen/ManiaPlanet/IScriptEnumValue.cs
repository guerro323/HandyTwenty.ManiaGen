using ManiaGen.Generator.Statements;

namespace ManiaGen.ManiaPlanet;

public interface IScriptEnumValue<TSelf, T> : IScriptValue
    where TSelf : IScriptEnumValue<TSelf, T>
    where T : Enum
{
    static abstract string ConvertToManiaScriptEquivalent(T value);
    
    T Value { get; set; }

    bool IScriptValue.ConstantEquals(IScriptValue other)
    {
        return other is TSelf { } otherI && otherI.Value.Equals(Value);
    }

    ManiaScriptStatement IScriptValue.ToStatement()
    {
        return new RawStatement($"{TSelf.TypeRepresentation}::{TSelf.ConvertToManiaScriptEquivalent(Value)}")
        {
            Creator = this
        };
    }

    IScriptValue IScriptValue.Bottom()
    {
        return this;
    }
}
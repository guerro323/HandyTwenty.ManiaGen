using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Statements;

public sealed record DeferredScriptValueStatement(IScriptValue Value) : ManiaScriptStatement
{
    public override void Generate(ManiaStringBuilder builder)
    {
        Value.ToStatement().Generate(builder);
    }
}
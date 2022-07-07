
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class Button
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        var converted_OnClick = MS_OnClick(gen);base.Generate(gen, ___generatedObjects);
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PendingEventLabel),
            () => converted_OnClick,
        });
    }
}
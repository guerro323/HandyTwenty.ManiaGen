
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        if (!___generatedObjects.Contains(_button))
        {
            _button.Generate(gen, ___generatedObjects);
            ___generatedObjects.Add(_button);
        }
        if (gen.GetLinkedValue(this._button) is not { } g_this__button)
        	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
        if (gen.GetLinkedValue(_button.OnClickLabel) is not { } g__button_OnClickLabel)
        	g__button_OnClickLabel = gen.LinkObject(_button.OnClickLabel, gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.NetObject>(new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(_button.OnClickLabel), "_button_OnClickLabel"));
        var converted_OnClick = MS_OnClick(gen);
        var converted_OnUpdate = MS_OnUpdate(gen);
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => g__button_OnClickLabel,
            () => converted_OnClick,
        });
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(UpdateLabel),
            () => converted_OnUpdate,
        });
    }
}

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    private readonly object MSToken_OnClick = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnClick => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnClick) is not { } m_OnClick)
            m_OnClick = gen.LinkObject(MSToken_OnClick, gen.CreateMethod(args =>
            {
                if (gen.GetLinkedValue(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter")) is not { } g__counter)
                	g__counter = gen.LinkObject(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter"), gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.Integer>(_counter, "_counter"));
                if (gen.GetLinkedValue(this._button) is not { } g_this__button)
                	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
                if (gen.GetLinkedValue(_button.Label) is not { } g__button_Label)
                	g__button_Label = gen.LinkObject(_button.Label, gen.Global<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>(_button.Label, "_button_Label"));
                gen.AssignAdd(() => g__counter, () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 1);
                global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>>(g__button_Label), 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "Click me more!")
                );
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "clicked!")
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_OnClick;
    };
}

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    private readonly object MSToken_OnUpdate = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnUpdate => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnUpdate) is not { } m_OnUpdate)
            m_OnUpdate = gen.LinkObject(MSToken_OnUpdate, gen.CreateMethod(args =>
            {
                if (gen.GetLinkedValue(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter")) is not { } g__counter)
                	g__counter = gen.LinkObject(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter"), gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.Integer>(_counter, "_counter"));
                if (gen.GetLinkedValue(this._label) is not { } g_this__label)
                	g_this__label = gen.LinkObject(this._label, gen.Global<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>(this._label, "this__label"));
                if (gen.GetLinkedValue(this._button) is not { } g_this__button)
                	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
                gen.If(new[]
                {
                    () => gen.Greater(() => g__counter, () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 0)
                },
                () => 
                {
                    global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText.Call(gen, 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>>(g_this__label), 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(gen.InterpolatedText(new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                        {
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Text) "Clicked ",
                            () => g__counter,
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Text) " time(s)",
                        }))
                    );
                    gen.AssignAdd(() => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativeRotation.Get(gen, g_this__label), () => gen.Multiply(() => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 4, () => g__counter));
                    gen.Assign(() => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3.Get(gen, g_this__button), () => gen.NetMethod(typeof(global::System.Numerics.Vector2), ".ctor", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => (global::ManiaGen.ManiaPlanet.IScriptValue.Real) 0.4835f,
                        () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 0,
                    }));
                    gen.Assign(() => gen.NetMethod(typeof(global::System.Numerics.Vector2), "X", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3.Get(gen, g_this__button),
                    }), () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 4);
                });
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_OnUpdate;
    };
}
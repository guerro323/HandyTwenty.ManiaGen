
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class Button
{
    private readonly object MSToken_OnClick = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnClick => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnClick) is not { } m_OnClick)
            m_OnClick = gen.LinkObject(MSToken_OnClick, gen.CreateMethod(args =>
            {
                var arg_ev = args[0];
                gen.If(new[]
                {
                    () => gen.Equal(() => global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Api.Type.Get(gen, arg_ev), () => (global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Api.Types.EventType) CMlScriptEvent.EType.MouseClick)
                },
                () => 
                {
                    gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(OnClickLabel),
                    });
                });
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
                typeof(global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent),
            }, allowGeneric: false));
        return m_OnClick;
    };
}
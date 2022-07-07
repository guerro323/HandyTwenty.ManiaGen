
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
using static ManiaGen.ManiaPlanet.CManiaScript;
namespace ManiaGen;

partial class TestPrint
{
    private readonly object MSToken_Hi = new();
    public Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_Hi => gen =>
    {
        if (gen.GetLinkedValue(MSToken_Hi) is not { } m_Hi)
            m_Hi = gen.LinkObject(MSToken_Hi, gen.CreateMethod(args =>
            {
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "hi!")
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_Hi;
    };
}
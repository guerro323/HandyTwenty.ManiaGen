
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
using static ManiaGen.ManiaPlanet.CManiaScript;
namespace ManiaGen;

partial class TestPrint
{
    private readonly object MSToken_Sum = new();
    public Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_Sum => gen =>
    {
        if (gen.GetLinkedValue(MSToken_Sum) is not { } m_Sum)
            m_Sum = gen.LinkObject(MSToken_Sum, gen.CreateMethod(args =>
            {
                var arg_left = args[0];
                var arg_right = args[1];
                gen.AssignAdd(() => arg_left, () => arg_right);
                return gen.Return(() => gen.Add(() => arg_left, () => arg_right));
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue),
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue),
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue),
            }, allowGeneric: true));
        return m_Sum;
    };
}
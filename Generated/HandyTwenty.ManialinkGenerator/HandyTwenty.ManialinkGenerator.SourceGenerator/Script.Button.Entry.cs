
using System.Numerics;
using Test.HTManialink;
using Test.Libs;
using Test.Stuff;
using Test.Stuff.Flow;
namespace Test;

partial class Button
{
    private readonly object MSToken_Entry = new();
    public Func<global::Test.Stuff.ManiaScriptGenerator, global::Test.Stuff.IScriptValue> MS_Entry => gen =>
    {
        if (gen.GetLinkedValue(MSToken_Entry) is not { } m_Entry)
            m_Entry = gen.LinkObject(MSToken_Entry, gen.CreateMethod(args =>
            {
                var converted_OnClick = MS_OnClick(gen);base.Generate(gen);
                gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel<Test.Stuff.CMlScriptEvent>), "Subscribe", new Func<global::Test.Stuff.IScriptValue>[]
                {
                    () => new global::Test.Stuff.IScriptValue.NetObject(PendingEventLabel),
                    () => converted_OnClick,
                });
                return gen.Return(() => global::Test.Stuff.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::Test.Stuff.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_Entry;
    };
}
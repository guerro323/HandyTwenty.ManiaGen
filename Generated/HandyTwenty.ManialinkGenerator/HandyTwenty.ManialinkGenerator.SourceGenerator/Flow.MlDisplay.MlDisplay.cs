
namespace Test.HTManialink;

partial class MlDisplay
{
    public override void Generate(global::Test.Stuff.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        if (gen.GetLinkedValue(this.Nod) is not { } g_this_Nod)
        	g_this_Nod = gen.LinkObject(this.Nod, gen.Global<global::Test.HTManialink.CMlScriptExtended>(this.Nod, "this_Nod"));
        gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
        {
            () => new global::Test.Stuff.IScriptValue.NetObject(PreMainLabel),
        });
        gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
        {
            () => new global::Test.Stuff.IScriptValue.NetObject(MainLabel),
        });
        gen.While(new[]
        {
            () => (global::Test.Stuff.IScriptValue.Boolean) true
        },
        () => 
        {
            gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
            {
                () => new global::Test.Stuff.IScriptValue.NetObject(PreUpdateLabel),
            });
            gen.ForEach(() => global::Test.Stuff.CMlScript.Api.Properties.PendingEvents.Get(gen, g_this_Nod), 
            v_ev => 
            {
                gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel<Test.Stuff.CMlScriptEvent>), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
                {
                    () => new global::Test.Stuff.IScriptValue.NetObject(PendingEventLabel),
                    () => v_ev,
                });
            });
            gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
            {
                () => new global::Test.Stuff.IScriptValue.NetObject(UpdateLabel),
            });
            gen.NetMethod(typeof(global::Test.Stuff.Flow.EventLabel), "Invoke", new Func<global::Test.Stuff.IScriptValue>[]
            {
                () => new global::Test.Stuff.IScriptValue.NetObject(AnimateLabel),
            });
        });
    }
}
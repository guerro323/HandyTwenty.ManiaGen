
namespace ManiaGen.HTManialink;

partial class MlComposer
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        if (gen.GetLinkedValue(this.Nod) is not { } g_this_Nod)
        	g_this_Nod = gen.LinkObject(this.Nod, gen.Global<global::ManiaGen.HTManialink.CMlScriptExtended>(this.Nod, "this_Nod"));
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PreMainLabel),
        });
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(MainLabel),
        });
        gen.While(new[]
        {
            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Boolean) true
        },
        () => 
        {
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PreUpdateLabel),
            });
            gen.ForEach(() => global::ManiaGen.ManiaPlanet.Symbols.CMlScript.Api.Properties.PendingEvents.Get(gen, g_this_Nod), 
            v_ev => 
            {
                gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                {
                    () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PendingEventLabel),
                    () => v_ev,
                });
            });
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(UpdateLabel),
            });
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(AnimateLabel),
            });
        });
    }
}
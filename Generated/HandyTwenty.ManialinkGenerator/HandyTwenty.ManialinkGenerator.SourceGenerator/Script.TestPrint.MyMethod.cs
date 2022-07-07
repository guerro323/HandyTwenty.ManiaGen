
using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
using static ManiaGen.ManiaPlanet.CManiaScript;
namespace ManiaGen;

partial class TestPrint
{
    private readonly object MSToken_MyMethod = new();
    public Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_MyMethod => gen =>
    {
        if (gen.GetLinkedValue(MSToken_MyMethod) is not { } m_MyMethod)
            m_MyMethod = gen.LinkObject(MSToken_MyMethod, gen.CreateMethod(args =>
            {
                var converted_Hi = MS_Hi(gen);
                var v_oz = gen.Declare(() => (global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Types.AlignHorizontal) HorizontalAlignment.Center, "oz");
                gen.If(new[]
                {
                    () => gen.NotEqual(() => v_oz, () => (global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Types.AlignHorizontal) HorizontalAlignment.Center)
                },
                () => 
                {
                    global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "yes!")
                    );
                });
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(
                    gen.NetMethod(typeof(int), "ToString", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => 
                        gen.Method((global::ManiaGen.ManiaPlanet.IScriptValue.ScriptMethod) MS_Sum(gen), new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                        {
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 42,
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 42,
                        }),
                    }))
                );
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(
                    gen.NetMethod(typeof(float), "ToString", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => 
                        gen.Method((global::ManiaGen.ManiaPlanet.IScriptValue.ScriptMethod) MS_Sum(gen), new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                        {
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Real) 0.5f,
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Real) 0.1f,
                        }),
                    }))
                );
                gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                {
                    () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(MainLabel),
                    () => converted_Hi,
                });
                gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                {
                    () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(MainLabel),
                });
                var v_input = gen.Declare(() => (global::ManiaGen.ManiaPlanet.IScriptValue.Real) 0.5f, "input");
                gen.If(new[]
                {
                    () => gen.Greater(() => 
                    gen.Method((global::ManiaGen.ManiaPlanet.IScriptValue.ScriptMethod) MS_Sum(gen), new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => v_input,
                        () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 4,
                    }), () => v_input)
                },
                () => 
                {
                    global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "yes!")
                    );
                });
                var v_ok = gen.Declare(() => (global::ManiaGen.ManiaPlanet.IScriptValue.Boolean) false, "ok");
                gen.If(new[]
                {
                    () => gen.Equal(() => v_ok, () => (global::ManiaGen.ManiaPlanet.IScriptValue.Boolean) true)
                },
                () => 
                {
                    global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "ok")
                    );
                });
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_MyMethod;
    };
}
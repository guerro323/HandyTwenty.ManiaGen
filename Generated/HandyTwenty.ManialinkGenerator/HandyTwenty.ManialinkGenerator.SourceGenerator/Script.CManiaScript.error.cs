
using ManiaGen.Generator;
using ManiaGen.ManiaPlanet.Libs;
using static ManiaGen.ManiaPlanet.IScriptValue;
namespace ManiaGen.ManiaPlanet;

partial class CManiaScript
{
    private static readonly object MSToken_error = new();
    public static Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_error => gen =>
    {
        if (gen.GetLinkedValue(MSToken_error) is not { } m_error)
            m_error = gen.LinkObject(MSToken_error, gen.CreateMethod(args =>
            {
                var arg_arg = args[0];
                var v_txt = gen.Declare(() => 
                global::ManiaGen.ManiaPlanet.Libs.MsTextLib.ComposeApi.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "ERROR: %1"), 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(arg_arg)
                ), "txt");
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(v_txt)
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.Text),
            }, allowGeneric: false));
        return m_error;
    };
}
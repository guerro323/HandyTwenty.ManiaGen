using ManiaGen.Generator;
using ManiaGen.ManiaPlanet.Libs;
using static ManiaGen.ManiaPlanet.IScriptValue;

namespace ManiaGen.ManiaPlanet;

public static partial class CManiaScript
{
    public class Log : IApiFunc<Text, IScriptValue.Void>
    {
        public static IScriptValue.Void Call(ManiaScriptGenerator generator, Func<Variable<Text>> arg1)
        {
            return generator.Method("log", new Func<IScriptValue>[] {arg1});
        }
    }

    [ManiaScriptApi(typeof(Log))]
    public static void log(string arg)
    {
        Console.WriteLine("called here");
    }

    [ManiaScriptMethod]
    public static void warn(string arg)
    {
        var txt = MsTextLib.Compose("WARN: %1", arg);
        log(txt);
    }
    
    [ManiaScriptMethod]
    public static void error(string arg)
    {
        var txt = MsTextLib.Compose("ERROR: %1", arg);
        log(txt);
    }
    
    public class Translate : IApiFunc<Text, Text>
    {
        public static Text Call(ManiaScriptGenerator generator, Func<Variable<Text>> arg1)
        {
            return generator.Method("_", new Func<IScriptValue>[] {arg1}, (Text) generator.Compile(arg1).value);
        }
    }

    [ManiaScriptApi(typeof(Translate))]
    public static string _(string arg)
    {
        Console.WriteLine("called _");
        throw new NotImplementedException();
    }
}
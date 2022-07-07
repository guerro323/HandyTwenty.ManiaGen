using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Libs;

public class MsTextLib : IManiaScriptLib
{
    public class ComposeApi : IApiFunc<IScriptValue.Text, IScriptValue.Text, IScriptValue.Text>
    {
        public static IScriptValue.Text Call(ManiaScriptGenerator generator, Func<IScriptValue.Variable<IScriptValue.Text>> format, Func<IScriptValue.Variable<IScriptValue.Text>> arg0)
        {
            var lib = generator.RequireLib<MsTextLib>();
            return generator.Method($"{lib.Name}::Compose", new Func<IScriptValue>[]
            {
                format,
                arg0
            }, new IScriptValue.Text(string.Empty)
            {
                IsConstant = generator.Compile(format).value.IsConstant
                             && generator.Compile(arg0).value.IsConstant
            });
        }

        public static IScriptValue.Text Call(ManiaScriptGenerator generator, Func<IScriptValue.Variable<IScriptValue.Text>> format, Func<IScriptValue.Variable<IScriptValue.Text>> arg0, Func<IScriptValue.Variable<IScriptValue.Text>> arg1)
        {
            var lib = generator.RequireLib<MsTextLib>();
            return generator.Method($"{lib.Name}::Compose", new Func<IScriptValue>[]
            {
                format,
                arg0,
                arg1
            }, new IScriptValue.Text(string.Empty)
            {
                IsConstant = generator.Compile(format).value.IsConstant
                             && generator.Compile(arg0).value.IsConstant
                             && generator.Compile(arg1).value.IsConstant
            });
        }
    }

    [ManiaScriptApi(typeof(ComposeApi))]
    public static string Compose(string format, string arg0) => throw new NotImplementedException();

    [ManiaScriptApi(typeof(ComposeApi))]
    public static string Compose(string format, string arg0, string arg1) => throw new NotImplementedException();
    
    public static string Path => "TextLib";
}
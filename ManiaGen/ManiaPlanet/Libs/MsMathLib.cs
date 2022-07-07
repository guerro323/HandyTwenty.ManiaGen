using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Libs;

public class MsMathLib : IManiaScriptLib
{
    public class ToRealApi : IApiFunc<IScriptValue.Integer, IScriptValue.Real>
    {
        public static IScriptValue.Real Call(ManiaScriptGenerator generator, Func<IScriptValue.Variable<IScriptValue.Integer>> arg)
        {
            var compiledArg = generator.Compile(arg).value;
            
            var lib = generator.RequireLib<MsMathLib>();
            return generator.Method($"{lib.Name}::ToReal", new Func<IScriptValue>[]
            {
                arg
            }, new IScriptValue.Real(((IScriptValue.Integer) compiledArg.Bottom()).Value)
            {
                IsConstant = compiledArg.IsConstant
            });
        }
    }

    [ManiaScriptApi(typeof(ToRealApi))]
    public float ToReal(int i)
    {
        return i;
    }
    
    public static string Path => "MathLib";
}
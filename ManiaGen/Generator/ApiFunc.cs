using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator;

public interface IApiFunc
{
    
}

public interface IApiProperty<TThis, T> : IApiFunc, IDynamicApiFunc
    where TThis : IScriptValue 
    where T : IScriptValue
{
    static abstract IScriptValue.Variable<T> Get(ManiaScriptGenerator generator, IScriptValue variable);
}

public interface IApiFunc<out TRet> : IApiFunc
    where TRet : IScriptValue
{
    static abstract TRet Call(ManiaScriptGenerator generator);
}

public interface IApiFunc<T1, out TRet> : IApiFunc
    where T1 : IScriptValue
    where TRet : IScriptValue
{
    static abstract TRet Call(ManiaScriptGenerator generator, Func<IScriptValue.Variable<T1>> arg1);
}

public interface IApiFunc<T1, T2, out TRet> : IApiFunc
    where T1 : IScriptValue
    where T2 : IScriptValue
    where TRet : IScriptValue
{
    static abstract TRet Call(ManiaScriptGenerator generator, Func<IScriptValue.Variable<T1>> arg1, Func<IScriptValue.Variable<T2>> arg2);
}

public interface IDynamicApiFunc
{
}
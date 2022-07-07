using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Flow;

public abstract class EventLabelBase
{
    public virtual string? GetExternalName() => null;

    protected internal abstract Type[] GetArgumentTypes();
    protected internal readonly List<IScriptValue> Arguments = new();
    protected internal readonly List<Action<IScriptValue.ScriptLabel>> Subscribers = new();
    internal int Subnesting;

    private static int _worldId = 1;
    public int WorldId = _worldId++;
    public List<IScriptValue.ScriptLabel> CreatedLabels = new List<IScriptValue.ScriptLabel>();
}

public class EventLabel : EventLabelBase
{
    public void Invoke()
    {
        
    }
    
    public void Subscribe(Action action)
    {
        
    }

    protected internal override Type[] GetArgumentTypes()
    {
        return Array.Empty<Type>();
    }
}

public class EventLabel<T1> : EventLabelBase
{
    public void Invoke(T1 arg1)
    {

    }
    
    public void Subscribe(Action<T1> action)
    {
        
    }
    
    protected internal override Type[] GetArgumentTypes()
    {
        return new[]
        {
            typeof(T1)
        };
    }
}
namespace ManiaGen.Generator;

/// <summary>
/// Indicate that this method can generate maniascript
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class ManiaScriptMethod : Attribute
{
    
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum)]
public class ManiaScriptApiAttribute : Attribute
{
    public readonly Type Type;
    public readonly string Template;

    public ManiaScriptApiAttribute(Type type)
    {
        Type = type;
    }
    
    public ManiaScriptApiAttribute(string template)
    {
        Template = template;
    }
}
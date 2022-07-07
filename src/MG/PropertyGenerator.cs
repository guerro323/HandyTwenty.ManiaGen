using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HandyTwenty.ManialinkGenerator;

public class PropertyGenerator
{
    private MethodGenerator.LogMethod Log;
    private SemanticModel semanticModel;
    private ISymbol _parentSymbol;

    public PropertyGenerator(SemanticModel semanticModel, ISymbol parentSymbol, MethodGenerator.LogMethod log)
    {
        this.semanticModel = semanticModel;
        _parentSymbol = parentSymbol;
        Log = log;
    }

    public List<MemberPathInfo> Generate(SyntaxNode node)
    {
        void GetNodes<T>(SyntaxNode up, List<T> list, int indent = 0)
        {
            foreach (var child in up.ChildNodes())
            {
                if (child is T s)
                {
                    list.Add(s);
                    GetNodes(child, list, indent + 1);
                }
                else
                    GetNodes(child, list, indent + 1);
            }
        }
        
        var list = new List<MemberPathInfo>();
        var nodes = new List<SyntaxNode>();
        GetNodes(node, nodes);
        nodes.RemoveAll(n => n.IsKind(SyntaxKind.SimpleMemberAccessExpression));

        foreach (var n in nodes)
        {
            Log($"Node: {n}");
        }

        foreach (var n in nodes)
        {
            if (!GenerateRecursive(list, n))
                break;
        }

        return list;
    }

    private bool ContainsSymbol(ISymbol symbol, ITypeSymbol? containing = null)
    {
        containing ??= _parentSymbol as ITypeSymbol ?? _parentSymbol.ContainingType;
        
        if (containing == null)
            throw new InvalidOperationException($"containing type of '{_parentSymbol}' null?");
        
        var i = 0;
        while (i++ < 64)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.ContainingType, containing)) 
                return true;

            if (containing.BaseType is { } baseType)
            {
                containing = baseType;
                continue;
            }

            return false;
        }

        throw new InvalidOperationException("recursion?");
    }

    private bool GenerateRecursive(List<MemberPathInfo> list, SyntaxNode node)
    {
        if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            throw new InvalidOperationException($"Didn't expected a member access expression here");
        }
        
        var identifierSymbol = semanticModel.GetSymbolInfo(node).Symbol;
        Log($"Symbol Type: {identifierSymbol.GetType().FullName} '{identifierSymbol}'");
        switch (identifierSymbol)
        {
            case INamedTypeSymbol:
                list.Add(new MemberPathInfo
                {
                    Scope = MemberPathInfo.EScope.Class,

                    Name = identifierSymbol.Name,
                    Symbol = identifierSymbol
                });
                return false;
            case ILocalSymbol:
                list.Add(new MemberPathInfo
                {
                    Scope = MemberPathInfo.EScope.Field,
                    FieldScope = MemberPathInfo.EFieldScope.Local,

                    Name = identifierSymbol.Name,
                    Symbol = identifierSymbol
                });
                return true;
            case IPropertySymbol or IFieldSymbol or IParameterSymbol:
            {
                var type = identifierSymbol switch
                {
                    IFieldSymbol f => f.Type,
                    IPropertySymbol p => p.Type,
                    IParameterSymbol p => p.Type
                };
                
                var attribute = identifierSymbol.GetAttributes()
                    .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
                // ManiaScript API
                if (attribute != null)
                {
                    type = (INamedTypeSymbol) attribute.ConstructorArguments[0].Value!;
                    var name = type.GetTypeName();

                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Field,
                        Api = MemberPathInfo.EApi.ManiaPlanet,

                        Name = name,
                        Symbol = type
                    });
                    return true;
                }
                 
                attribute = identifierSymbol.GetAttributes()
                    .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptMethod"));
                if (attribute != null)
                {
                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Field,
                        Api = MemberPathInfo.EApi.User,

                        Name = identifierSymbol.Name,
                        Symbol = identifierSymbol,
                        FieldType = type
                    });
                    return true;
                }
                
                // If the type of the member isn't a IScriptValue then we consider it a NetObject,
                // and if that member isn't on the creator
                if (!ContainsSymbol(identifierSymbol)
                    && !identifierSymbol.ContainingType.AllInterfaces.Any(i => i.Name.StartsWith("IScriptValue"))
                    && !type.AllInterfaces.Any(i => i.Name.StartsWith("IScriptValue")))
                {
                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Field,
                        Api = MemberPathInfo.EApi.Net,

                        Name = identifierSymbol.Name,
                        Symbol = identifierSymbol,
                        FieldType = type
                    });
                    return true;
                }
                
                // Global or parameter variable
                else
                {
                    if (identifierSymbol is IParameterSymbol parameterSymbol)
                    {
                        list.Add(new MemberPathInfo
                        {
                            Scope = MemberPathInfo.EScope.Field,
                            FieldScope = MemberPathInfo.EFieldScope.Parameter,

                            Name = identifierSymbol.Name,
                            Symbol = parameterSymbol,
                            FieldType = type
                        });
                        return true;
                    }

                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Field,
                        FieldScope = MemberPathInfo.EFieldScope.Global,

                        Name = identifierSymbol.Name,
                        Symbol = identifierSymbol,
                        FieldType = type
                    });
                    return true;
                }
            }
            case IMethodSymbol:
            {
                var attribute = identifierSymbol.GetAttributes()
                    .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
                // ManiaScript API
                if (attribute != null)
                {
                    var type = (INamedTypeSymbol) attribute.ConstructorArguments[0].Value!;
                    var name = type.GetTypeName();

                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Method,
                        Api = MemberPathInfo.EApi.ManiaPlanet,

                        Name = name,
                        Symbol = type
                    });
                    return true;
                }
                 
                attribute = identifierSymbol.GetAttributes()
                    .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptMethod"));
                if (attribute != null)
                {
                    list.Add(new MemberPathInfo
                    {
                        Scope = MemberPathInfo.EScope.Method,
                        Api = MemberPathInfo.EApi.User,

                        Name = identifierSymbol.Name,
                        Symbol = identifierSymbol
                    });
                    return true;
                }
                
                list.Add(new MemberPathInfo
                {
                    Scope = MemberPathInfo.EScope.Method,
                    Api = MemberPathInfo.EApi.Net,

                    Name = identifierSymbol.Name,
                    Symbol = identifierSymbol
                });
                return true;
            }
            default:
                return false;
        }
        
        
    }
}

public struct MemberPathInfo
{
    public enum EApi
    {
        None,
        /// <summary>
        /// [ManiaScriptApi]
        /// </summary>
        ManiaPlanet,
        /// <summary>
        /// [ManiaScriptMethod]
        /// </summary>
        User,
        /// <summary>
        /// generator.NetMethod()
        /// </summary>
        Net
    }
    
    public enum EFieldScope
    {
        Local,
        Global,
        Parameter
    }

    public enum EScope
    {
        Field,
        Method,
        Class
    }

    public EScope Scope;
    public EFieldScope FieldScope;
    public EApi Api;

    private string? _finalName;
    public string FinalName
    {
        get => _finalName ?? Name;
        set => _finalName = value;
    }

    public string Name;
    public ISymbol Symbol;

    public ITypeSymbol? FieldType;
}
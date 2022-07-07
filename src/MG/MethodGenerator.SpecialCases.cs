using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HandyTwenty.ManialinkGenerator;

public partial class MethodGenerator
{
    ITypeSymbol SpecialCase_GetMsType(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.IdentifierName:
                var symbol = ModelExtensions.GetSymbolInfo(semanticModel, node).Symbol;
                return symbol switch
                {
                    ILocalSymbol local => local.Type,
                    IFieldSymbol field => field.Type,
                    IPropertySymbol property => property.Type,
                    INamedTypeSymbol typeSymbol => typeSymbol
                };
            default:
                throw new ArgumentOutOfRangeException("node.Kind()");
        }
    }

    public string SpecialCase_GetMsTypeNameFromSymbol(ITypeSymbol symbol)
    {
        string msify(string type) => $"global::ManiaGen.ManiaPlanet.IScriptValue.{type}";

        if (symbol is ITypeParameterSymbol)
        {
            // generic type
            return "global::ManiaGen.ManiaPlanet.IScriptValue";
        }

        if (symbol.GetTypeName() == "global::System.Numerics.Vector2")
            return msify("Vec2");
        if (symbol.GetTypeName() == "global::System.Numerics.Vector3")
            return msify("Vec3");
        
        if (!symbol.AllInterfaces.Any(i => i.Name != "IScriptValue"))
        {
            // a simple .NET Object (only used for method mapping)
            return msify("NetObject");
        }

        return symbol.SpecialType switch
        {
            SpecialType.System_Void => msify("Void"),
            SpecialType.System_Int32 => msify("Integer"),
            SpecialType.System_Single => msify("Real"),
            SpecialType.System_Boolean => msify("Boolean"),
            SpecialType.System_String => msify("Text"),
            SpecialType.System_Char => msify("Text"),
            _ => symbol.GetTypeName()
        };
    }

    bool IsMsType(ITypeSymbol type)
    {
        var name = SpecialCase_GetMsTypeNameFromSymbol(type);
        return !name.StartsWith("global::ManiaGen.ManiaPlanet.IScriptValue.NetObject");
    }

    string SpecialCase_GetMsTypeName(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.IdentifierName:
                var s = semanticModel.GetSymbolInfo(node).Symbol;
                return SpecialCase_GetMsTypeNameFromSymbol(s switch
                {
                    ILocalSymbol local => local.Type,
                    IFieldSymbol field => field.Type,
                    IPropertySymbol property => property.Type,
                    ITypeSymbol typeSymbol => typeSymbol,
                    _ => throw new InvalidOperationException(s?.GetType().FullName ?? "null value")
                });
            case SyntaxKind.IntKeyword:
                return "global::ManiaGen.ManiaPlanet.IScriptValue.Integer";
            case SyntaxKind.FloatKeyword:
                return "global::ManiaGen.ManiaPlanet.IScriptValue.Real";
            case SyntaxKind.BoolKeyword:
                return "global::ManiaGen.ManiaPlanet.IScriptValue.Boolean";
            default:
                throw new ArgumentOutOfRangeException(node.Kind().ToString());
        }
    }

    void SpecialCase_ConvertCondition(SyntaxNode node)
    {
        switch (node)
        {
            case IdentifierNameSyntax identifier:
                var msType = SpecialCase_GetMsType(identifier);
                var isBoolean = msType.Name == "Boolean" || msType.BaseType is {Name: "Boolean"};
                if (isBoolean)
                    b.StringBuilder.Append("gen.Equal(() => ");
                Convert(identifier);
                if (isBoolean)
                    b.StringBuilder.Append(", () => (global::ManiaGen.ManiaPlanet.IScriptValue.Boolean) true)");
                break;

            default:
                Convert(node);
                break;
        }
    }

    int SpecialCase_ConvertIdentifier(SyntaxNode node, int start, out bool isApiProperty, out bool dontContinue)
    {
        dontContinue = false;
        isApiProperty = false;

        var identifierSymbol = semanticModel.GetSymbolInfo(node).Symbol;
        Log($"Symbol Type: {identifierSymbol.GetType().FullName} '{identifierSymbol}'");
        if (identifierSymbol is INamedTypeSymbol typeSymbol)
        {
            //b.StringBuilder.Append()
            dontContinue = true;
        }
        else if (identifierSymbol is ILocalSymbol localSymbol)
        {
            b.StringBuilder.Append("v_");
            b.StringBuilder.Append(localSymbol.Name);
        }
        else if (identifierSymbol is IPropertySymbol or IFieldSymbol or IParameterSymbol)
        {
            var attribute = identifierSymbol.GetAttributes()
                .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
            if (attribute != null)
            {
                isApiProperty = true;

                var type = (INamedTypeSymbol) attribute.ConstructorArguments[0].Value!;
                var name = type.GetTypeName();

                var cpy = b.StringBuilder.ToString();
                b.StringBuilder.Clear();
                b.StringBuilder.Append(name);
                b.StringBuilder.Append(".Get(gen, ");
                b.StringBuilder.Append(cpy);
                b.StringBuilder.Append(')');
            }
            // Global or parameter variable
            else
            {
                var type = identifierSymbol switch
                {
                    IFieldSymbol f => f.Type,
                    IPropertySymbol p => p.Type,
                    IParameterSymbol p => p.Type
                };

                if (identifierSymbol is IParameterSymbol parameterSymbol)
                {
                    b.StringBuilder.Append($"arg_{identifierSymbol.Name}");

                    if (!globalSymbolSet.Contains(identifierSymbol.Name))
                    {
                        globalBuilder.AppendLine(
                            $"var arg_{identifierSymbol.Name} = args[{parameterSymbol.Ordinal}];"
                        );

                        globalSymbolSet.Add(identifierSymbol.Name);
                    }
                }
                else
                {
                    if (!IsMsType(type))
                    {
                        b.StringBuilder.Insert(start, $"new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(");
                        var diff = b.StringBuilder.Length;
                        b.StringBuilder.Append(identifierSymbol.Name);
                        b.StringBuilder.Append(')');

                        Log("diff=" + diff);

                        return diff;
                    }
                    else
                    {
                        // TODO: it was '_g + Name' before
                        b.StringBuilder.Append($"g_{identifierSymbol.Name}");

                        if (!globalSymbolSet.Contains(identifierSymbol.Name))
                        {
                            var key = identifierSymbol.Name;
                            // This is a value type, so we need to convert to something that is the same reference on multiple calls
                            if (type.IsValueType)
                            {
                                key = $"new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, \"{identifierSymbol.ContainingType.Name}.{identifierSymbol.Name}\")";
                            }

                            var val = identifierSymbol.Name;
                            var name = $"\"{identifierSymbol.Name}\"";
                            
                            // make sure to register the variable as a global before all
                            globalBuilder.AppendLine(
                                $"if (gen.GetLinkedValue({key}) is not {{ }} g_{identifierSymbol.Name})"
                            );
                            globalBuilder.AppendLine(
                                $"\tg_{identifierSymbol.Name} = gen.LinkObject({key}, gen.Global<{SpecialCase_GetMsTypeNameFromSymbol(type)}>({val}, {name}));"
                            );

                            globalSymbolSet.Add(identifierSymbol.Name);
                        }
                    }
                }
            }
        }
        else if (identifierSymbol is IMethodSymbol)
        {
            if (!globalSymbolSet.Contains(identifierSymbol.Name))
            {
                globalBuilder.AppendLine("var converted_");
                globalBuilder.StringBuilder.Append(identifierSymbol.Name);
                globalBuilder.StringBuilder.Append(" = MS_");
                globalBuilder.StringBuilder.Append(identifierSymbol.Name);
                globalBuilder.StringBuilder.Append("(gen);");

                globalSymbolSet.Add(identifierSymbol.Name);
            }

            b.StringBuilder.Append("converted_");
            b.StringBuilder.Append(identifierSymbol.Name);
        }

        return 0;
    }
}
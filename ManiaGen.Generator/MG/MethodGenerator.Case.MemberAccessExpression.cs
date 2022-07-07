using System.Text;
using HandyTwenty.ManialinkGenerator;
using Microsoft.CodeAnalysis;

namespace ManiaGen.Generator;

public partial class MethodGenerator
{
    private void GeneratePath(SyntaxNode node, Func<MemberPathInfo, bool>? stopFunc = null, bool isAssigning = false)
    {
        Log("first symbol: " + semanticModel.GetSymbolInfo(node).Symbol.GetType().FullName);
        if (semanticModel.GetSymbolInfo(node).Symbol is IFieldSymbol
            {
                Type: INamedTypeSymbol
                {
                    TypeKind: TypeKind.Enum
                } asTypeSymbol
            })
        {
            Log("Found type symbol: " + asTypeSymbol);
            var attribute = asTypeSymbol
                .GetAttributes()
                .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
            if (attribute != null)
            {
                Log("found the attribute!");
                b.StringBuilder.Append('(');
                b.StringBuilder.Append(
                    ((INamedTypeSymbol) attribute.ConstructorArguments[0].Value).GetTypeName());
                b.StringBuilder.Append(')');
                b.StringBuilder.Append(' ');
                b.StringBuilder.Append(node);
                return;
            }
        }

        var propertyGenerator = new PropertyGenerator(semanticModel, symbol, Log);
        var list = propertyGenerator.Generate(node);
        foreach (var l in list)
        {
            Log($"Scope={l.Scope}, FieldScope={l.FieldScope}, Name={l.Name}, FieldType={l.FieldType}");
        }

        var groups = new List<(bool useDirectVariable, bool isMsType, List<MemberPathInfo> span)>();
        var currentGroup = (useDirectVariable: true, isMsType: true, span: new List<MemberPathInfo>());
        foreach (var l in list)
        {
            var stop = stopFunc?.Invoke(l) == true;
            if (stop)
            {
                break;
            }
            
            currentGroup.span.Add(l);

            if (l.FieldType != null && !IsMsType(l.FieldType))
                currentGroup.isMsType = false;

            // Terminate group
            if (l.Api != MemberPathInfo.EApi.None)
            {
                currentGroup.useDirectVariable = false;
                
                groups.Insert(0, currentGroup);
                currentGroup = (true, true, new List<MemberPathInfo>());
            }
        }
        
        if (currentGroup.span.Count > 0)
            groups.Add(currentGroup);

        bool ExecuteGroup(ref int index)
        {
            var group = groups[index];
            
            Log($"DirectVar={group.useDirectVariable} IsMsType={group.isMsType} Length={group.span.Count}");
            var lastMember = group.span[^1];
            var isPropertyCall = false;
            if (lastMember.Api != MemberPathInfo.EApi.None)
            {
                isPropertyCall = true;

                switch (lastMember.Api)
                {
                    case MemberPathInfo.EApi.ManiaPlanet:
                        b.StringBuilder.Append(((ITypeSymbol) lastMember.Symbol).GetTypeName());
                        b.StringBuilder.Append(".Get(gen, ");
                        break;
                    case MemberPathInfo.EApi.User:
                        b.StringBuilder.Append("gen.Method(");
                        b.StringBuilder.Append("MS_");
                        b.StringBuilder.Append(lastMember.Symbol.Name);
                        b.StringBuilder.Append(isAssigning ? "_set" : "_get");
                        b.StringBuilder.Append("(gen)");
                        b.StringBuilder.Append(", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                        b.BeginBracket();
                        break;
                    case MemberPathInfo.EApi.Net:
                        b.StringBuilder.Append("gen.NetMethod(typeof(");
                        b.StringBuilder.Append(lastMember.Symbol.ContainingType.GetTypeName());
                        b.StringBuilder.Append("), \"");
                        b.StringBuilder.Append(lastMember.Name);
                        b.StringBuilder.Append("\", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                        b.BeginBracket();
                        break;
                }

                if (group.span.Count > 1)
                    lastMember = group.span[^2];
            }

            var first = true;
            var pathBuilder = new StringBuilder();
            var rawPathBuilder = new StringBuilder();
            var start = pathBuilder.Length;
            for (var i = 0; i < group.span.Count; i++)
            {
                var path = group.span[i];
                if (isPropertyCall && i == group.span.Count - 1)
                    break;

                var castToOriginalType = (requested: group.useDirectVariable && false, done: false);
                if (first && castToOriginalType.requested && path.FieldType != null)
                {
                    pathBuilder.Append("((");
                    pathBuilder.Append(path.FieldType.GetTypeName());
                    pathBuilder.Append(')');
                    
                    castToOriginalType.done = true;
                }
                
                if (path.FieldScope == MemberPathInfo.EFieldScope.Parameter)
                {
                    if (!globalSymbolSet.Contains(path.Name))
                    {
                        globalBuilder.AppendLine(
                            $"var arg_{path.Name} = args[{(path.Symbol as IParameterSymbol)!.Ordinal}];"
                        );

                        globalSymbolSet.Add(path.Name);
                    }

                    pathBuilder.Append("arg_");
                }
                else if (path.FieldScope == MemberPathInfo.EFieldScope.Local)
                {
                    pathBuilder.Append("v_");
                }

                if (lastMember.FieldScope == MemberPathInfo.EFieldScope.Global)
                {
                    var finalName = path.Name;

                    var key = path.Name;
                    var access = "this";
                    if (rawPathBuilder.Length > 0)
                        access = rawPathBuilder.ToString();

                    // This is a value type, so we need to convert to something that is the same reference on multiple calls
                    if (path.FieldType!.IsValueType)
                    {
                        key =
                            $"new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName({access}, \"{path.Symbol.ContainingType.Name}.{path.Name}\")";
                    }
                    else
                    {
                        key = $"{access}.{path.Name}";
                    }

                    var val = $"{access}.{path.Name}";
                    if (!IsMsType(path.FieldType))
                    {
                        val = $"new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject({val})";
                    }
                    
                    var rawName = $"{access.Replace('.', '_')}_{path.Name}";
                    var name = $"\"{rawName}\"";
                    finalName = $"g_{rawName}";

                    if (!globalSymbolSet.Contains(finalName))
                    {
                        // make sure to register the variable as a global before all
                        globalBuilder.AppendLine(
                            $"if (gen.GetLinkedValue({key}) is not {{ }} {finalName})"
                        );
                        globalBuilder.AppendLine(
                            $"\t{finalName} = gen.LinkObject({key}, gen.Global<{SpecialCase_GetMsTypeNameFromSymbol(path.FieldType)}>({val}, {name}));"
                        );

                        globalSymbolSet.Add(finalName);
                    }

                    pathBuilder.Clear();
                    pathBuilder.Append(finalName);
                    rawPathBuilder.Append(path.Name);
                    
                    first = true;
                    continue;
                }

                if (first)
                {
                    first = false;
                }
                else pathBuilder.Append('.');

                pathBuilder.Append(path.Name);
                rawPathBuilder.Append(path.Name);

                if (castToOriginalType.done)
                {
                    pathBuilder.Append(".Bottom())");
                }
            }

            b.StringBuilder.Append(pathBuilder);

            if (isPropertyCall)
            {
                //b.StringBuilder.Append($"/* {group.span[^1].Api} */");
                if (lastMember.Api is MemberPathInfo.EApi.User or MemberPathInfo.EApi.Net)
                {
                    groups.RemoveAt(0);

                    for (index = 0; index < groups.Count; index++)
                    {
                        b.AppendLine("() => ");
                        ExecuteGroup(ref index);
                        b.StringBuilder.Append(',');
                    }

                    b.EndBracket();
                }

                b.StringBuilder.Append(')');
                return false;
            }

            return true;
        }

        for (var i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            ExecuteGroup(ref i);
        }
    }
    
    /*private void Case_MemberAccessExpressionSyntax(SyntaxNode node)
    {
        Log("first symbol: " + semanticModel.GetSymbolInfo(node).Symbol.GetType().FullName);
        if (semanticModel.GetSymbolInfo(node).Symbol is IFieldSymbol
            {
                Type: INamedTypeSymbol
                {
                    TypeKind: TypeKind.Enum
                } asTypeSymbol
            })
        {
            Log("Found type symbol: " + asTypeSymbol);
            var attribute = asTypeSymbol
                .GetAttributes()
                .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
            if (attribute != null)
            {
                Log("found the attribute!");
                b.StringBuilder.Append('(');
                b.StringBuilder.Append(
                    ((INamedTypeSymbol) attribute.ConstructorArguments[0].Value).GetTypeName());
                b.StringBuilder.Append(')');
                b.StringBuilder.Append(' ');
                b.StringBuilder.Append(node);
                return;
            }
        }

        var identifiers = node.ChildNodes();
        var first = true;

        var oldB = b;
        b = new CodeBuilder(b);

        var isApiProperty = false;

        var startAll = b.StringBuilder.Length;

        foreach (var identifier in identifiers)
        {
            var wasFirst = first;
            if (first) first = false;

            var start = b.StringBuilder.Length;

            var wasApiProperty = isApiProperty;
            var diff = SpecialCase_ConvertIdentifier(identifier, startAll, out isApiProperty, out var dontContinue);
            if (dontContinue)
                break;

            Console.WriteLine($"insert at {start} + {diff} = {start + diff}");
            if (!isApiProperty && !wasFirst)
                b.StringBuilder.Insert(diff > 0 ? diff : start, '.');

            if (wasApiProperty && !isApiProperty)
            {
                throw new InvalidOperationException("Can't go onto a normal property from an API property");
            }
        }

        // calling b.ToString() will put braces which is not intended here
        oldB.StringBuilder.Append(b.StringBuilder.ToString());
        b = oldB;
        return;
    }*/
}
#if FALSE
using System.Collections.Concurrent;
using System.Diagnostics.SymbolStore;
using System.Linq.Expressions;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace HandyTwenty.ManialinkGenerator;

public class ScriptSubGenerator : SubGenerator
{
    private Thread mainThread;

    private ConcurrentDictionary<Thread, List<string>> _logMap = new();

    protected override void Log<T>(T obj, int indent = 0)
    {
        if (Thread.CurrentThread == mainThread)
            base.Log(obj, indent);
        else
        {
            var list = _logMap.GetOrAdd(Thread.CurrentThread, new List<string>());
            list.Add(string.Join(null, Enumerable.Repeat("\t", indent)) + obj);
        }
    }

    protected override void Generate()
    {
        mainThread = Thread.CurrentThread;
        
        // Parallelize the work to reduce the generator time
        var tasks = new List<Action>();
        foreach (var tree in Compilation.SyntaxTrees)
        {
            var semanticModel = Compilation.GetSemanticModel(tree);
            foreach (var declare in tree
                         .GetRoot()
                         .DescendantNodesAndSelf()
                         .OfType<MethodDeclarationSyntax>())
            {
                tasks.Add(() =>
                {
                    if (string.IsNullOrEmpty(tree.FilePath)
                        || tree.FilePath.Contains("Generated/"))
                        return;

                    var symbol = (IMethodSymbol) ModelExtensions.GetDeclaredSymbol(semanticModel, declare)!;
                    if (!symbol.GetAttributes()
                            .Any(data => data.AttributeClass!.Name.StartsWith("ManiaScriptMethod")))
                        return;

                    Log($"Found Method {symbol.Name}");
                    try
                    {
                        FoundMethod(tree, semanticModel, symbol, default);
                    }
                    catch (Exception ex)
                    {
                        Log($"Exception when operating on {tree.FilePath}\n{ex}");
                    }
                });

            }
        }
        
        Parallel.Invoke(tasks.ToArray());

        foreach (var list in _logMap)
        {
            Receiver.Log.AddRange(list.Value);
        }
    }

    public record struct Setup(bool forceInline)
    {
        
    }

    private void FoundMethod(SyntaxTree tree, SemanticModel semanticModel,
        IMethodSymbol symbol, Setup setup)
    {
        var bodySyntax = (symbol.DeclaringSyntaxReferences
            .First()
            .GetSyntax() as MethodDeclarationSyntax)!;

        void GetNodes<T>(SyntaxNode up, List<T> list, int indent = 0)
        {
            foreach (var node in up.ChildNodes())
            {
                Log(string.Join("", Enumerable.Repeat("  ", indent)) + node.GetType().Name, 1);
                if (node is T s)
                {
                    list.Add(s);
                    GetNodes(node, list, indent + 1);
                }
                else
                    GetNodes(node, list, indent + 1);
            }
        }

        var blockSyntaxNodes = new List<BlockSyntax>();
        GetNodes(bodySyntax, blockSyntaxNodes);
        if (blockSyntaxNodes.Count == 0)
        {
            Log("Couldn't get the block syntax", 1);
            return;
        }
        
        var b = new CodeBuilder();
        b.Encapsulate(symbol);
        b.AppendLine($"private{(symbol.IsStatic ? " static" : "")} readonly object MSToken_{symbol.Name} = new();");
        b.AppendLine($"{symbol.DeclaredAccessibility.ToString().ToLower()}{(symbol.IsStatic ? " static" : "")} Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_{symbol.Name} => gen =>");
        b.BeginBracket();

        if (!setup.forceInline)
        {
            b.AppendLine($"if (gen.GetLinkedValue(MSToken_{symbol.Name}) is not {{ }} m_{symbol.Name})");
            b.BeginScope();
            b.AppendLine($"m_{symbol.Name} = gen.LinkObject(MSToken_{symbol.Name}, gen.CreateMethod(args =>");
            b.BeginBracket();
        }

        var generator = new MethodGenerator(b, semanticModel, symbol, blockSyntaxNodes[0], Log);
        generator.Generate();

        /*var globalStart = b.StringBuilder.Length;
        var globalBuilder = new CodeBuilder(b);
        
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

        string SpecialCase_GetMsTypeNameFromSymbol(ITypeSymbol symbol)
        {
            string msify(string type) => $"global::ManiaGen.ManiaPlanet.IScriptValue.{type}";
            
            if (symbol is ITypeParameterSymbol)
            {
                // generic type
                return "global::ManiaGen.ManiaPlanet.IScriptValue";
            }

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
                _ => symbol.GetTypeName()
            };
        }

        string SpecialCase_GetMsTypeName(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IdentifierName:
                    var symbol = semanticModel.GetSymbolInfo(node).Symbol;
                    return symbol switch
                    {
                        ILocalSymbol local => local.Type.GetTypeName(),
                        IFieldSymbol field => field.Type.GetTypeName(),
                        IPropertySymbol property => property.Type.GetTypeName(),
                        ITypeSymbol typeSymbol => typeSymbol.GetTypeName(),
                        _ => throw new InvalidOperationException(symbol?.GetType().FullName ?? "null value")
                    };
                case SyntaxKind.IntKeyword:
                    return "global::ManiaGen.ManiaPlanet.IScriptValue.Integer";
                case SyntaxKind.FloatKeyword:
                    return "global::ManiaGen.ManiaPlanet.IScriptValue.Real";
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
        
        var globalSymbolSet = new HashSet<string>();

        void SpecialCase_ConvertIdentifier(SyntaxNode node, out bool isApiProperty, out bool dontContinue)
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
                        if (!type.AllInterfaces.Any(i => i.Name == "IScriptValue"))
                        {
                            b.StringBuilder.Append("new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(");
                            b.StringBuilder.Append(identifierSymbol.Name);
                            b.StringBuilder.Append(')');
                        }
                        else
                        {
                            // TODO: it was '_g + Name' before
                            b.StringBuilder.Append($"g_{identifierSymbol.Name}");

                            if (!globalSymbolSet.Contains(identifierSymbol.Name))
                            {
                                // make sure to register the variable as a global before all
                                globalBuilder.AppendLine(
                                    $"if (gen.GetLinkedValue({identifierSymbol.Name}) is not {{ }} g_{identifierSymbol.Name})"
                                );
                                globalBuilder.AppendLine(
                                    $"\tg_{identifierSymbol.Name} = gen.LinkObject({identifierSymbol.Name}, gen.Global<{SpecialCase_GetMsTypeNameFromSymbol(type)}>());"
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
        }
        
        void Convert(SyntaxNode node)
        {
            switch (node)
            {
                case BlockSyntax:
                case ExpressionStatementSyntax:
                {
                    foreach (var child in node.ChildNodes())
                    {
                        Convert(child);
                        if (node is not BlockSyntax) 
                            b.StringBuilder.Append(';');
                    }

                    break;
                }
                
                // (a)
                case ParenthesizedExpressionSyntax:
                {
                    Convert(node.ChildNodes().First());
                    break;
                }

                // a X= b 
                // X -> = + - * ...
                case AssignmentExpressionSyntax:
                {
                    var children = node.ChildNodes();
                    var left = children.ElementAt(0);
                    var right = children.ElementAt(1);

                    switch (node.Kind())
                    {
                        case SyntaxKind.SimpleAssignmentExpression:
                        case SyntaxKind.AddAssignmentExpression:
                        case SyntaxKind.MultiplyAssignmentExpression:
                        case SyntaxKind.DivideAssignmentExpression:
                        {
                            b.AppendLine("gen.Assign");
                            b.StringBuilder.Append(node.Kind() switch
                            {
                                SyntaxKind.SimpleAssignmentExpression => "",
                                SyntaxKind.AddAssignmentExpression => "Add",
                                SyntaxKind.MultiplyAssignmentExpression => "Multiply",
                                SyntaxKind.DivideAssignmentExpression => "Divide",
                            });
                            b.StringBuilder.Append("(() => ");
                            Convert(left);
                            b.StringBuilder.Append(", () => ");
                            Convert(right);
                            b.StringBuilder.Append(')');
                            break;
                        }
                    }

                    break;
                }
                
                // Xa
                // X -> ! - + ++ -- ...
                case PrefixUnaryExpressionSyntax:
                {
                    var children = node.ChildNodes();
                    switch (node.Kind())
                    {
                        case SyntaxKind.LogicalNotExpression:
                            b.StringBuilder.Append("gen.Negate(");
                            Convert(children.ElementAt(0));
                            b.StringBuilder.Append(')');
                            break;
                    }

                    break;
                }

                // a X b
                // X -> == || > ...
                case BinaryExpressionSyntax:
                {
                    var children = node.ChildNodes();
                    switch (node.Kind())
                    {
                        case SyntaxKind.GreaterThanExpression:
                        case SyntaxKind.LogicalOrExpression:
                        case SyntaxKind.LogicalAndExpression:
                        case SyntaxKind.AddExpression:
                            var left = children.ElementAt(0);
                            var right = children.ElementAt(1);

                            b.StringBuilder.Append("gen.");
                            b.StringBuilder.Append(node.Kind() switch
                            {
                                SyntaxKind.AddExpression => "Add",
                                SyntaxKind.LogicalOrExpression => "Or",
                                SyntaxKind.LogicalAndExpression => "And",
                                SyntaxKind.GreaterThanExpression => "Greater",
                                SyntaxKind.GreaterThanOrEqualExpression => "GreaterOrEqual",
                                SyntaxKind.LessThanExpression => "Less",
                                SyntaxKind.LessThanOrEqualExpression => "LessOrEqual"
                            });

                            b.StringBuilder.Append("(() => ");
                            //SpecialCase_ConvertCondition(left);
                            Convert(left);
                            b.StringBuilder.Append(", () => ");
                            //SpecialCase_ConvertCondition(right);
                            Convert(right);
                            b.StringBuilder.Append(')');

                            break;
                    }

                    break;
                }

                case MemberAccessExpressionSyntax:
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
                            b.StringBuilder.Append(((INamedTypeSymbol) attribute.ConstructorArguments[0].Value).GetTypeName());
                            b.StringBuilder.Append(')');
                            b.StringBuilder.Append(' ');
                            b.StringBuilder.Append(node);
                            break;
                        }
                    }
                    
                    var identifiers = node.ChildNodes();
                    var first = true;

                    var oldB = b;
                    b = new CodeBuilder(b);

                    var isApiProperty = false;

                    foreach (var identifier in identifiers)
                    {
                        var wasFirst = first;
                        if (first) first = false;

                        var start = b.StringBuilder.Length;
                        
                        var wasApiProperty = isApiProperty;
                        SpecialCase_ConvertIdentifier(identifier, out isApiProperty, out var dontContinue);
                        if (dontContinue)
                            break;

                        if (!isApiProperty && !wasFirst)
                            b.StringBuilder.Insert(start, '.');
                        
                        if (wasApiProperty && !isApiProperty)
                        {
                            throw new InvalidOperationException("Can't go onto a normal property from an API property");
                        }
                    }

                    // calling b.ToString() will put braces which is not intended here
                    oldB.StringBuilder.Append(b.StringBuilder.ToString());
                    b = oldB;
                    break;
                }

                case LambdaExpressionSyntax:
                {
                    var child = node.ChildNodes();
                    var parameter = child.ElementAt(0);
                    var body = child.ElementAt(1);

                    break;
                }

                // X
                // X -> a
                case IdentifierNameSyntax:
                {
                    Log($"-> {node}");
                    SpecialCase_ConvertIdentifier(node, out _, out _);
                    break;
                }

                // else { a }
                case ElseClauseSyntax:
                {
                    var childNodes = node.ChildNodes();
                    var block = childNodes.ElementAt(0);

                    var isIfStatement = block is IfStatementSyntax;
                    if (!isIfStatement)
                    {
                        b.StringBuilder.Append("Else(() =>");
                        b.BeginBracket();
                    }

                    Convert(block);
                    
                    if (!isIfStatement)
                    {
                        b.EndBracket();
                        b.StringBuilder.Append(");");
                    }

                    break;
                }

                // if (a) { b } 
                case IfStatementSyntax statement:
                {
                    var childNodes = statement.ChildNodes();
                    var condition = childNodes.ElementAt(0);
                    var block = childNodes.ElementAt(1);

                    var isParentElseClause = statement.Parent is ElseClauseSyntax;

                    b.AppendLine(isParentElseClause ? "ElseIf(new[]" : "gen.If(new[]");
                    
                    b.BeginBracket();
                    {
                        b.AppendLine("() => ");
                        SpecialCase_ConvertCondition(condition);
                    }
                    b.EndBracket();
                    b.StringBuilder.Append(',');
                    
                    b.AppendLine("() => ");
                    b.BeginBracket();
                    {
                        Convert(block);
                    }
                    b.EndBracket();
                    b.StringBuilder.Append(')');
                    if (childNodes.ElementAtOrDefault(2) is { } clause)
                    {
                        b.StringBuilder.Append('.');
                        Convert(clause);
                    }
                    else
                    {
                        b.StringBuilder.Append(';');
                    }
                    
                    break;
                }

                // X1 a X2
                // X1 -> var type
                // X2 -> '' '= b'
                case LocalDeclarationStatementSyntax:
                {
                    var variableDeclaration = node.ChildNodes().First() as VariableDeclarationSyntax;
                    if (variableDeclaration == null)
                    {
                        Log("Invalid declaration syntax!");
                        break;
                    }

                    var childNodes = variableDeclaration.ChildNodes();
                    if (childNodes.ElementAt(1) is not VariableDeclaratorSyntax declarator)
                    {
                        Log("Declarator not found");
                        break;
                    }

                    var identifier = declarator.GetFirstToken();
                    // declarator -> Initializer -> Value
                    if (declarator.ChildNodes().Any())
                    {
                        var value = declarator.ChildNodes().First()
                            .ChildNodes().First();
                        
                        b.AppendLine("var v_");
                        b.StringBuilder.Append(identifier);
                        b.StringBuilder.Append(" = ");
                        if (value.Kind() != SyntaxKind.NullLiteralExpression)
                        {
                            b.StringBuilder.Append("gen.Declare(() => ");
                            Convert(value);
                            b.StringBuilder.Append(");");
                        }
                        else
                        {
                            var type = SpecialCase_GetMsTypeName(childNodes.ElementAt(0));

                            b.StringBuilder.Append("gen.Declare<");
                            b.StringBuilder.Append(type);
                            b.StringBuilder.Append(">(() => ");
                            Convert(value);
                            b.StringBuilder.Append(");");
                        }
                    }
                    // this is a typed variable
                    else
                    {
                        var type = SpecialCase_GetMsTypeName(childNodes.ElementAt(0));
                        
                        b.AppendLine("var v_");
                        b.StringBuilder.Append(identifier);
                        b.StringBuilder.Append(" = ");
                        b.StringBuilder.Append("gen.Declare<");
                        b.StringBuilder.Append(type);
                        b.StringBuilder.Append(">();");
                    }

                    break;
                }
                
                

                // a(b)
                case InvocationExpressionSyntax invocation:
                {
                    var childNodes = invocation.ChildNodes();
                    var methodSymbol = ModelExtensions.GetSymbolInfo(semanticModel, invocation).Symbol as IMethodSymbol;
                    if (methodSymbol == null)
                    {
                        Log($"Method with expression '{invocation}' not found");
                        break;
                    }

                    var arguments = childNodes.ElementAt(1);
                    if (arguments is not ArgumentListSyntax)
                    {
                        Log("Expected an ArgumentListSyntax");
                        break;
                    }

                    Log($"method: '{methodSymbol}' '{arguments}'");
                    var attribute = methodSymbol.GetAttributes()
                        .FirstOrDefault(data => data.AttributeClass.Name.StartsWith("ManiaScriptApi"));
                    if (attribute == null)
                    {
                        attribute = methodSymbol.GetAttributes()
                            .FirstOrDefault(data => data.AttributeClass!.Name.StartsWith("ManiaScriptMethod"));

                        // User method set with [ManiaScriptMethod]
                        if (attribute != null)
                        {
                            Log(":> Is user method");

                            var baseType = methodSymbol.ContainingType.GetTypeName();
                            b.AppendLine($"gen.Method((global::ManiaGen.ManiaPlanet.IScriptValue.ScriptMethod) ");
                            if (methodSymbol.IsStatic)
                            {
                                b.StringBuilder.Append(baseType);
                                b.StringBuilder.Append('.');
                            }
                            else if (false == SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType, symbol.ContainingType))
                            {
                                SpecialCase_ConvertIdentifier(childNodes.ElementAt(0), out _, out _);
                                b.StringBuilder.Append('.');
                            }

                            b.StringBuilder.Append("MS_");
                            b.StringBuilder.Append(methodSymbol.Name);
                            b.StringBuilder.Append("(gen), new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                            b.BeginBracket();

                            var i = 0;
                            foreach (var syntaxNode in arguments.ChildNodes())
                            {
                                Log("found: " + syntaxNode);

                                b.AppendLine("() => ");
                                
                                // not needed?
                                // b.StringBuilder.Append("(global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                                // b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(methodSymbol.Parameters[i++].Type));
                                // b.StringBuilder.Append(">) ");
                            
                                var arg = (ArgumentSyntax) syntaxNode;
                                Convert(arg.ChildNodes().First());
                                b.StringBuilder.Append(',');
                            }
                            b.EndBracket();
                            
                            b.StringBuilder.Append(')');
                        }
                        // .NET Method that are defined via generator.MapNetMethod(...);
                        else
                        {
                            // We only support 'ToString()' for now
                            // TODO: in future, add a way in the generator to supply external methods so that we can remove this case
                            // Example:
                            //  generator.CreateNetMethodMapping(typeof(object), nameof(ToString), args => {});
                            b.AppendLine("gen.NetMethod(");
                            if (!methodSymbol.IsStatic)
                            {
                                b.StringBuilder.Append("typeof(");
                                b.StringBuilder.Append(methodSymbol.ContainingType.GetTypeName());
                                b.StringBuilder.Append("), ");
                            }

                            b.StringBuilder.Append('"');
                            b.StringBuilder.Append(methodSymbol.Name);
                            b.StringBuilder.Append("\", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                            b.BeginBracket();

                            {
                            }
                            
                            foreach (var syntaxNode in new[] {childNodes.ElementAt(0)}.Concat(arguments.ChildNodes()))
                            {
                                Log("found: " + syntaxNode);

                                b.AppendLine("() => ");
                                Convert(syntaxNode.ChildNodes().First());
                                b.StringBuilder.Append(',');
                            }
                            b.EndBracket();
                            
                            b.StringBuilder.Append(')');
                        }
                    }
                    else
                    {
                        var type = (INamedTypeSymbol) attribute.ConstructorArguments[0].Value!;
                        var name = type.GetTypeName();
                        Log($"Type: {name}");
                        
                        b.AppendLine($"{name}.Call(gen");
                        b.BeginScope();
                        
                        var apiFunc = type.Interfaces[0];
                        
                        var i = 0;
                        if (type.AllInterfaces.Any(i => i.Name == "IDynamicApiFunc"))
                        {
                            var path = childNodes.ElementAt(0);
                            b.StringBuilder.Append(", ");
                            b.AppendLine();
                            
                            b.StringBuilder.Append("() => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                            b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(apiFunc.TypeArguments[0]));
                            b.StringBuilder.Append(">>(");
                            foreach (var n in path.ChildNodes())
                                Convert(n);
                            b.StringBuilder.Append(')');

                            i += 1;
                        }
                        
                        foreach (var syntaxNode in arguments.ChildNodes())
                        {
                            Log("found: " + syntaxNode);
                            
                            b.StringBuilder.Append(", ");
                            b.AppendLine();
                            
                            b.StringBuilder.Append("() => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                            b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(apiFunc.TypeArguments[i++]));
                            b.StringBuilder.Append(">>(");
                            
                            var arg = (ArgumentSyntax) syntaxNode;
                            Convert(arg.ChildNodes().First());
                            
                            b.StringBuilder.Append(')');
                        }
                        
                        b.EndScope();
                        b.AppendLine(")");
                    }
                    
                    break;
                }

                // mostly a[b]
                case ElementAccessExpressionSyntax:
                {
                    var children = node.ChildNodes();
                    var identifier = children.ElementAt(0);
                    // Access to method args (there could be a better way to do that
                    if (semanticModel.GetSymbolInfo(identifier).Symbol is IParameterSymbol {Name: "args"})
                    {
                        
                    }
                    break;
                }

                // a
                case LiteralExpressionSyntax literal:
                {
                    if (literal.Kind() == SyntaxKind.NullLiteralExpression)
                    {
                        b.StringBuilder.Append("null");
                        break;
                    }
                    
                    b.StringBuilder.Append("(global::ManiaGen.ManiaPlanet.IScriptValue.");

                    var ret = literal.ToString();
                    var type = literal.Kind() switch
                    {
                        SyntaxKind.StringLiteralExpression => "Text",
                        SyntaxKind.NumericLiteralExpression => ret.Contains('.') switch
                        {
                            true => "Real",
                            false => "Integer"
                        },
                        SyntaxKind.FalseLiteralExpression or SyntaxKind.TrueLiteralExpression => "Boolean",
                        _ => throw new InvalidOperationException("Not found for " + literal.Kind())
                    };

                    b.StringBuilder.Append(type);
                    b.StringBuilder.Append(") ");
                    b.StringBuilder.Append(ret);
                    break;
                }

                case ReturnStatementSyntax:
                {
                    var children = node.ChildNodes();
                    var value = children.ElementAt(0);
                    
                    b.AppendLine("return gen.Return(() => ");
                    Convert(value);
                    b.StringBuilder.Append(");");
                    break;
                }
            }
        }

        foreach (var child in blockSyntaxNodes[0].ChildNodes())
            Convert(child);*/

        if (!setup.forceInline)
        {
            if (symbol.ReturnType.SpecialType == SpecialType.System_Void)
                b.AppendLine("return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);");
            
            b.EndScope();
            b.AppendLine("}, new Type[] {");
            b.BeginScope();
            b.AppendLine($"typeof({generator.SpecialCase_GetMsTypeNameFromSymbol(symbol.ReturnType)}),");
            foreach (var param in symbol.Parameters)
            {
                b.AppendLine($"typeof({generator.SpecialCase_GetMsTypeNameFromSymbol(param.Type)}),");
            }

            var allowGeneric = symbol.Parameters.Any(p => p.Type is ITypeParameterSymbol);

            b.EndScope();
            b.AppendLine($"}}, allowGeneric: {(allowGeneric ? "true" : "false")}));");
            b.EndScope();
            
            b.AppendLine($"return m_{symbol.Name};");
        }
        
        b.EndBracket();
        b.StringBuilder.Append(";");

        Log(b);
        
        var hint = $"Script.{Path.GetFileNameWithoutExtension(tree.FilePath)}.{symbol.Name}";
        
        Context.AddSource(hint, b.ToString());
    }
}
#endif
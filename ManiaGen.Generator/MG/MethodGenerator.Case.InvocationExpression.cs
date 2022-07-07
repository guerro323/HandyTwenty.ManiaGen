using HandyTwenty.ManialinkGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ManiaGen.Generator;

public partial class MethodGenerator
{
    private void Case_InvocationExpressionSyntax(InvocationExpressionSyntax invocation)
    {
        var childNodes = invocation.ChildNodes();
        var methodSymbol = ModelExtensions.GetSymbolInfo(semanticModel, invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
        {
            Log($"Method with expression '{invocation}' not found");
            return;
        }

        var arguments = childNodes.ElementAt(1);
        if (arguments is not ArgumentListSyntax)
        {
            Log("Expected an ArgumentListSyntax");
            return;
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
                else if (false == SymbolEqualityComparer.Default.Equals(methodSymbol.ContainingType,
                             symbol.ContainingType))
                {
                    var start = b.StringBuilder.Length;
                    SpecialCase_ConvertIdentifier(childNodes.ElementAt(0), start, out _, out _);
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
                    /*b.StringBuilder.Append("(global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                            b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(methodSymbol.Parameters[i++].Type));
                            b.StringBuilder.Append(">) ");*/

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
                var identifier = childNodes.ElementAt(0);
                if (identifier.Kind() == SyntaxKind.SimpleMemberAccessExpression
                    && identifier.ChildNodes().ElementAt(0).Kind() == SyntaxKind.BaseExpression)
                {
                    if (methodSymbol.Name == "Entry")
                    {
                        // kinda poopy for now, later have a better support for .base()
                        b.StringBuilder.Append(identifier.ToString().Replace("Entry", "Generate(gen, ___generatedObjects)"));
                    }
                }
                else
                {
                    // We only support 'ToString()' for now
                    // TODO: in future, add a way in the generator to supply external methods so that we can remove this case
                    // Example:
                    //  generator.CreateNetMethodMapping(typeof(object), nameof(ToString), args => {});
                    b.AppendLine("gen.NetMethod(");
                    b.StringBuilder.Append("typeof(");
                    b.StringBuilder.Append(methodSymbol.ContainingType.GetTypeName());
                    b.StringBuilder.Append("), ");

                    b.StringBuilder.Append('"');
                    b.StringBuilder.Append(methodSymbol.Name);
                    b.StringBuilder.Append("\", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                    b.BeginBracket();

                    var first = true;
                    foreach (var syntaxNode in new[] {childNodes.ElementAt(0)}.Concat(arguments.ChildNodes()))
                    {
                        if (first)
                        {
                            first = false;
                            if (methodSymbol.IsStatic && !methodSymbol.IsExtensionMethod)
                                continue;
                        }
                        
                        Log("found: " + syntaxNode);

                        b.AppendLine("() => ");
                        Convert(syntaxNode.ChildNodes().First());
                        b.StringBuilder.Append(',');
                    }

                    b.EndBracket();

                    b.StringBuilder.Append(')');
                }
            }
        }
        // [ManiaScriptAPI] Methods
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

                b.StringBuilder.Append(
                    "() => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(apiFunc.TypeArguments[0]));
                b.StringBuilder.Append(">>(");
                var isApiProperty = false;
                var dontContinue = false;

                var start = b.StringBuilder.Length;
                GeneratePath(path, info => info.Scope == MemberPathInfo.EScope.Method);
                /*foreach (var n in path.ChildNodes())
                {
                    // TODO: very rudementary (we need to know the exact index in path of the method)
                    if (n.ToString() == methodSymbol.Name)
                        break;
                    
                    SpecialCase_ConvertIdentifier(n, start, out isApiProperty, out dontContinue);
                    if (dontContinue)
                        break;
                }*/

                b.StringBuilder.Append(')');

                i += 1;
            }

            foreach (var syntaxNode in arguments.ChildNodes())
            {
                Log("found: " + syntaxNode);

                b.StringBuilder.Append(", ");
                b.AppendLine();

                b.StringBuilder.Append(
                    "() => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<");
                b.StringBuilder.Append(SpecialCase_GetMsTypeNameFromSymbol(apiFunc.TypeArguments[i++]));
                b.StringBuilder.Append(">>(");

                var arg = (ArgumentSyntax) syntaxNode;
                Convert(arg.ChildNodes().First());

                b.StringBuilder.Append(')');
            }

            b.EndScope();
            b.AppendLine(")");
        }

        return;
    }
}
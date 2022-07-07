using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HandyTwenty.ManialinkGenerator;

public partial class MethodGenerator
{
    public delegate void LogMethod(string text, int level = 0);
    
    private LogMethod Log;
    private CodeBuilder b;
    private SemanticModel semanticModel;
    private ISymbol symbol;
    private SyntaxNode blockSyntax;
    
    public MethodGenerator(CodeBuilder codeBuilder, SemanticModel semanticModel, ISymbol symbol,
        SyntaxNode blockSyntax, LogMethod log)
    {
        this.b = codeBuilder;
        this.semanticModel = semanticModel;
        this.symbol = symbol;
        this.blockSyntax = blockSyntax;
        this.Log = log;
    }
    
    private HashSet<string> globalSymbolSet = new();
    private CodeBuilder globalBuilder;

    public void Generate()
    {
        var globalStart = b.StringBuilder.Length;
        globalBuilder = new CodeBuilder(b);

        foreach (var child in blockSyntax.ChildNodes())
            Convert(child);
        
        b.StringBuilder.Insert(globalStart, globalBuilder.StringBuilder);
    }

    public void Convert(SyntaxNode node)
    {
        Log(node.GetType().Name + ";;" + node.Kind(), 1);
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
                Case_AssignmentExpressionSyntax(node);
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
                Case_BinaryExpressionSyntax(node);
                break;
            }

            case MemberAccessExpressionSyntax:
            {
                GeneratePath(node);
                break;
            }

            case ObjectCreationExpressionSyntax:
            {
                var children = node.ChildNodes();
                var type = (INamedTypeSymbol) semanticModel.GetSymbolInfo(children.ElementAt(0)).Symbol;
                var arguments = children.ElementAt(1);

                b.StringBuilder.Append("gen.NetMethod(typeof(");
                b.StringBuilder.Append(type.GetTypeName());
                b.StringBuilder.Append("), \".ctor\", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                b.BeginBracket();
                foreach (var arg in arguments.ChildNodes())
                {
                    b.AppendLine("() => ");
                    // Argument -> ???
                    Convert(arg.ChildNodes().First());
                    b.StringBuilder.Append(',');
                }
                b.EndBracket();
                b.StringBuilder.Append(')');
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

                var start = b.StringBuilder.Length;
                SpecialCase_ConvertIdentifier(node, start, out _, out _);

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

            case WhileStatementSyntax:
            {
                var childNodes = node.ChildNodes();
                var condition = childNodes.ElementAt(0);
                var block = childNodes.ElementAt(1);
                
                b.AppendLine("gen.While(new[]");

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
                b.StringBuilder.Append(';');

                break;
            }

            case ForEachStatementSyntax:
            {
                var childNodes = node.ChildNodes();
                var valueIdentifier = node.ChildTokens().ElementAt(2);
                var arrayIdentifier = childNodes.ElementAt(1);
                var block = childNodes.ElementAt(2);
                
                b.AppendLine("gen.ForEach(() => ");
                Convert(arrayIdentifier);
                b.StringBuilder.Append(", ");
                
                b.AppendLine("v_");
                b.StringBuilder.Append(valueIdentifier);
                b.StringBuilder.Append(" => ");
                b.BeginBracket();
                {
                    Convert(block);
                }
                b.EndBracket();
                b.StringBuilder.Append(')');
                b.StringBuilder.Append(';');

                break;
            }

            // X1 a X2
            // X1 -> var type
            // X2 -> '' '= b'
            case LocalDeclarationStatementSyntax:
            {
                Case_LocalDeclarationStatementSyntax(node);
                break;
            }



            // a(b)
            case InvocationExpressionSyntax invocation:
            {
                Case_InvocationExpressionSyntax(invocation);
                break;
            }

            // mostly a[b]
            case ElementAccessExpressionSyntax:
            {
                var children = node.ChildNodes();
                var arrayIdentifier = children.ElementAt(0);
                var indexIdentifier = children
                    .ElementAt(1) // ArgumentList
                    .ChildNodes()
                    .ElementAt(0) // Argument
                    .ChildNodes()
                    .ElementAt(0); // ???

                var arraySymbol = semanticModel.GetSymbolInfo(arrayIdentifier).Symbol!;
                var type = arraySymbol switch
                {
                    IPropertySymbol s => s.Type,
                    IFieldSymbol s => s.Type,
                    ILocalSymbol s => s.Type
                };

                b.StringBuilder.Append("gen.NetMethod(typeof(");
                b.StringBuilder.Append(type);
                b.StringBuilder.Append("), \".[]\", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                b.BeginBracket();
                {
                    b.AppendLine("() => ");
                    Convert(arrayIdentifier);
                    b.StringBuilder.Append(',');
                    b.AppendLine("() => ");
                    Convert(indexIdentifier);
                }
                b.EndBracket();
                b.StringBuilder.Append(')');

                break;
            }

            // X1 where X1 was in a text such as:
            // $"X1"
            // (it's just a simple text)
            case InterpolatedStringTextSyntax:
            {
                b.StringBuilder.Append("(global::ManiaGen.ManiaPlanet.IScriptValue.Text) \"");
                b.StringBuilder.Append(node);
                b.StringBuilder.Append("\"");
                break;
            }

            // {X1:X2}
            case InterpolationSyntax:
            {
                // for now only convert X1
                Convert(node.ChildNodes().ElementAt(0));
                // TODO: :X2 (mostly useful for floats) (implement it like a method?)
                break;
            }

            // $"X1 {X2} X3 {X4} X5..."
            case InterpolatedStringExpressionSyntax:
            {
                b.StringBuilder.Append("gen.InterpolatedText(new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]");
                b.BeginBracket();
                foreach (var child in node.ChildNodes())
                {
                    b.AppendLine("() => ");
                    Convert(child);
                    b.StringBuilder.Append(',');
                }
                b.EndBracket();
                b.StringBuilder.Append(')');
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
                    SyntaxKind.CharacterLiteralExpression => "Text",
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

            default:
            {
                Log($"not found;{node.GetType().Name};kind={node.Kind()}");
                break;
            }
        }
    }
}
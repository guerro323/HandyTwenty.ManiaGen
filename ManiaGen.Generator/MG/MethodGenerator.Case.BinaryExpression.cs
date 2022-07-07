using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ManiaGen.Generator;

public partial class MethodGenerator
{
    private void Case_BinaryExpressionSyntax(SyntaxNode node)
    {
        var children = node.ChildNodes();
        switch (node.Kind())
        {
            case SyntaxKind.AddExpression:
            case SyntaxKind.MultiplyExpression:
            case SyntaxKind.DivideExpression:
            case SyntaxKind.ModuloExpression:
            case SyntaxKind.LogicalOrExpression:
            case SyntaxKind.LogicalAndExpression:
            case SyntaxKind.GreaterThanExpression:
            case SyntaxKind.GreaterThanOrEqualExpression:
            case SyntaxKind.LessThanExpression:
            case SyntaxKind.LessThanOrEqualExpression:
            case SyntaxKind.EqualsExpression:
            case SyntaxKind.NotEqualsExpression:
                var left = children.ElementAt(0);
                var right = children.ElementAt(1);

                b.StringBuilder.Append("gen.");
                b.StringBuilder.Append(node.Kind() switch
                {
                    SyntaxKind.AddExpression => "Add",
                    SyntaxKind.MultiplyExpression => "Multiply",
                    SyntaxKind.DivideExpression => "Divide",
                    SyntaxKind.ModuloExpression => "Modulo",
                    SyntaxKind.LogicalOrExpression => "Or",
                    SyntaxKind.LogicalAndExpression => "And",
                    SyntaxKind.GreaterThanExpression => "Greater",
                    SyntaxKind.GreaterThanOrEqualExpression => "GreaterOrEqual",
                    SyntaxKind.LessThanExpression => "Less",
                    SyntaxKind.LessThanOrEqualExpression => "LessOrEqual",
                    SyntaxKind.EqualsExpression => "Equal",
                    SyntaxKind.NotEqualsExpression => "NotEqual",
                });

                b.StringBuilder.Append("(() => ");
                //SpecialCase_ConvertCondition(left);
                Convert(left);
                b.StringBuilder.Append(", () => ");
                //SpecialCase_ConvertCondition(right);
                Convert(right);
                b.StringBuilder.Append(')');

                break;
            
            case SyntaxKind.AsExpression:
            {
                var identifier = children.ElementAt(0);
                var type = children.ElementAt(1);

                b.StringBuilder.Append("gen.Cast<");
                b.StringBuilder.Append(SpecialCase_GetMsTypeName(type));
                b.StringBuilder.Append(">(() => ");
                Convert(identifier);
                b.StringBuilder.Append(")");
                break;
            }
            
            // TODO: case SyntaxKind.CoalesceExpression:
            // use if and export the variable outside when needed (eg: when in a method call)
        }

        return;
    }
}
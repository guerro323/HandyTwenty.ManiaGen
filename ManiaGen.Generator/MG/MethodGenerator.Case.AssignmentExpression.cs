using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ManiaGen.Generator;

public partial class MethodGenerator
{
    private void Case_AssignmentExpressionSyntax(SyntaxNode node)
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

        return;
    }
}
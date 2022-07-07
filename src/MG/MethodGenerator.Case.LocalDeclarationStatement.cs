using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HandyTwenty.ManialinkGenerator;

public partial class MethodGenerator
{
    private void Case_LocalDeclarationStatementSyntax(SyntaxNode node)
    {
        var variableDeclaration = node.ChildNodes().First() as VariableDeclarationSyntax;
        if (variableDeclaration == null)
        {
            Log("Invalid declaration syntax!");
            return;
        }

        var childNodes = variableDeclaration.ChildNodes();
        if (childNodes.ElementAt(1) is not VariableDeclaratorSyntax declarator)
        {
            Log("Declarator not found");
            return;
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
                b.StringBuilder.Append(", \"");
                b.StringBuilder.Append(identifier);
                b.StringBuilder.Append("\");");
            }
            else
            {
                var type = SpecialCase_GetMsTypeName(childNodes.ElementAt(0));

                b.StringBuilder.Append("gen.Declare<");
                b.StringBuilder.Append(type);
                b.StringBuilder.Append(">(() => ");
                Convert(value);
                b.StringBuilder.Append(", \"");
                b.StringBuilder.Append(identifier);
                b.StringBuilder.Append("\");");
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

        return;
    }
}
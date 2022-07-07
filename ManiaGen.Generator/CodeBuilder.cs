using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ManiaGen.Generator;

public class CodeBuilder
{
    public readonly StringBuilder StringBuilder = new();

    private int _scopeLevel = 0;

    public CodeBuilder()
    {
        
    }

    public CodeBuilder(CodeBuilder copy)
    {
        _scopeLevel = copy._scopeLevel;
    }
    
    public void BeginScope()
    {
        _scopeLevel++;
    }

    public void BeginBracket()
    {
        AppendLine("{");
        BeginScope();
    }

    public void EndBracket()
    {
        EndScope();
        AppendLine("}");
    }

    public string GetLinePrefix()
    {
        return $"\n{string.Join(null, Enumerable.Repeat("    ", _scopeLevel))}";
    }

    public void AppendLine()
    {
        StringBuilder.Append(GetLinePrefix());
    }
    
    public void AppendLine(string text)
    {
        StringBuilder.Append(GetLinePrefix());
        StringBuilder.Append(text);
    }

    public void EndScope()
    {
        _scopeLevel--;
    }
    
    private static IEnumerable<INamedTypeSymbol> GetParentTypes(ISymbol method)
    {
        var type = method.ContainingType;
        while (type != null)
        {
            yield return type;
            type = type.ContainingType;
        }
    }

    public void Encapsulate(ISymbol symbol)
    {
        var imports = symbol.DeclaringSyntaxReferences
            .First()
            .SyntaxTree
            .GetRoot()
            .DescendantNodes()
            .Where(node => node is UsingDirectiveSyntax);
        foreach (var import in imports)
        {
            AppendLine(import.ToString());
        }
        
        AppendLine($"namespace {symbol.ContainingNamespace};\n");

        var parentTypes = GetParentTypes(symbol)
            .Reverse()
            .ToList();
        
        foreach (var parentType in parentTypes)
        {
            static string GetKindStr(ITypeSymbol s)
            {
                return s.TypeKind switch
                {
                    TypeKind.Class => "class",
                    TypeKind.Struct => "struct",
                    _ => s.TypeKind.ToString().ToLower()
                };
            }
            
            AppendLine();

            StringBuilder
                .Append("partial ")
                .Append(GetKindStr(parentType))
                .Append(' ')
                .Append(parentType.Name);
            BeginBracket();
        }
    }

    public void Flush()
    {
        while (_scopeLevel > 0)
            EndBracket();
    }

    public override string ToString()
    {
        Flush();
        return StringBuilder.ToString();
    }
}
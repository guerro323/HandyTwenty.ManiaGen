using Microsoft.CodeAnalysis;

namespace ManiaGen.Generator;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<string> Log = new();
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        
    }
}
using Microsoft.CodeAnalysis;

namespace HandyTwenty.ManialinkGenerator;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<string> Log = new();
    
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        
    }
}
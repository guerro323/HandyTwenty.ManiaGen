using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HandyTwenty.ManialinkGenerator;

public class FlowSubGenerator : SubGenerator
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
                         .OfType<ClassDeclarationSyntax>())
            {
                tasks.Add(() =>
                {
                    if (string.IsNullOrEmpty(tree.FilePath)
                        || tree.FilePath.Contains("Generated/"))
                        return;

                    var symbol = (ITypeSymbol) ModelExtensions.GetDeclaredSymbol(semanticModel, declare)!;
                    if (!symbol.AllInterfaces
                            .Any(t => t.Name.StartsWith("IManiaScriptEntry")))
                        return;

                    Log($"Found Class {symbol.Name}");
                    try
                    {
                       FoundClass(tree, semanticModel, symbol);
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
    
    private void FoundClass(SyntaxTree tree, SemanticModel semanticModel, ITypeSymbol symbol)
    {
        var methodSymbol = symbol.GetMembers()
            .FirstOrDefault(m => m.Name == "Entry");
        if (methodSymbol == null)
            throw new InvalidOperationException("No method named 'Entry' found");
        
        var bodySyntax = (methodSymbol.DeclaringSyntaxReferences
            .First()
            .GetSyntax() as MethodDeclarationSyntax)!;
        
        var bodyNodes = bodySyntax.ChildNodes();
        var blockSyntaxNode = bodyNodes.Last();

        if (blockSyntaxNode.Kind() != SyntaxKind.Block)
            throw new InvalidOperationException("Expected a block but had " + blockSyntaxNode.Kind());

        var memberOverride = methodSymbol.IsOverride ? "override" : "virtual";
        
        var b = new CodeBuilder();
        b.Encapsulate(methodSymbol);
        b.AppendLine($"public {memberOverride} void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)");
        b.BeginBracket();
        
        if (symbol.AllInterfaces.Any(i => i.Name.StartsWith("IManiaScriptEntryGenerateOthers")))
        {
            b.AppendLine("GenerateOthers(gen, ___generatedObjects);");
        }

        foreach (var variable in symbol.GetMembers())
        {
            var type = variable switch
            {
                IPropertySymbol s => s.Type,
                IFieldSymbol s => s.Type,
                _ => null
            };
            // Maybe don't check for the interface, but check for the 'Generate' method?
            // This would allow users to create their own entry without needing the source generator
            if (type == null || !type.AllInterfaces.Any(t => t.Name.StartsWith("IManiaScriptEntry")))
                continue;
            
            Log($"Found a flow variable named '{variable.Name}'");
            b.AppendLine($"if (!___generatedObjects.Contains({variable.Name}))");
            b.BeginBracket();
            b.AppendLine(variable.Name);
            b.StringBuilder.Append(".Generate(gen, ___generatedObjects);");
            b.AppendLine($"___generatedObjects.Add({variable.Name});");
            b.EndBracket();
        }

        var generator = new MethodGenerator(b, semanticModel, symbol, blockSyntaxNode, Log);
        generator.Generate();

        b.EndBracket();

        Log(b);
        
        var hint = $"Flow.{Path.GetFileNameWithoutExtension(tree.FilePath)}.{symbol.Name}";
        
        lock (this) {
            Context.AddSource(hint, b.ToString());
        }
    }
}
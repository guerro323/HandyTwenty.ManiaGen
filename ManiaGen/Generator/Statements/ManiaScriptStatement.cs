using System.Runtime.InteropServices;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator.Statements;

public abstract record ManiaScriptStatement
{
    public bool IsColonLess() => this is IColonLessStatement {DisableColonLess: false};

    public static void GetChildrenStatements(ManiaScriptStatement statement, List<(ManiaScriptStatement, int)> result, int depth = 0)
    {
        result.Add((statement, depth));
        if (statement is not CollectionStatement collection)
            return;
        
        foreach (var child in collection.Statements)
            GetChildrenStatements(child, result, depth + 1);
    }
    
    public static void RecursivelyUpdateStatement(ref ManiaScriptStatement statement, IScriptValue find, ManiaScriptStatement with)
    {
        if (statement.Creator == find)
        {
            statement = with;
            return;
        }
        
        if (statement is BodyStatement collection)
        {
            foreach (ref var child in CollectionsMarshal.AsSpan(collection.Statements))
                RecursivelyUpdateStatement(ref child, find, with);
        }
    }
    
    public static void RecursivelyUpdateStatement(ref ManiaScriptStatement statement, Func<ManiaScriptStatement, bool> find, Func<ManiaScriptStatement, ManiaScriptStatement> with)
    {
        if (find(statement))
        {
            statement = with(statement);
            return;
        }
        
        if (statement is BodyStatement collection)
        {
            foreach (ref var child in CollectionsMarshal.AsSpan(collection.Statements))
                RecursivelyUpdateStatement(ref child, find, with);
        }
    }
    
    public IScriptValue? Creator { get; set; }

    public abstract void Generate(ManiaStringBuilder builder);
}
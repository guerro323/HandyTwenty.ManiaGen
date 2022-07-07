namespace ManiaGen.Generator.Statements;

public record ScriptStatement(
        List<ManiaScriptStatement> Globals,
        List<ManiaScriptStatement> Labels,
        List<ManiaScriptStatement> Methods,
        List<ManiaScriptStatement> Statements)
    : CollectionStatement(Statements)
{
    public override void Generate(ManiaStringBuilder builder)
    {
        void Generate(Action action, int lines = 1)
        {
            var start = builder.StringBuilder.Length;
            action();
            if (start != builder.StringBuilder.Length)
            {
                for (var i = 0; i < lines; i++)
                    builder.AppendLine();
            }
        }
        
        foreach (var global in Globals)
        {
            Generate(() => global.Generate(builder));
        }
        
        if (Globals.Count > 0) builder.AppendLine();
        
        foreach (var label in Labels)
        {
            Generate(() => label.Generate(builder), 2);
        }
        
        foreach (var method in Methods)
        {
            Generate(() => method.Generate(builder));
        }

        if (Methods.Count > 0) builder.AppendLine();

        builder.StringBuilder.Append("main() ");
        var blockStatement = new BlockStatement(Statements);
        blockStatement.Generate(builder);
    }
}
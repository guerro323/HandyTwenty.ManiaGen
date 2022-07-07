using ManiaGen.Generator.Statements;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator;

public partial class ManiaScriptGenerator
{
    /// <summary>
    /// Create an interpolated text using {{{ }}} in ManiaScript
    /// </summary>
    /// <example>
    /// Hello {world}!
    ///     var world = Declare((Text) "world!");
    ///     InterpolatedText(() => {
    ///         () => (Text) "Hello",
    ///         () => world,
    ///         () => (Text) "!",
    ///     });
    /// </example>
    public IScriptValue InterpolatedText(Func<IScriptValue>[] args)
    {
        var compiledArgs = args.Select(Compile);
        Root.Statements.Add(
            new InterpolatedTextStatement(
                compiledArgs.Select(a => (ManiaScriptStatement) (a.scope with { IsList = true })).ToList()
            )
        );

        // TODO: Interpolate string (needed for returning a constant value)
        return IScriptValue.Text.Default;
    }

    public record InterpolatedTextStatement(List<ManiaScriptStatement> Statements) : BodyStatement(Statements)
    {
        public override void Generate(ManiaStringBuilder builder)
        {
            var sb = builder.StringBuilder;
            if (Statements.Count == 0)
            {
                // Empty text
                sb.Append("\"\"");
                return;
            }

            sb.Append("\"\"\"");
            foreach (var statement in Statements)
            {
                if (statement.Creator is IScriptValue.Text text)
                {
                    // normal text
                    // (don't call statement.Generate since it will put the " ")
                    builder.StringBuilder.Append(text.Value);
                    continue;
                }

                // argument
                sb.Append("{{{");
                statement.Generate(builder);
                sb.Append("}}}");
            }

            sb.Append("\"\"\"");
        }
    }
}
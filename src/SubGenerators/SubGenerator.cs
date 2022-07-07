using Microsoft.CodeAnalysis;

namespace HandyTwenty.ManialinkGenerator;

public abstract class SubGenerator
{
    public GeneratorExecutionContext Context { get; private set; }
    public SyntaxReceiver Receiver { get; private set; }

    // settable
    public Compilation Compilation { get; protected set; }

    protected virtual void Log<T>(T obj, int indent = 0)
    {
        Receiver.Log.Add(string.Join(null, Enumerable.Repeat("\t", indent)) + obj);
    }

    protected abstract void Generate();

    public static T Make<T>(T generator,
        GeneratorExecutionContext context, SyntaxReceiver receiver, ref Compilation compilation)
        where T : SubGenerator
    {
        generator.Context = context;
        generator.Receiver = receiver;
        generator.Compilation = compilation;

        generator.Generate();

        return generator;
    }
}
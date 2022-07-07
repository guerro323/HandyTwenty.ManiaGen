using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen.Generator.Flow;

public interface IManiaScriptEntry
{
    CNod NodNonGeneric { get; set; }

    void Generate(ManiaScriptGenerator gen, HashSet<object> generatedObjects);
    void Entry();
}

public interface IManiaScriptEntryGenerateOthers
{
    void GenerateOthers(ManiaScriptGenerator gen, HashSet<object> generatedObjects);
}

public interface IManiaScriptEntry<T> : IManiaScriptEntry
    where T : CNod
{
    CNod IManiaScriptEntry.NodNonGeneric
    {
        get => Nod;
        set => Nod = (T) value;
    }

    T Nod { get; set; }
}
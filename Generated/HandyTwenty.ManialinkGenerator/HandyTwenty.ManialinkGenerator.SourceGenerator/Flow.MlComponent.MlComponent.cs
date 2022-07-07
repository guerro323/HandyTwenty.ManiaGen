
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen.HTManialink;

partial class MlComponent
{
    public virtual void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
    }
}
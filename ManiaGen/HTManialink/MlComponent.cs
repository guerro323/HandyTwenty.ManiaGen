using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen.HTManialink;

public abstract partial class MlComponent : CMlFrame, 
    IManiaScriptEntry<CMlScriptExtended>, IManiaScriptEntryGenerateOthers,
    IScriptValue.IDisableAllRepresentation
{
    /// <summary>
    /// Called before ***Main***. Use it to create and initialize variables.
    /// </summary>
    public EventLabel PreMainLabel => Nod.PreMainLabel;

    /// <summary>
    /// Called after **PreMain***. Use it to initialize and configure data.
    /// </summary>
    public EventLabel MainLabel => Nod.MainLabel;

    /// <summary>
    /// Called before ***Update***. Use it to update values that are required.
    /// </summary>
    public EventLabel PreUpdateLabel => Nod.PreUpdateLabel;

    public EventLabel<CMlScriptEvent> PendingEventLabel => Nod.PendingEventLabel;

    /// <summary>
    /// ***Update*** Label
    /// </summary>
    public EventLabel UpdateLabel => Nod.UpdateLabel;

    /// <summary>
    /// ***Animate*** Label updated after ***Update***. Use it to animate your components.
    /// </summary>
    /// <remarks>
    /// This label may not be added to the final code.
    /// </remarks>
    public EventLabel AnimateLabel => Nod.AnimateLabel;

    public CMlScriptExtended Nod { get; set; } = null!;

    public virtual void Entry()
    {
    }

    public void GenerateOthers(ManiaScriptGenerator gen, HashSet<object> generatedObjects)
    {
        gen.LinkObject(Nod, new IScriptValue.Variable<CMlScript>("This", Nod));
        
        void Recursive(CMlFrame frame)
        {
            foreach (var child in frame.Children)
            {
                if (generatedObjects.Contains(child))
                    continue;
             
                if (child is CMlFrame other)
                    Recursive(other);

                if (child is IManiaScriptEntry entry)
                {
                    entry.Generate(gen, generatedObjects);
                }

                generatedObjects.Add(child);
            }
        }
        
        Recursive(this);
    }
}
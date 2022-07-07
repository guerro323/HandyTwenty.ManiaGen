using ManiaGen.Generator.Flow;
using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen.HTManialink;

public class CMlScriptExtended : CMlScript
{
    public readonly EventLabel PreMainLabel = new();
    public readonly EventLabel MainLabel = new();
    public readonly EventLabel PreUpdateLabel = new();
    public readonly EventLabel<CMlScriptEvent> PendingEventLabel = new();
    public readonly EventLabel UpdateLabel = new();
    public readonly EventLabel AnimateLabel = new();
}
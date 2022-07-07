using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlScript : CNod
{
    [ManiaScriptApi(typeof(Api.Properties.PendingEvents))]
    public CMlScriptEvent[] PendingEvents { get; }
}
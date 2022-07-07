using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlScriptEvent : CNod
{
    [ManiaScriptApi(typeof(Api.Types.EventType))]
    public enum EType
    {
        KeyPress,
        MouseClick
    }

    [ManiaScriptApi(typeof(CMlScriptEvent.Api.Type))] public EType Type { get; }

    public CMlScriptEvent(EType type)
    {
        Type = type;
    }
}
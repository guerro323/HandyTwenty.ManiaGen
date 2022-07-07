using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen;

public partial class Button : MlComponent
{
    public readonly EventLabel OnClickLabel = new();
    
    public readonly CMlLabel Label;
    public readonly CMlQuad Quad;
    
    public Button(string text)
    {
        Children = new CMlControl[]
        {
            Label = new CMlLabel(text)
            {
                HAlign = HorizontalAlignment.Center,
                VAlign = VerticalAlignment.Center
            },
            Quad = new CMlQuad
            {
                EnableScriptEvents = true,
                Size = Size,
                HAlign = HorizontalAlignment.Center,
                VAlign = VerticalAlignment.Center
            }
        };
    }
    
    public override void Entry()
    {
        base.Entry();
        
        PendingEventLabel.Subscribe(OnClick);
    }

    [ManiaScriptMethod]
    private void OnClick(CMlScriptEvent ev)
    {
        if (ev.Type == CMlScriptEvent.EType.MouseClick)
            OnClickLabel.Invoke();
    }
}
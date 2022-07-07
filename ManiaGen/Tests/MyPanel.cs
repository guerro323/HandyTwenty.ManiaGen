using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen;

public partial class MyPanel : MlComponent
{
    private readonly CMlLabel _label;
    private readonly Button _button;

    public int _counter;

    public MyPanel()
    {
        Child = new CMlControl[]
        {
            _label = new CMlLabel("Hi"),
            _button = new Button("Click \"me\"!"),
            
            new Button("Random button")
            {
                Position = new Vector2(0, -10)
            }
        };
    }

    public override void Entry()
    {
        _button.OnClickLabel.Subscribe(OnClick);
        UpdateLabel.Subscribe(OnUpdate);
    }

    [ManiaScriptMethod]
    private void OnClick()
    {
        _counter += 1;
        _button.Label.SetText("Click me more!");
        CManiaScript.log("clicked!");
    }

    [ManiaScriptMethod]
    private void OnUpdate()
    {
        if (_counter > 0)
        {
            _label.SetText($"Clicked {_counter} time(s)");
            _label.Rotation += 4 * _counter;

            _button.Position = new Vector2(0.4835f, 0);
            _button.Position.X = 4;
        }
    }
}
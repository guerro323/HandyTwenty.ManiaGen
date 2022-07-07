using System.Linq.Expressions;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;

namespace ManiaGen;

public static partial class Program
{
    public static void Main(string[] args)
    {
        var panel = new MlComposer
        {
            Children = new CMlControl[]
            {
                new MyPanel()
            }
        };
        var sb = new ManiaStringBuilder();

        var nod = new CMlScriptExtended();
        // Do a first pass first to assign control ids and the context CNod
        SetIdRecursively(panel, null, nod);

        var generator = new ManiaScriptGenerator();
        panel.Generate(generator, new());

        // should we do that before generating the MS code?
        // reason for Yes: it seems more logical to render the interface first
        // reason for No: we can keep the debugging name on variables
        SetIdRecursively(panel, generator, nod);

        Console.WriteLine(generator.ToString());

        panel.Render(sb);
        Console.WriteLine(sb.StringBuilder.ToString());
    }

    public static void SetIdRecursively(CMlControl component, ManiaScriptGenerator? generator, CMlScriptExtended script,
        string? prefix = null)
    {
        if (string.IsNullOrEmpty(prefix))
            prefix = "R";

        component.ControlId = prefix;
        if (component is IManiaScriptEntry scriptingComponent)
        {
            scriptingComponent.NodNonGeneric = script;
        }
        else if (generator?.GetLinkedValue(component) is { } value)
        {
            generator.NetMethod(typeof(EventLabelBase), "Subscribe", new Func<object>[]
            {
                () => script.PreMainLabel,
                () => generator.CreateMethod(_ =>
                {
                    generator.Assign(
                        () => value,
                        () => generator.Cast(() => generator.Method("Page.GetFirstChild", new Func<IScriptValue>[]
                        {
                            () => (IScriptValue.Text) component.ControlId
                        }, (CNod) value.Bottom()), component.GetType())
                    );
                    return IScriptValue.Void.Default;
                }, new Type[]
                {
                    typeof(IScriptValue.Void)
                })
            });
        }

        if (component is CMlFrame frame)
        {
            var i = 0;
            foreach (var child in frame.Children)
            {
                SetIdRecursively(child, generator, script, $"{prefix}_{i++}");
            }
        }
    }
}
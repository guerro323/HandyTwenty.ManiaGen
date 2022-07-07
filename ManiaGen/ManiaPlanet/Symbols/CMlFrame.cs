using System.Collections;
using System.Runtime.InteropServices;
using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public class CMlFrame : CMlControl, IEnumerable<CMlControl>
{
    protected CMlControl Child
    {
        set => Children = new[] {value};
    }

    public CMlControl[] Children = Array.Empty<CMlControl>();

    public void Add(CMlControl component)
    {
        Array.Resize(ref Children, Children.Length + 1);
        Children[^1] = component;
    }
    
    protected override void OnManialinkRender(ManiaStringBuilder sb)
    {
        sb.BeginScope();
        foreach (var child in Children)
        {
            sb.AppendLine();
            child.Render(sb);
        }
        sb.EndScope();
        sb.AppendLine();
    }

    protected override string GetControlName()
    {
        return "frame";
    }

    IEnumerator<CMlControl> IEnumerable<CMlControl>.GetEnumerator()
    {
        return MemoryMarshal.ToEnumerable<CMlControl>(Children.AsMemory()).GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return Children.GetEnumerator();
    }
}
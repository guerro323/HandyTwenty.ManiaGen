using System.Collections;
using System.Globalization;
using System.Numerics;
using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public abstract partial class CMlControl : CNod
{
    public bool EnableScriptEvents { get; set; }
    
    [ManiaScriptApi(typeof(Api.Properties.ControlId))]
    public string ControlId = string.Empty;

    [ManiaScriptApi(typeof(Api.Properties.Parent))]
    public CMlFrame Parent;

    [ManiaScriptApi(typeof(Api.Properties.Size))]
    public Vector2 Size;

    [ManiaScriptApi(typeof(Api.Properties.HorizontalAlign))]
    public HorizontalAlignment HAlign;

    [ManiaScriptApi(typeof(Api.Properties.VerticalAlign))]
    public VerticalAlignment VAlign;

    [ManiaScriptApi(typeof(Api.Properties.Visible))]
    public bool Visible = true;

    [ManiaScriptApi(typeof(Api.Properties.RelativePosition_V3))]
    public Vector2 Position;

    [ManiaScriptApi(typeof(Api.Properties.ZIndex))]
    public float Depth;

    [ManiaScriptApi(typeof(Api.Properties.RelativeScale))]
    public float Scale = 1.0f;

    [ManiaScriptApi(typeof(Api.Properties.RelativeRotation))]
    public float Rotation;

    [ManiaScriptApi(typeof(Api.Properties.AbsolutePosition_V3))]
    public readonly Vector2 AbsolutePosition;

    [ManiaScriptApi(typeof(Api.Properties.AbsoluteScale))]
    public readonly float AbsoluteScale;

    [ManiaScriptApi(typeof(Api.Properties.AbsoluteRotation))]
    public readonly float AbsoluteRotation;

    [ManiaScriptApi(typeof(Api.Properties.IsFocused))]
    public readonly bool IsFocused;
    
    protected abstract string GetControlName();
    protected abstract void OnManialinkRender(ManiaStringBuilder maniaStringBuilder);

    public virtual void Render(ManiaStringBuilder builder)
    {
        var sb = builder.StringBuilder;
        
        sb.Append($"<{GetControlName()} id='{ControlId}'");

        if (EnableScriptEvents)
        {
            sb.Append(" scriptevents=\"1\"");
        }
        
        if (HAlign != HorizontalAlignment.None)
        {
            sb.Append($" halign=\"{Enum.GetName(HAlign)}\"");
        }

        if (VAlign != VerticalAlignment.None)
        {
            sb.Append($" valign=\"{Enum.GetName(VAlign)}\"");
        }
        
        if (Depth != 0)
        {
            sb.Append($" z-index=\"{Depth}\"");
        }

        if (Position != default)
        {
            sb.Append($" pos=\"{Position.X.ToString(CultureInfo.InvariantCulture)} {Position.Y.ToString(CultureInfo.InvariantCulture)}\"");
        }
        
        if (Rotation != 0.0)
        {
            sb.Append($" rot=\"{Rotation.ToString(CultureInfo.InvariantCulture)}\"");
        }
        
        if (Scale != 1.0)
        {
            sb.Append($" scale=\"{Scale.ToString(CultureInfo.InvariantCulture)}\"");
        }
        
        if (Size != default)
        {
            sb.Append($" size=\"{Size.X.ToString(CultureInfo.InvariantCulture)} {Size.Y.ToString(CultureInfo.InvariantCulture)}\"");
        }

        if (!Visible)
        {
            sb.Append($" visible=\"0\"");
        }

        if (this is IEnumerable)
            sb.Append('>');

        OnManialinkRender(builder);

        if (this is IEnumerable)
            sb.Append($"</{GetControlName()}>");
        else
            sb.Append("/>");
    }

    public static implicit operator CMlControl(CMlControl[] controls)
    {
        return new CMlFrame {Children = controls};
    }
}
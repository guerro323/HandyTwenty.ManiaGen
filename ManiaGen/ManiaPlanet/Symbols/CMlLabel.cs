using System.Globalization;
using System.Numerics;
using System.Security;
using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlLabel : CMlControl,
    ICMlControlStyleFeature,
    ICMlControlOpacityFeature
{
    [ManiaScriptApi(typeof(Api.Properties.Value))]
    public string Text { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.TextFont))]
    public string TextFont { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.ValueLineCount))]
    public int ValueLineCount { get; }

    [ManiaScriptApi(typeof(Api.Properties.MaxLine))]
    public int MaxLine { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.LineSpacing))]
    public float LineSpacing { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.ItalicSlope))]
    public float ItalicSlope { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.AppendEllipsis))]
    public bool AppendEllipsis { get; set; } = true;

    [ManiaScriptApi(typeof(Api.Properties.AutoNewLine))]
    public bool AutoNewLine { get; set; } = true;
 
    [ManiaScriptApi(typeof(Api.Properties.TextPrefix))]
    public string TextPrefix { get; set; }

    [ManiaScriptApi(typeof(Api.Properties.TextColor))]
    public Vector3 TextColor;

    [ManiaScriptApi(typeof(Api.Properties.TextSizeReal))]
    public float TextSize { get; set; }

    public CMlLabel(string text)
    {
        Text = text;
    }

    [ManiaScriptApi(typeof(Api.Functions.SetText))]
    public void SetText(string text)
    {
        Text = text;
    }

    protected override void OnManialinkRender(ManiaStringBuilder builder)
    {
        if (!string.IsNullOrEmpty(Text))
            builder.AppendXml("value", SecurityElement.Escape(Text));
        if (!string.IsNullOrEmpty(TextFont))
            builder.AppendXml("textfont", TextFont);
        if (!string.IsNullOrEmpty(TextPrefix))
            builder.AppendXml("textprefix", SecurityElement.Escape(TextPrefix));
        if (TextSize != 0)
            builder.AppendXml("textsize", TextSize.ToString(CultureInfo.InvariantCulture));
        if (TextColor != default)
            builder.AppendXml("textcolor",
                $"{TextColor.X.ToString(CultureInfo.InvariantCulture)} {TextColor.Y.ToString(CultureInfo.InvariantCulture)} {TextColor.Z.ToString(CultureInfo.InvariantCulture)}");
        
        if (MaxLine != 0)
            builder.AppendXml("maxline", MaxLine.ToString());
        if (LineSpacing != 0)
            builder.AppendXml("linespacing", LineSpacing.ToString(CultureInfo.InvariantCulture));
        if (ItalicSlope != 0)
            builder.AppendXml("italicslope", ItalicSlope.ToString(CultureInfo.InvariantCulture));
        
        if (!AppendEllipsis)
            builder.AppendXml("appendellipsis", "0");
        if (!AutoNewLine)
            builder.AppendXml("autonewline", "0");
        
        if (!string.IsNullOrEmpty(Style))
            builder.AppendXml("style", Style);
        if (!string.IsNullOrEmpty(Substyle))
            builder.AppendXml("substyle", Substyle);
        if (Opacity != 0)
            builder.AppendXml("opacity", Opacity.ToString(CultureInfo.InvariantCulture));
    }

    protected override string GetControlName()
    {
        return "label";
    }

    [ManiaScriptApi(typeof(ICMlControlStyleFeature.Api.Style))]
    public string Style { get; set; }

    [ManiaScriptApi(typeof(ICMlControlStyleFeature.Api.Substyle))]
    public string Substyle { get; set; }

    [ManiaScriptApi(typeof(ICMlControlOpacityFeature.Api.Opacity))]
    public float Opacity { get; set; }
}
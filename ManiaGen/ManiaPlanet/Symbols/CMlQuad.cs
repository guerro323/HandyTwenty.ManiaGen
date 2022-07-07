using System.Globalization;
using System.Numerics;
using System.Security;
using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlQuad : CMlControl,
    ICMlControlStyleFeature,
    ICMlControlOpacityFeature
{
    [ManiaScriptApi(typeof(Api.Properties.ImageUrl))]
    public string ImageUrl;

    [ManiaScriptApi(typeof(Api.Properties.ImageUrlFocus))]
    public string ImageUrlFocus;

    [ManiaScriptApi(typeof(Api.Properties.AlphaMaskUrl))]
    public string AlphaMaskUrl;

    [ManiaScriptApi(typeof(Api.Properties.StyleSelected))]
    public bool StyleSelected;

    [ManiaScriptApi(typeof(Api.Properties.DownloadInProgress))]
    public bool DownloadInProgress;

    [ManiaScriptApi(typeof(Api.Properties.SuperSample))]
    public bool SuperSample;

    [ManiaScriptApi(typeof(Api.Properties.Colorize))]
    public Vector3 Colorize;

    [ManiaScriptApi(typeof(Api.Properties.ModulateColor))]
    public Vector3 ModulateColor;

    [ManiaScriptApi(typeof(Api.Properties.BgColor))]
    public Vector3 BgColor;

    [ManiaScriptApi(typeof(Api.Properties.BgColorFocus))]
    public Vector3 BgColorFocus;

    protected override void OnManialinkRender(ManiaStringBuilder builder)
    {
        if (!string.IsNullOrEmpty(ImageUrl))
            builder.AppendXml("image", SecurityElement.Escape(ImageUrl));
        if (!string.IsNullOrEmpty(ImageUrlFocus))
            builder.AppendXml("imagefocus", SecurityElement.Escape(ImageUrlFocus));
        if (!string.IsNullOrEmpty(AlphaMaskUrl))
            builder.AppendXml("alphamask", SecurityElement.Escape(AlphaMaskUrl));
        if (!string.IsNullOrEmpty(Style))
            builder.AppendXml("style", Style);
        if (!string.IsNullOrEmpty(Substyle))
            builder.AppendXml("substyle", Substyle);
        if (StyleSelected)
            builder.AppendXml("styleselected", (StyleSelected ? 1 : 0).ToString());
        if (Colorize != default)
            builder.AppendXml("colorize", $"{Colorize.X.ToString(CultureInfo.InvariantCulture)} {Colorize.Y.ToString(CultureInfo.InvariantCulture)} {Colorize.Z.ToString(CultureInfo.InvariantCulture)}");
        if (ModulateColor != default)
            builder.AppendXml("modulate", $"{ModulateColor.X.ToString(CultureInfo.InvariantCulture)} {ModulateColor.Y.ToString(CultureInfo.InvariantCulture)} {ModulateColor.Z.ToString(CultureInfo.InvariantCulture)}");
        if (BgColor != default)
            builder.AppendXml("bgcolor", $"{BgColor.X.ToString(CultureInfo.InvariantCulture)} {BgColor.Y.ToString(CultureInfo.InvariantCulture)} {BgColor.Z.ToString(CultureInfo.InvariantCulture)}");
        if (BgColorFocus != default)
            builder.AppendXml("bgcolorfocus", $"{BgColorFocus.X.ToString(CultureInfo.InvariantCulture)} {BgColorFocus.Y.ToString(CultureInfo.InvariantCulture)} {BgColorFocus.Z.ToString(CultureInfo.InvariantCulture)}");
        if (Opacity != 0)
            builder.AppendXml("opacity", Opacity.ToString(CultureInfo.InvariantCulture));
    }

    protected override string GetControlName()
    {
        return "quad";
    }

    [ManiaScriptApi(typeof(ICMlControlStyleFeature.Api.Style))]
    public string Style { get; set; }

    [ManiaScriptApi(typeof(ICMlControlStyleFeature.Api.Substyle))]
    public string Substyle { get; set; }

    [ManiaScriptApi(typeof(ICMlControlOpacityFeature.Api.Opacity))]
    public float Opacity { get; set; }
}
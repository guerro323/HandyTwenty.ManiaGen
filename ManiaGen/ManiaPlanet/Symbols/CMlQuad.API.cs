using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

using static IScriptValue;

public partial class CMlQuad
{
    public new class Api
    {
        public static class Properties
        {
            public class ImageUrl : IApiProperty<CMlQuad, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlQuad>(variable), nameof(ImageUrl));
                }
            }

            public class ImageUrlFocus : IApiProperty<CMlQuad, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlQuad>(variable), nameof(ImageUrlFocus));
                }
            }

            public class AlphaMaskUrl : IApiProperty<CMlQuad, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlQuad>(variable), nameof(AlphaMaskUrl));
                }
            }

            public class StyleSelected : IApiProperty<CMlQuad, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlQuad>(variable), nameof(StyleSelected));
                }
            } 

            public class DownloadInProgress : IApiProperty<CMlQuad, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlQuad>(variable), nameof(DownloadInProgress));
                }
            }

            public class SuperSample : IApiProperty<CMlQuad, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlQuad>(variable), nameof(SuperSample));
                }
            }

            public class Colorize : IApiProperty<CMlQuad, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlQuad>(variable), nameof(Colorize));
                }
            }

            public class ModulateColor : IApiProperty<CMlQuad, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlQuad>(variable), nameof(ModulateColor));
                }
            }

            public class BgColor : IApiProperty<CMlQuad, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlQuad>(variable), nameof(BgColor));
                }
            }

            public class BgColorFocus : IApiProperty<CMlQuad, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlQuad>(variable), nameof(BgColorFocus));
                }
            }
        }

        public static class Functions
        {
            public sealed class RefreshImages : IApiFunc<CMlQuad, Void>, IDynamicApiFunc
            {
                public static Void Call(ManiaScriptGenerator generator, Func<Variable<CMlQuad>> inst)
                {
                    return generator.Method(
                        $"{((Variable<CMlQuad>) generator.Compile(inst).value).Name}.RefreshImages",
                        Array.Empty<Func<IScriptValue>>()
                    );
                }
            }
        }
    }
}
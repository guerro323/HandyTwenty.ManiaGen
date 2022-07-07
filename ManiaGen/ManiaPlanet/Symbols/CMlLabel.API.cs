using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

using static IScriptValue;

public partial class CMlLabel
{
    public new class Api
    {
        public static class Properties
        {
            public class Value : IApiProperty<CMlLabel, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlLabel>(variable), nameof(Value));
                }
            }

            public class TextFont : IApiProperty<CMlLabel, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlLabel>(variable), nameof(TextFont));
                }
            }

            public class ValueLineCount : IApiProperty<CMlLabel, Integer>
            {
                public static Variable<Integer> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Integer>(Ref<CMlLabel>(variable), nameof(ValueLineCount));
                }
            }

            public class MaxLine : IApiProperty<CMlLabel, Integer>
            {
                public static Variable<Integer> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Integer>(Ref<CMlLabel>(variable), nameof(MaxLine));
                }
            }

            public class LineSpacing : IApiProperty<CMlLabel, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlLabel>(variable), nameof(LineSpacing));
                }
            }

            public class ItalicSlope : IApiProperty<CMlLabel, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlLabel>(variable), nameof(ItalicSlope));
                }
            }

            public class AppendEllipsis : IApiProperty<CMlLabel, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlLabel>(variable), nameof(AppendEllipsis));
                }
            }

            public class AutoNewLine : IApiProperty<CMlLabel, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlLabel>(variable), nameof(AutoNewLine));
                }
            }

            public class TextPrefix : IApiProperty<CMlLabel, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlLabel>(variable), nameof(TextPrefix));
                }
            }

            public class TextColor : IApiProperty<CMlLabel, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlLabel>(variable), nameof(TextColor));
                }
            }

            public class TextSizeReal : IApiProperty<CMlLabel, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlLabel>(variable), nameof(TextSizeReal));
                }
            } 
        }

        public static class Functions
        {
            public sealed class SetText : IApiFunc<CMlLabel, Text, Void>, IDynamicApiFunc
            {
                public static Void Call(ManiaScriptGenerator generator, Func<Variable<CMlLabel>> inst,
                    Func<Variable<Text>> text)
                {
                    return generator.Method($"{((Variable<CMlLabel>) generator.Compile(inst).value).Name}.SetText",
                        new Func<IScriptValue>[]
                        {
                            text
                        });
                }
            }
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using ManiaGen.Generator;

// ReSharper disable InconsistentNaming

namespace ManiaGen.ManiaPlanet.Symbols;

using static IScriptValue;

public partial class CMlControl
{
    [ManiaScriptApi(typeof(Api.Types.AlignHorizontal))]
    public enum HorizontalAlignment
    {
        None,
        Left,
        Center,
        Right,
    }

    [ManiaScriptApi(typeof(Api.Types.AlignVertical))]
    public enum VerticalAlignment
    {
        None,
        Top,
        Center,
        Center2,
        Bottom
    }

    public static class Api
    {
        public static class Properties
        {
            public class Parent : IApiProperty<CMlControl, CMlFrame>
            {
                public static Variable<CMlFrame> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<CMlFrame>(Ref<CMlControl>(variable), nameof(Parent));
                }
            }

            public class ControlId : IApiProperty<CMlControl, Text>
            {
                public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Text>(Ref<CMlControl>(variable), nameof(ControlId));
                }
            }

            public class Size : IApiProperty<CMlControl, Vec2>
            {
                public static Variable<Vec2> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec2>(Ref<CMlControl>(variable), nameof(Size));
                }
            }

            public class HorizontalAlign : IApiProperty<CMlControl, Types.AlignHorizontal>
            {
                public static Variable<Types.AlignHorizontal> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Types.AlignHorizontal>(Ref<CMlControl>(variable), nameof(HorizontalAlign));
                }
            }

            public class VerticalAlign : IApiProperty<CMlControl, Types.AlignVertical>
            {
                public static Variable<Types.AlignVertical> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Types.AlignVertical>(Ref<CMlControl>(variable), nameof(VerticalAlign));
                }
            }

            public class Visible : IApiProperty<CMlControl, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlControl>(variable), nameof(Visible));
                }
            }

            public class RelativePosition_V3 : IApiProperty<CMlControl, Vec3>
            {
                public static Variable<Vec3> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec3>(Ref<CMlControl>(variable), nameof(RelativePosition_V3));
                }
            }

            public class ZIndex : IApiProperty<CMlControl, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlControl>(variable), nameof(ZIndex));
                }
            }

            public class RelativeScale : IApiProperty<CMlControl, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlControl>(variable), nameof(RelativeScale));
                }
            }

            public class RelativeRotation : IApiProperty<CMlControl, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlControl>(variable), nameof(RelativeRotation));
                }
            }

            public class AbsolutePosition_V3 : IApiProperty<CMlControl, Vec2>
            {
                public static Variable<Vec2> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Vec2>(Ref<CMlControl>(variable), nameof(AbsolutePosition_V3));
                }
            }

            public class AbsoluteScale : IApiProperty<CMlControl, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlControl>(variable), nameof(AbsoluteScale));
                }
            }

            public class AbsoluteRotation : IApiProperty<CMlControl, Real>
            {
                public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Real>(Ref<CMlControl>(variable), nameof(AbsoluteRotation));
                }
            }

            public class IsFocused : IApiProperty<CMlControl, Boolean>
            {
                public static Variable<Boolean> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property<Boolean>(Ref<CMlControl>(variable), nameof(IsFocused));
                }
            }
        }

        public static class Functions
        {
            public class Show : IApiFunc<CMlControl, Void>, IDynamicApiFunc
            {
                public static Void Call(ManiaScriptGenerator generator, Func<Variable<CMlControl>> arg1)
                {
                    return generator.Method(
                        $"{((Variable<CMlLabel>) generator.Compile(arg1).value).Name}.{nameof(Show)}",
                        Array.Empty<Func<IScriptValue>>()
                    );
                }
            }

            public class Hide : IApiFunc<CMlControl, Void>, IDynamicApiFunc
            {
                public static Void Call(ManiaScriptGenerator generator, Func<Variable<CMlControl>> arg1)
                {
                    return generator.Method(
                        $"{((Variable<CMlLabel>) generator.Compile(arg1).value).Name}.{nameof(Hide)}",
                        Array.Empty<Func<IScriptValue>>()
                    );
                }
            }
        }

        public static class Types
        {
            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
            public class AlignHorizontal : IScriptEnumValue<AlignHorizontal, HorizontalAlignment>
            {
                public static string TypeRepresentation => "CMlControl::AlignHorizontal";
                public static IScriptValue Default => (AlignHorizontal) default(HorizontalAlignment);
                public bool IsConstant { get; set; }

                public static string ConvertToManiaScriptEquivalent(HorizontalAlignment value)
                {
                    return value switch
                    {
                        HorizontalAlignment.None => "None",
                        HorizontalAlignment.Left => "Left",
                        HorizontalAlignment.Center => "HCenter",
                        HorizontalAlignment.Right => "Right",
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                }

                public HorizontalAlignment Value { get; set; }

                public static implicit operator AlignHorizontal(HorizontalAlignment value) => new()
                {
                    Value = value,
                    IsConstant = true
                };
            }

            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
            public class AlignVertical : IScriptEnumValue<AlignVertical, VerticalAlignment>
            {
                public static string TypeRepresentation => "CMlControl::AlignVertical";
                public static IScriptValue Default => (AlignVertical) default(VerticalAlignment);
                public bool IsConstant { get; set; }

                public static string ConvertToManiaScriptEquivalent(VerticalAlignment value)
                {
                    return value switch
                    {
                        VerticalAlignment.None => "None",
                        VerticalAlignment.Top => "Top",
                        VerticalAlignment.Center => "VCenter",
                        VerticalAlignment.Center2 => "VCenter2",
                        VerticalAlignment.Bottom => "Bottom",
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                }

                public VerticalAlignment Value { get; set; }

                public static implicit operator AlignVertical(VerticalAlignment value) => new()
                {
                    Value = value,
                    IsConstant = true
                };
            }
        }
    }
}
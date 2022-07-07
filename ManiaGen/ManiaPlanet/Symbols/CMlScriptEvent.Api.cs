using System.Diagnostics.CodeAnalysis;
using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlScriptEvent
{
    public static class Api
    {
        public class Type : IApiProperty<CMlScriptEvent, Types.EventType>
        {
            public static IScriptValue.Variable<Types.EventType> Get(ManiaScriptGenerator generator,
                IScriptValue variable)
            {
                return generator.Property<Types.EventType>((IScriptValue.IVariable) variable, "Type");
            }
        }

        public static class Types
        {
            [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
            public class EventType : IScriptEnumValue<EventType, EType>
            {
                public static string TypeRepresentation => "CMlScriptEvent::Type";
                public static IScriptValue Default => (EventType) default(EType);
                public bool IsConstant { get; set; }

                public static string ConvertToManiaScriptEquivalent(EType value)
                {
                    return value switch
                    {
                        EType.KeyPress => "KeyPress",
                        EType.MouseClick => "MouseClick",
                        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                    };
                }

                public EType Value { get; set; }

                public static implicit operator EventType(EType value) => new()
                {
                    Value = value,
                    IsConstant = true
                };
            }
        }
    }
}
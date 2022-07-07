using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

using static IScriptValue;

public interface ICMlControlStyleFeature
{
    string Style { get; set; }

    // ReSharper disable IdentifierTypo
    string Substyle { get; set; }
    // ReSharper restore IdentifierTypo

    public static class Api
    {
        public class Style : IApiProperty<CMlControl, Text>
        {
            public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
            {
                return generator.Property<Text>(Ref<CMlControl>(variable), nameof(Style));
            }
        }

        // ReSharper disable IdentifierTypo
        public class Substyle : IApiProperty<CMlControl, Text>
        {
            // ReSharper restore IdentifierTypo

            public static Variable<Text> Get(ManiaScriptGenerator generator, IScriptValue variable)
            {
                return generator.Property<Text>(Ref<CMlControl>(variable), nameof(Substyle));
            }
        }
    }
}
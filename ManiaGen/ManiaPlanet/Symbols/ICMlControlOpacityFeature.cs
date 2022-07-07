using ManiaGen.Generator;

namespace ManiaGen.ManiaPlanet.Symbols;

using static IScriptValue;

public interface ICMlControlOpacityFeature
{
    float Opacity { get; set; }

    public static class Api
    {
        public class Opacity : IApiProperty<CMlControl, Real>
        {
            public static Variable<Real> Get(ManiaScriptGenerator generator, IScriptValue variable)
            {
                return generator.Property<Real>(Ref<CMlControl>(variable), nameof(Opacity));
            }
        }
    }
}
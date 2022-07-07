using ManiaGen.Generator;
using static ManiaGen.ManiaPlanet.IScriptValue;

namespace ManiaGen.ManiaPlanet.Symbols;

public partial class CMlScript
{
    public static class Api
    {
        public static class Properties
        {
            public class PendingEvents : IApiProperty<CMlScript, ValueArray<CMlScriptEvent>>
            {
                public static Variable<ValueArray<CMlScriptEvent>> Get(ManiaScriptGenerator generator, IScriptValue variable)
                {
                    return generator.Property(
                        Ref<CMlScript>(variable),
                        nameof(PendingEvents),
                        new ValueArray<CMlScriptEvent>()
                    );
                }
            } 
        }
    }
}
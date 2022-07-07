using System.Numerics;
using ManiaGen.Generator.Flow;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Libs;

namespace ManiaGen.Generator;

using static IScriptValue;

public partial class ManiaScriptGenerator
{
    private void GenerateNetMethodMapping()
    {
        void GenerateTypeObject()
        {
            CreateNetMethodMapping(typeof(object), "ToString", args =>
            {
                var lib = RequireLib<MsTextLib>();
                return Return(() => Method($"{lib.Name}::ToText", new Func<IScriptValue>[]
                {
                    () => args[0]
                }, new Text(args[0].Bottom().ToString()!)
                {
                    IsConstant = args[0].IsConstant
                }));
            }, new Type[]
            {
                typeof(Text),
                typeof(IScriptValue)
            }, embed: true);
        }

        void GenerateLabels()
        {
            CreateNetMethodMapping(typeof(EventLabelBase), "Invoke", args =>
            {
                var label = (EventLabelBase) ((NetObject) args[0].Bottom()).Value;
                if (GetLinkedValue(label) is not ScriptLabel { } val)
                {
                    val = (ScriptLabel) LinkObject(label, CreateLabel());
                }

                label.Arguments.Clear();
                foreach (var arg in args[1..])
                    label.Arguments.Add(arg);

                (val, var existed) = GetOrCreateLabelFromName(val.Name + "_" + label.Subnesting++);
                if (!existed)
                {
                    foreach (var subscriber in label.Subscribers)
                    {
                        subscriber(val);
                    }

                    label.CreatedLabels.Add(val);
                }

                CallLabel(val);

                return Return(() => Void.Default);
            }, new[]
            {
                typeof(Void)
            }, forceInline: true);

            CreateNetMethodMapping(typeof(EventLabelBase), "Subscribe", args =>
            {
                var label = (EventLabelBase) ((NetObject) args[0].Bottom()).Value;
                var method = (ScriptMethod) args[1];

                var action = (ScriptLabel scriptLabel) =>
                {
                    Label(scriptLabel, () =>
                    {
                        Method(
                            method,
                            label
                                .Arguments
                                .Select(t => new Func<IScriptValue>(() => t))
                                .ToArray()
                        );
                    });
                };
                label.Subscribers.Add(action);
                foreach (var l in label.CreatedLabels)
                {
                    action(l);
                }

                return Return(() => Void.Default);
            }, new[]
            {
                typeof(Void)
            }, forceInline: true);
        }

        void GenerateVectors()
        {
            float GetFloat(IScriptValue value)
            {
                if (value.Bottom() is Integer i)
                    return i.Value;
                return ((Real) value.Bottom()).Value;
            }

            CreateNetMethodMapping(typeof(Vector2), ".ctor", args =>
            {
                var x = GetFloat(args[0]);
                var y = GetFloat(args[1]);
                
                return Return(() => Ref(new Vec2(new Vector2(x, y))));
            }, new Type[]
            {
                typeof(Vec2),
                typeof(Real),
                typeof(Real),
            }, embed: true);
            
            CreateNetMethodMapping(typeof(Vector3), ".ctor", args =>
            {
                var x = GetFloat(args[0]);
                var y = GetFloat(args[1]);
                var z = GetFloat(args[1]);
                
                return Return(() => new Vec3(new Vector3(x, y, z)));
            }, new Type[]
            {
                typeof(Vec2),
                typeof(Real),
                typeof(Real),
                typeof(Real),
            }, embed: true);

            for (var i = 0; i < 3; i++)
            {
                var name = i switch
                {
                    0 => "X",
                    1 => "Y",
                    2 => "Z"
                };

                for (var y = 0; y < 2; y++)
                {
                    var type = y == 0 ? typeof(Vector2) : typeof(Vector3);
                    CreateNetMethodMapping(type, name, args =>
                    {
                        return Return(() => Property((IVariable) args[0], name, (Real) Real.Default));
                    }, new Type[]
                    {
                        typeof(Real)
                    }, embed: true);
                }
            }
        }

        GenerateTypeObject();
        GenerateLabels();
        GenerateVectors();
    }
}
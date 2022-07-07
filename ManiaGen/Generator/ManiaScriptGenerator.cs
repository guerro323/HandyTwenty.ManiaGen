using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ManiaGen.Generator.Statements;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator;

using static IScriptValue;

public delegate void ManiaScriptLambda(ManiaScriptGenerator gen);

public partial class ManiaScriptGenerator
{
    public BodyStatement Root { get; set; }

    public ManiaScriptGenerator(CollectionStatement? root = null)
    {
        if (root == null)
        {
            root = new ScriptStatement(
                new List<ManiaScriptStatement>(),
                new List<ManiaScriptStatement>(),
                new List<ManiaScriptStatement>(),
                new List<ManiaScriptStatement>()
            );
        }

        Root = root;

        GenerateNetMethodMapping();
    }

    private Dictionary<string, Dictionary<Type, ScriptMethod>> _netMethodMap = new();

    public void CreateNetMethodMapping(Type type, string name, CustomMethod action, Type[] argsType,
        bool forceInline = false, bool disableInlining = false,
        bool embed = false,
        bool allowGeneric = true)
    {
        ref var map = ref CollectionsMarshal.GetValueRefOrAddDefault(_netMethodMap, name, out var exists)!;
        if (!exists)
            map = new Dictionary<Type, ScriptMethod>();

        map[type] = CreateMethod(action, argsType, forceInline, true, embed, allowGeneric);
    }

    public void CreateNetMethodMapping(string name, CustomMethod action, Type[] argsType,
        bool forceInline = false, bool disableInlining = false,
        bool allowGeneric = true)
    {
        CreateNetMethodMapping(typeof(object), name, action, argsType, forceInline, disableInlining, allowGeneric);
    }

    public IScriptValue NetMethod(Type type, string name, Func<object>[] args)
    {
        if (!_netMethodMap.TryGetValue(name, out var map))
            throw new InvalidOperationException($"No .NET Method found for type={type} name='{name}' (name not found)");

        var lastType = default(Type);
        foreach (var (kvpType, kvpAction) in map)
        {
            if (!kvpType.IsAssignableFrom(type))
                continue;
            if (kvpType == type)
            {
                lastType = type;
                break;
            }

            // A -> B -> C
            // B.IsAssignableFrom(C) true
            // A.IsAssignableFrom(B) false
            // AKA get the nearest type
            if (lastType == null || lastType.IsAssignableFrom(kvpType))
                lastType = kvpType;
        }
        
        if (lastType == null)
            throw new InvalidOperationException($"No .NET Method found for type={type} name='{name}' (type not mapped)");

        var objToValueArgs = new List<Func<IScriptValue>>();
        foreach (var arg in args)
        {
            objToValueArgs.Add(() =>
            {
                var compiled = arg();
                if (compiled is IScriptValue val) return val;
                return new NetObject(compiled);
            });
        }
        
        return Method(map[lastType], objToValueArgs.ToArray());
    }

    private Dictionary<object, IScriptValue> _linkForward = new(new LinkComparer());

    private class LinkComparer : IEqualityComparer<object?>
    {
        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        public bool Equals(object? x, object? y)
        {
            if (x is LinkWithName xL && y is LinkWithName yL)
                return xL.Equals(yL);

            return x!.Equals(y); // dictionary never have null keys
        }

        public int GetHashCode(object obj)
        {
            return obj is LinkWithName l ? l.GetHashCode() : obj.GetHashCode();
        }
    }
    
    public record class LinkWithName(object Holder, string Name);

    public IScriptValue LinkObject(object obj, IScriptValue script)
    {
        if (obj == null)
            return script;
        
        _linkForward[obj] = script;
        return script;
    }

    public IScriptValue? GetLinkedValue(object obj)
    {
        if (obj == null)
            return null;
        
        return _linkForward.GetValueOrDefault(obj);
    }

    private Dictionary<string, (ScriptLabel label, LabelStatement statement)> _labelInfoMap = new();

    private int _labelCount;

    private (ScriptLabel label, bool existed) GetOrCreateLabelFromName(string name)
    {
        if (_labelInfoMap.TryGetValue(name, out var details))
            return (details.label, true);

        details.label = new ScriptLabel(name);
        details.statement = new LabelStatement(name, new List<ManiaScriptStatement>());
        _labelInfoMap[name] = details;
        
        return (details.label, false);
    }

    public ScriptLabel CreateLabel()
    {
        var labelName = $"l_{_labelCount++}";
        return GetOrCreateLabelFromName(labelName).label;
    }
    
    public void Label(ScriptLabel label, Action then)
    {
        // The user can feed us an existing label from .CreateLabel()
        // but also can give a '(ScriptLabel) "PlayLoop"' which doesn't exist yet (so create it!)
        // No worry, this will not duplicate
        label = GetOrCreateLabelFromName(label.Name).label;

        var oldRoot = Root;
        Root = _labelInfoMap[label.Name].statement;

        then();

        Root = oldRoot;
    }

    public void CallLabel(ScriptLabel label, bool latestExtensionOnly = false)
    {
        Root.Statements.Add(new CallLabelStatement(label.Name, latestExtensionOnly));
    }

    private Dictionary<string, string> _libSet = new();

    public IVariable RequireLib<T>()
        where T : IManiaScriptLib
    {
        var libShortcut = $"lib_{T.Path}";
        _libSet[T.Path] = libShortcut;
        
        return new Variable<Void>(libShortcut, null);
    }
    
    internal int _varCount;
    private int _globalVarCount;

    public Variable<T> Declare<T>(Func<T>? value = null, string? debuggingName = null)
        where T : class, IScriptValue
    {
        var varName = debuggingName == null ? $"v_{_varCount++}" : $"v_{_varCount++}_{debuggingName}";

        BodyStatement? scope = null;
        T? compiledValue = null;
        if (value != null)
        {
            var ret = Compile(value);
            compiledValue = (T) ret.value;
            scope = ret.scope with {IsList = true};
        }

        Root.Statements.Add(new DeclareStatement(varName, compiledValue.GetManiaScriptTypeName(), scope));
        return new Variable<T>(varName, compiledValue);
    }

    public IVariable Declare(Func<IScriptValue> other, string? debuggingName = null)
    {
        var varName = debuggingName == null ? $"v_{_varCount++}" : $"v_{_varCount++}_{debuggingName}";

        var ret = Compile(other);
        var compiledValue = ret.value.Bottom();
        BodyStatement scope = ret.scope with {IsList = true};

        // Make sure to get the raw value and not some sort of variable
        compiledValue = compiledValue.Bottom();

        Root.Statements.Add(new DeclareStatement(varName, compiledValue.GetManiaScriptTypeNameNonGeneric(), scope));

        return VarFrom(varName, compiledValue.Bottom());
    }

    private readonly List<IVariable> _globalVariables= new();

    public Variable<T> Global<T>(T? value = null, string? debuggingName = null)
        where T : class, IScriptValue
    {
        var varName = debuggingName == null ? $"g_{_globalVarCount++}" : $"g_{_globalVarCount++}_{debuggingName}";
        var variable = new Variable<T>(varName, value);
        _globalVariables.Add(variable);
        
        return variable;
    }

    public void Break()
    {
        Root.Statements.Add(new BreakStatement());
        throw new ControlFlowBreakException();
    }
    
    public void Continue()
    {
        Root.Statements.Add(new ContinueStatement());
        throw new ControlFlowContinueException();
    }

    public IScriptValue Return(Func<IScriptValue> value)
    {
        var compiled = Compile(value);

        Root.Statements.Add(new ReturnStatement(compiled.scope) {Creator = compiled.value});
        return compiled.value;
    }

    public (IScriptValue value, CollectionStatement scope) Compile(Func<IScriptValue> toCompile)
    {
        var oldRoot = Root;
        var scope = Root = new CollectionStatement(new List<ManiaScriptStatement>(), ForceSingleLine: true, IsList: true);
        var value = toCompile();
        Root = oldRoot;
        
        if (scope.Statements.Count == 0 && value is not IDisableValueRepresentation)
            scope.Statements.Add(value.ToStatement());
        
        return (value, (CollectionStatement) scope with
        {
            Creator = value
        });
    }
    
    public CollectionStatement Compile(Action toCompile)
    {
        var oldRoot = Root;
        var scope = Root = new CollectionStatement(new List<ManiaScriptStatement>());
        toCompile();
        Root = oldRoot;

        return (CollectionStatement) scope;
    }

    public T Method<T>(string name, Func<IScriptValue>[]? args = null, T? ret = default)
        where T : IScriptValue
    {
        args ??= Array.Empty<Func<IScriptValue>>();
        ret ??= (T) T.Default;

        var compiledArgs = new IScriptValue[args.Length];
        
        var arguments = new List<ManiaScriptStatement>();
        var oldRoot = Root;
        Root = new CollectionStatement(new List<ManiaScriptStatement>());
        for (var i = 0; i < args.Length; i++)
        {
            var original = compiledArgs[i] = args[i]();
            var statement = Root.Statements.Count == 0 ? original.ToStatement() : Root.Statements[0];
            statement.Creator ??= original;
            
            arguments.Add(statement);

            Root.Statements.Clear();
        }

        Root = oldRoot;

        var method = new ScriptMethod(name, null!, null!)
        {
            DisableInlining = true,
            runtimeReturn = ret,
            runtimeArgs = compiledArgs
        };
        Root.Statements.Add(new MethodCallStatement(this, method, arguments));
        return ret;
    }

    public Void Method(string name, Func<IScriptValue>[]? args = null)
    {
        return Method<Void>(name, args);
    }

    public IScriptValue Method(ScriptMethod method, Func<IScriptValue>[] args)
    {
        var compiledArgs = new IScriptValue[args.Length];
        
        var arguments = new List<ManiaScriptStatement>();
        var oldRoot = Root;
        Root = new CollectionStatement(new List<ManiaScriptStatement>());
        for (var i = 0; i < args.Length; i++)
        {
            var original = compiledArgs[i] = args[i]();
            var statement = Root.Statements.Count == 0 
                ? (original is IDisableValueRepresentation 
                    ? new RawStatement("") 
                    : original.ToStatement()) 
                : Root.Statements[0];
            statement.Creator ??= original;
            
            arguments.Add(statement);

            Root.Statements.Clear();
        }

        Root = oldRoot;

        var (call, map, allowGeneric) = _methodMap[method.Name];
        var types = compiledArgs
            .Select(a => a.GetType())
            .ToArray();
        types = new[] {method.Arguments[0]}.Concat(types).ToArray();

        var key = map.Keys.FirstOrDefault(k => k.AsSpan().SequenceEqual(types));
        if (key == null && (allowGeneric || map.Count == 0))
        {
            key = allowGeneric ? types : method.Arguments;

            oldRoot = Root;
            var scope = Root = new CollectionStatement(new List<ManiaScriptStatement>());
            var ret = call(compiledArgs);
            Root = oldRoot;

            var requireInline = allowGeneric && types.Any(t => t == typeof(ScriptMethod));
            map[key] = method = new ScriptMethod(method.Name, types, scope.Statements)
            {
                ForceInline = method.ForceInline || requireInline,
                DisableInlining = method.DisableInlining && !requireInline,
                IsMacro = method.IsMacro,
                
                runtimeArgs = compiledArgs,
                runtimeReturn = ret
            };
            
            map[key].Called++;

            Root.Statements.Add(new MethodCallStatement(this, method, arguments));

            return ret;
        }
        else
        {
            if (!allowGeneric) key ??= method.Arguments;
            
            map[key].Called++;
            
            // get ret value
            oldRoot = Root;
            var scope = Root = new CollectionStatement(new List<ManiaScriptStatement>());
            var ret = call(compiledArgs);
            Root = oldRoot;

            method = method with
            {
                runtimeReturn = ret,
                runtimeArgs = compiledArgs,
                Body = scope.Statements
            };

            Root.Statements.Add(new MethodCallStatement(this, method, arguments));
            
            return ret;
        }
    }

    public delegate IScriptValue CustomMethod(IScriptValue[] args);

    private int _methodCount;
    private Dictionary<string, (CustomMethod, Dictionary<Type[], ScriptMethod>, bool allowGeneric)> _methodMap = new();

    public ScriptMethod CreateMethod(CustomMethod onCall, Type[] argsType, bool forceInline = false, bool dontInline = false, bool embed = false, bool allowGeneric = false)
    {
        var name = $"m_{_methodCount++}";
        // TODO: This information should be in the code directly as verbose comments
        /*var frames = new StackTrace(true).GetFrames();
        var i = 1;
        for (; i < frames.Length; i++)
        {
            if (frames[i].GetMethod()!.Name.Contains(nameof(CreateNetMethodMapping)))
                continue;

            break;
        }
        
        var st = frames[i];
        Console.WriteLine($"'{name}' was '{onCall.Method.DeclaringType}.{onCall.Method.Name} ({st.GetFileName()} {st.GetFileLineNumber()}:{st.GetFileColumnNumber()})'");
        */
        _methodMap[name] = (onCall, new Dictionary<Type[], ScriptMethod>(), allowGeneric);
        return new ScriptMethod(name, argsType, null!)
        {
            ForceInline = forceInline,
            DisableInlining = dontInline,
            IsMacro = embed
        };
    }

    public Variable<T> Property<T>(IVariable root, string path, T? ret = null)
        where T : class, IScriptValue
    {
        return new Variable<T>($"{root.Name}.{path}", ret ?? (T) DefaultFrom(typeof(T)));
    }
    
    public IScriptValue ElementAt(Func<IScriptValue> arrayFunc, Func<IScriptValue> indexFunc)
    {
        var arrayCompile = Compile(arrayFunc);
        var indexCompile = Compile(indexFunc);
        if (arrayCompile.value.Bottom() is not IEnumerableValues array)
            throw new InvalidOperationException($"{arrayCompile.value.Bottom().GetType().Name} is not an array");

        Root.Statements.Add(new ArrayAccessStatement(arrayCompile.scope, indexCompile.scope));
        return DefaultFrom(array.ValueType);
    }

    public string ToString(bool compact)
    {
        var builder = new ManiaStringBuilder() {Compact = compact};
        try
        {
            if (Root is ScriptStatement scriptStatement)
            {
                scriptStatement.Globals.Clear();
                scriptStatement.Labels.Clear();
                scriptStatement.Methods.Clear();

                var globalVariables = new CollectionStatement(new List<ManiaScriptStatement>());
                var globalInit = new BlockStatement(new List<ManiaScriptStatement>());

                scriptStatement.Globals.AddRange(_libSet.Select((kvp, _) =>
                {
                    var (library, shortcut) = kvp;
                    return new RawStatement($"#Include \"{library}\" as {shortcut}");
                }));
                _globalVariables.ForEach(v =>
                {
                    if (v.Type.GetInterfaces().Any(i => i == typeof(IDisableAllRepresentation)))
                        return;

                    var global = new DeclareStatement(v.Name, v.RuntimeTypeRepresentation);
                    if (v.Bottom() is { } value and not IDisableValueRepresentation)
                    {
                        globalInit.Statements.Add(new AssignStatement(
                            "=",
                            new RawStatement(global.Name),
                            value.ToStatement()
                        ));
                    }

                    globalVariables.Statements.Add(global);
                });
                scriptStatement.Globals.Add(globalVariables);

                /*if (globalInit.Statements.Count > 0)
                    scriptStatement.Statements.Insert(0, globalInit);*/

                scriptStatement.Labels.AddRange(_labelInfoMap.Values.Select(v => v.statement));

                foreach (var (name, (_, symbolMap, _)) in _methodMap)
                {
                    foreach (var (types, method) in symbolMap)
                    {
                        if (method.IsInlined || method.IsMacro)
                            continue;

                        var args = new List<(string name, IScriptValue original)>();
                        if (method.runtimeReturn != null)
                            args.Add((method.runtimeReturn.GetManiaScriptTypeNameNonGeneric(), method.runtimeReturn));
                        else
                            args.Add((types[0].GetManiaScriptTypeNameNonGeneric(), null!));

                        if (method.runtimeArgs != null)
                        {
                            foreach (var runtimeArg in method.runtimeArgs)
                            {
                                args.Add((runtimeArg.GetManiaScriptTypeNameNonGeneric(), runtimeArg));
                            }
                        }
                        else
                        {
                            foreach (var type in types)
                            {
                                args.Add((type.GetManiaScriptTypeNameNonGeneric(), null!));
                            }
                        }

                        scriptStatement.Methods.Add(new MethodDeclarationStatement(
                            method.Name,
                            args.ToArray(),
                            method.Body
                        ));
                    }
                }
            }

            Root.Generate(builder);

            return builder.StringBuilder.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error, last Generator.ToString(bool):\n{builder.StringBuilder.ToString()}");
            throw;
        }
    }
    
    public override string ToString()
    {
        return ToString(false);
    }

    public class ControlFlowContinueException : SuccessfulControlFlowException
    {
        
    }

    public class ControlFlowBreakException : SuccessfulControlFlowException
    {
        
    }
    
    /// <summary>
    /// Used with <see cref="ManiaScriptGenerator.Break"/> or <see cref="ManiaScriptGenerator.Continue"/>
    /// </summary>
    public abstract class SuccessfulControlFlowException : Exception
    {
        
    }
}
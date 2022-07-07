using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using ManiaGen.Generator;
using ManiaGen.Generator.Statements;

namespace ManiaGen.ManiaPlanet;

public interface IScriptValue
{
    /// <summary>
    /// Indicate that this <see cref="IScriptValue"/> is a class
    /// </summary>
    /// <remarks>
    /// This will change any assignment (=) to reference assignment (<=>)
    /// </remarks>
    public interface IIsClass
    {
    }

    /// <summary>
    /// Indicate that this <see cref="IScriptValue"/> cannot be represented as a value.
    /// </summary>
    public interface IDisableValueRepresentation
    {
        
    }
    
    /// <summary>
    /// Indicate that this <see cref="IScriptValue"/> cannot be a variable or represented in any form.
    /// </summary>
    public interface IDisableAllRepresentation : IDisableValueRepresentation
    {
        
    }

    public record class NetObject(object Value) : IScriptValue, IDisableAllRepresentation
    {
        public static string TypeRepresentation => null;
        public static IScriptValue Default => null;

        public bool IsConstant
        {
            set { }
            // Net Object are always constant to the generator since they can't be put in the ManiaScript runtime
            get => true;
        }
        public ManiaScriptStatement ToStatement()
        {
            throw new InvalidOperationException("Cannot convert to a statement");
        }

        public bool ConstantEquals(IScriptValue other)
        {
            return other is NetObject otherI && otherI.Value == Value;
        }

        public IScriptValue Bottom()
        {
            return this;
        }
    }
    
    public static IVariable VarFrom(string name, IScriptValue obj)
    {
        var genType = typeof(Variable<>).MakeGenericType(obj.Bottom().GetType());
        foreach (var constructor in genType.GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != 2)
                continue;

            return (IVariable) constructor.Invoke(new object?[]
            {
                name,
                obj
            });
        }
        
        throw new InvalidOperationException($"No matching constructor (string, IScriptValue) found on {genType}");
    }

    public static IVariable Ref(IScriptValue obj)
    {
        var objType = (obj.Bottom() ?? obj).GetType();

        return (IVariable) typeof(Variable<>)
            .MakeGenericType(objType)
            .GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public)!
            .Invoke(null, new object?[] {obj})!;
    }

    public static Variable<T> Ref<T>(IScriptValue obj)
        where T : IScriptValue
    {
        return new Variable<T>(obj is IVariable variable ? variable.Name : "@implicit", (T) obj.Bottom())
        {
            IsReference = true
        };
    }

    public static T CastTo<T>(object obj)
        where T : IScriptValue
    {
        if (obj is not IScriptValue value)
        {
            if (typeof(T) != typeof(IScriptValue) || typeof(T) != typeof(NetObject))
                throw new InvalidOperationException($"Invalid .NETObject cast {typeof(T)}");
            
            return (T) (object) new NetObject(obj);
        }
        
        // Converting a variable to a value
        if (value is IVariable && !typeof(T).GetInterfaces().Contains(typeof(IVariable)))
        {
            return (T) value.Bottom();
        }
        
        // Converting a value to a variable
        if (value is not IVariable && typeof(T).GetInterfaces().Contains(typeof(IVariable)))
        {
            return (T) typeof(T).GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public)!
                .Invoke(null, new object?[] {value})!;
        }

        return (T) value;
    }

    public static IScriptValue DefaultFrom(Type type)
    {
        var obj = RuntimeHelpers.GetUninitializedObject(type);
        if (type.GetInterfaces().Contains(typeof(IScriptValue)))
            return (IScriptValue) obj;
        return new NetObject(obj);
    }

    public interface IEnumerableValues : IScriptValue
    {
        public Type ValueType { get; }
    }

    public record ValueArray<T> : IEnumerableValues
        where T : IScriptValue
    {
        public static string TypeRepresentation => $"{T.TypeRepresentation}[]";
        public static IScriptValue Default => (ValueArray<T>) Array.Empty<T>();
        public bool IsConstant { get; set; }
        public ManiaScriptStatement ToStatement()
        {
            return new ArrayStatement(ValueType.GetManiaScriptTypeNameNonGeneric(), Values);
        }

        public T[] Values;

        public ValueArray(T[]? values = null)
        {
            Values = values ?? Array.Empty<T>();
        }

        public bool ConstantEquals(IScriptValue other)
        {
            return other is ValueArray<T> otherI && otherI.Values.AsSpan().SequenceEqual(Values);
        }

        public IScriptValue Bottom()
        {
            return this;
        }

        public Type ValueType => typeof(T);

        public static implicit operator ValueArray<T>(T[] array)
        {
            return new ValueArray<T>(array)
            {
                IsConstant = true
            };
        }

        public record ArrayStatement() : BodyStatement(new List<ManiaScriptStatement>())
        {
            public string ValueType;
            
            public ArrayStatement(string valueType, T[] array) : this()
            {
                ValueType = valueType;
                foreach (var value in array)
                {
                    Statements.Add(value.ToStatement());
                }
            }

            public ArrayStatement(string valueType, List<ManiaScriptStatement> elements) : this()
            {
                ValueType = valueType;
                Statements.AddRange(elements);
            }

            public override void Generate(ManiaStringBuilder builder)
            {
                var sb = builder.StringBuilder;
                sb.Append(ValueType);
                sb.Append(" [");
                builder.BeginScope();
                foreach (var statement in Statements)
                    statement.Generate(builder);
                builder.EndScope();
                sb.Append(']');
            }
        }
    }
    
    public record MethodCallAccess(ScriptMethod Method, IScriptValue[] Arguments) : IScriptValue
    {
        public static string TypeRepresentation => throw new InvalidOperationException("ScriptMethod can't be represented");

        public static IScriptValue Default => throw new InvalidOperationException("ScriptMethod can't be defaulted");

        public bool IsConstant
        {
            set { }
            get => Arguments.All(a => a.IsConstant);
        }

        public ManiaScriptStatement ToStatement()
        {
            return Method.IsInlined
                ? new BlockStatement(Method.Body)
                : new MethodStatement(Method.Name, Arguments.Select(a => a.ToStatement()).ToList());
        }

        public bool ConstantEquals(IScriptValue other)
        {
            return other is MethodCallAccess { } otherM && otherM == this;
        }

        public IScriptValue Bottom()
        {
            return this;
        }
    }
    
    public record ScriptMethod(string Name, Type[] Arguments, List<ManiaScriptStatement> Body) : IScriptValue, IDisableValueRepresentation
    {
        public IScriptValue? runtimeReturn = null;
        public IScriptValue[]? runtimeArgs = null;
        
        public bool IsInlined => ForceInline || (Called <= 1 && !DisableInlining);

        public static string TypeRepresentation => string.Empty;

        public static IScriptValue Default => throw new InvalidOperationException("ScriptMethod can't be defaulted");

        public bool IsConstant
        {
            set { }
            get => true;
        }

        public int Called;

        public bool ForceInline { get; set; }
        public bool DisableInlining { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            throw new InvalidOperationException("Can't convert a method to a statement directly");
        }

        public bool ConstantEquals(IScriptValue other)
        {
            return other is ScriptMethod { } otherM && otherM == this;
        }

        public IScriptValue Bottom()
        {
            return this;
        }

        public string RuntimeTypeRepresentation
        {
            get
            {
                if (!string.IsNullOrEmpty(runtimeReturn?.RuntimeTypeRepresentation))
                    return runtimeReturn.RuntimeTypeRepresentation;

                return runtimeReturn?.GetType().GetManiaScriptTypeNameNonGeneric() ?? string.Empty;
            }
        }

        public bool IsMacro { get; set; }
    }
    
    public record ScriptLabel(string Name) : IScriptValue, IDisableValueRepresentation
    {
        public static string TypeRepresentation => throw new InvalidOperationException("ScriptLabel can't be represented");

        public static IScriptValue Default => throw new InvalidOperationException("ScriptLabel can't have be defaulted");

        public bool IsConstant
        {
            set {}
            get => true;
        }
        
        public ManiaScriptStatement ToStatement()
        {
            throw new InvalidOperationException("Can't convert a label to a statement directly");
        }

        public bool ConstantEquals(IScriptValue other)
        {
            return other is ScriptLabel otherI && otherI.Name == Name;
        }

        public IScriptValue Bottom()
        {
            return this;
        }
        
        public static implicit operator ScriptLabel(string name) => new(name);
    }

    public interface IVariable : IScriptValue
    {
        string Name { get; }
        Type Type { get; }
    }
    
    public record Variable<T>(string Name, T? Value) : IVariable
        where T : IScriptValue
    {
        /// <summary>
        /// Is this variable a reference to a value?
        /// If yes, we will directly reference the value instead of a traditional variable name
        /// </summary>
        public bool IsReference { get; set; }

        public static string TypeRepresentation => T.TypeRepresentation;
        public static IScriptValue Default => throw new InvalidOperationException("Not possible to zero a variable");
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            return IsReference
                ? Value!.ToStatement()
                : new RawStatement(Name) {Creator = this};
        }
        
        public bool ConstantEquals(IScriptValue other)
        {
            return Value != null && Value.ConstantEquals(other);
        }
        
        public IScriptValue Bottom() => Value?.Bottom()!;

        public static implicit operator Variable<T>(T value)
        {
            return new Variable<T>(value is IVariable variable ? variable.Name : "@implicit", value)
            {
                IsReference = true
            };
        }
        
        public string RuntimeTypeRepresentation
        {
            get
            {
                if (Value != null)
                    return Value.RuntimeTypeRepresentation;
                if (!string.IsNullOrEmpty(T.TypeRepresentation))
                    return T.TypeRepresentation;

                return typeof(T).Name;
            }
        }

        public Type Type => typeof(T);
    }
    
    public record Void : IScriptValue
    {
        public static string TypeRepresentation => "Void";
        public static IScriptValue Default => new Void {IsConstant = true};

        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            return new RawStatement(string.Empty) {Creator = this};
        }
        
        public bool ConstantEquals(IScriptValue other)
        {
            return other is Void;
        }
        
        public IScriptValue Bottom() => this;
    }
    
    public record Text(string Value) : IScriptValue, IEnumerableValues
    {
        public static string TypeRepresentation => "Text";
        public static IScriptValue Default => (Text) "";
        
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            return new RawStatement($"\"{Value.Replace("\"", "\\\"")}\"") {Creator = this};
        }

        public static implicit operator Text(string value) => new(value) {IsConstant = true};
        
        public bool ConstantEquals(IScriptValue other)
        {
            if (other is Text otherI)
                return Value == otherI.Value;
            return false;
        }
        
        public IScriptValue Bottom() => this;
        public Type ValueType => typeof(Text);
    }

    public record Integer(int Value) : IScriptValue
    {
        public static string TypeRepresentation => "Integer";
        public static IScriptValue Default => (Integer) 0;
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            return new RawStatement(Value.ToString()) {Creator = this};
        }

        public bool ConstantEquals(IScriptValue other)
        {
            if (other is Integer otherI)
                return Value == otherI.Value;
            return false;
        }
        
        public IScriptValue Bottom() => this;

        public static implicit operator Integer(int value) => new(value) {IsConstant = true};
    }
    
    public record Real(float Value) : IScriptValue
    {
        public static string TypeRepresentation => "Real";
        public static IScriptValue Default => (Real) 0.0f;
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            return new RawStatement(Value.ToString(CultureInfo.InvariantCulture)) {Creator = this};
        }

        public bool ConstantEquals(IScriptValue other)
        {
            if (other is Real otherI)
                return Value == otherI.Value;
            return false;
        }
        
        public IScriptValue Bottom() => this;

        public static implicit operator Real(float value) => new(value) {IsConstant = true};
    }
    
    public record Vec2(Vector2 Value) : IScriptValue
    {
        public static string TypeRepresentation => "Vec2";
        public static IScriptValue Default => (Vec2) default(Vector2);
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            var x = Value.X.ToString(CultureInfo.InvariantCulture);
            if (!x.Contains('.'))
                x = $"{x}.0";
            var y = Value.Y.ToString(CultureInfo.InvariantCulture);
            if (!y.Contains('.'))
                y = $"{y}.0";
            
            return new RawStatement($"<{x}, {y}>") {Creator = this};
        }

        public bool ConstantEquals(IScriptValue other)
        {
            if (other is Vec2 otherI)
                return Value == otherI.Value;
            return false;
        }
        
        public IScriptValue Bottom() => this;

        public static implicit operator Vec2(Vector2 value) => new(value) {IsConstant = true};
    }
    
    public record Vec3(Vector3 Value) : IScriptValue
    {
        public static string TypeRepresentation => "Vec3";
        public static IScriptValue Default => (Vec3) default(Vector3);
        public bool IsConstant { get; set; }

        public ManiaScriptStatement ToStatement()
        {
            var x = Value.X.ToString(CultureInfo.InvariantCulture);
            if (!x.Contains('.'))
                x = $"{x}.0";
            var y = Value.Y.ToString(CultureInfo.InvariantCulture);
            if (!y.Contains('.'))
                y = $"{y}.0";
            var z = Value.Z.ToString(CultureInfo.InvariantCulture);
            if (!z.Contains('.'))
                z = $"{z}.0";
            
            return new RawStatement($"<{x}, {y}, {z}>") {Creator = this};
        }

        public bool ConstantEquals(IScriptValue other)
        {
            if (other is Vec3 otherI)
                return Value == otherI.Value;
            return false;
        }
        
        public IScriptValue Bottom() => this;

        public static implicit operator Vec3(Vector3 value) => new(value) {IsConstant = true};
    }
    
    public record Boolean(bool Value) : IScriptValue
    {
        public static string TypeRepresentation => "Boolean";
        public static IScriptValue Default => (Boolean) false;
        public virtual bool IsConstant { get; set; }

        public virtual ManiaScriptStatement ToStatement()
        {
            return new RawStatement(Value ? "True" : "False") {Creator = this};
        }

        public bool ConstantEquals(IScriptValue other)
        {
            if (other.Bottom() is Boolean otherI)
                return Value == otherI.Value;
            return false;
        }

        public IScriptValue Bottom() => this;

        public static implicit operator Boolean(bool value) => new(value) {IsConstant = true};
    }

    static abstract string TypeRepresentation { get; }
    static abstract IScriptValue Default { get; }

    bool IsConstant { get; set; }

    ManiaScriptStatement ToStatement();
    bool ConstantEquals(IScriptValue other);

    /// <summary>
    /// Get the bottom value
    /// </summary>
    /// <returns></returns>
    IScriptValue Bottom();

    virtual string RuntimeTypeRepresentation => GetType().Name;
}

public static class ScriptValueExtension
{
    public static string GetManiaScriptTypeName<T>(this T? t)
        where T : IScriptValue
    {
        if (!string.IsNullOrEmpty(T.TypeRepresentation))
            return T.TypeRepresentation;

        t ??= (T) RuntimeHelpers.GetUninitializedObject(typeof(T));
        if (!string.IsNullOrEmpty(t.RuntimeTypeRepresentation))
            return t.RuntimeTypeRepresentation;

        throw new InvalidOperationException($"No type representation found for '{typeof(T)}'");
    }
    
    public static string GetManiaScriptTypeNameNonGeneric(this IScriptValue t)
    {
        if (t == null)
        {
            return string.Empty;
        }
        
        var method = typeof(ScriptValueExtension)
            .GetMethod(nameof(GetManiaScriptTypeName), BindingFlags.Static | BindingFlags.Public);

        return (string) method.MakeGenericMethod(t.GetType())
            .Invoke(null, new object?[] {t});
    }
    
    public static string GetManiaScriptTypeNameNonGeneric(this Type? t)
    {
        if (t == null)
        {
            return string.Empty;
        }
        
        var method = typeof(ScriptValueExtension)
            .GetMethod(nameof(GetManiaScriptTypeName), BindingFlags.Static | BindingFlags.Public);

        return (string) method.MakeGenericMethod(t)
            .Invoke(null, new object?[] {null});
    }
}
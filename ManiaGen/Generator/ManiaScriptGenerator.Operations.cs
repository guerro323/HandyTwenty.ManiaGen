using ManiaGen.Generator.Statements;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator;

using static IScriptValue;

public partial class ManiaScriptGenerator
{
    public IScriptValue Add(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("+", l.scope, r.scope));

        switch (l.value)
        {
            // Don't return integer here
            case Integer lValue when r.value is Integer rValue:
                return new Integer(lValue.Value + rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
            case Real lValue when r.value is Real rValue:
                return new Real(lValue.Value + rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
        }

        return Integer.Default;
    }
    
    public IScriptValue Multiply(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("*", l.scope, r.scope));

        switch (l.value)
        {
            case Integer lValue when r.value is Integer rValue:
                return new Integer(lValue.Value * rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
            case Real lValue when r.value is Real rValue:
                return new Real(lValue.Value * rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
        }

        return Integer.Default;
    }
    
    public IScriptValue Divide(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("/", l.scope, r.scope));

        switch (l.value)
        {
            case Integer lValue when r.value is Integer rValue:
                return new Integer(lValue.Value / rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
            case Real lValue when r.value is Real rValue:
                return new Real(lValue.Value / rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
        }

        return Integer.Default;
    }
    
    public IScriptValue Modulo(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("%", l.scope, r.scope));

        switch (l.value)
        {
            case Integer lValue when r.value is Integer rValue:
                return new Integer(lValue.Value % rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
            case Real lValue when r.value is Real rValue:
                return new Real(lValue.Value % rValue.Value)
                {
                    IsConstant = lValue.IsConstant && rValue.IsConstant
                };
        }

        return Integer.Default;
    }

    public Boolean Negate(Func<IScriptValue> value)
    {
        var compiled = Compile(value);
        Root.Statements.Add(new PrefixStatement("!", compiled.scope));

        var val = (Boolean) compiled.value;
        return new Boolean(val.IsConstant && !val.Value)
        {
            IsConstant = val.IsConstant
        };
    }

    private int? Compare(IScriptValue left, IScriptValue right)
    {
        for (var i = 0; i < 2; i++)
        {
            var arg1 = i == 0 ? left : right;
            var arg2 = i == 0 ? right : left;
            var ret = arg1 switch
            {
                Integer l when arg2 is Integer r => l.Value.CompareTo(r.Value),
                Integer l when arg2 is Real r => l.Value.CompareTo(r.Value),
                Real l when arg2 is Real r => l.Value.CompareTo(r.Value),
                _ => default(int?)
            };

            if (ret != null)
            {
                if (i == 1)
                    ret = -ret;
                return ret;
            }
        }

        return null;
    }

    public Boolean Equal(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("==", l.scope, r.scope));

        return new Boolean(Compare(l.value, r.value) == 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean NotEqual(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("!=", l.scope, r.scope));

        return new Boolean(Compare(l.value, r.value) != 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean Greater(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement(">", l.scope, r.scope));

        return new Boolean(Compare(l.value, r.value) > 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean GreaterOrEqual(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement(">=", l.scope, r.scope));

        return new Boolean(Compare(l.value, r.value) >= 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean Less(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("<", l.scope, r.scope));

        return new Boolean(Compare(l.value, r.value) < 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean LessOrEqual(Func<IScriptValue> left, Func<IScriptValue> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("<=", l.scope, r.scope));


        return new Boolean(Compare(l.value, r.value) <= 0)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean And(Func<Boolean> left, Func<Boolean> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("&&", l.scope, r.scope));

        return new Boolean(((Boolean) l.value).Value && ((Boolean) r.value).Value)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public Boolean Or(Func<Boolean> left, Func<Boolean> right)
    {
        var l = Compile(left);
        var r = Compile(right);

        l.scope = l.scope with {IsList = true};
        r.scope = r.scope with {IsList = true};

        Root.Statements.Add(new OperationStatement("||", l.scope, r.scope));

        return new Boolean(((Boolean) l.value).Value || ((Boolean) r.value).Value)
        {
            IsConstant = l.value.IsConstant && r.value.IsConstant
        };
    }

    public T Cast<T>(Func<IScriptValue> value)
        where T : IScriptValue
    {
        var compiled = Compile(value);
        compiled.scope = compiled.scope with
        {
            IsList = true
        };
        var ret = compiled.value.Bottom() is T val
            ? val
            : (T) DefaultFrom(typeof(T));

        Root.Statements.Add(new CastAssignment(compiled.scope, ret.GetManiaScriptTypeName()));

        return ret;
    }

    public IScriptValue Cast(Func<IScriptValue> value, Type type)
    {
        var compiled = Compile(value);
        compiled.scope = compiled.scope with
        {
            IsList = true
        };
        var ret = type.IsAssignableTo(compiled.value.Bottom().GetType())
            ? compiled.value
            : DefaultFrom(type);

        Root.Statements.Add(new CastAssignment(compiled.scope, ret.GetManiaScriptTypeNameNonGeneric()));
        
        return ret;
    }
}
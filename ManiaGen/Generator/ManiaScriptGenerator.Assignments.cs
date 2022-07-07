using System.Runtime.InteropServices;
using ManiaGen.Generator.Statements;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Libs;

namespace ManiaGen.Generator;

using static IScriptValue;

public partial class ManiaScriptGenerator
{
    private void AssignBase(
        ref (IScriptValue value, CollectionStatement scope) variable,
        ref (IScriptValue value, CollectionStatement scope) value)
    {
        if ((variable.value.Bottom() is Real || variable.value is Variable<Real>) && value.value.Bottom() is Integer)
        {
            var oldScope = value.scope;
            var reference = Ref<Integer>((Integer) 42);
            value.scope = Compile(
                () => MsMathLib.ToRealApi.Call(this, () => reference)
            ).scope;

            // Set back the old statement
            var span = CollectionsMarshal.AsSpan(value.scope.Statements);
            foreach (ref var statement in span)
            {
                ManiaScriptStatement.RecursivelyUpdateStatement(ref statement, reference.Bottom(), oldScope);
            }
        }
    }
    
    public IScriptValue Assign(Func<IScriptValue> variable, Func<IScriptValue> value)
    {
        var compiledVariable = Compile(variable);
        var compiledValue = Compile(value);
        AssignBase(ref compiledVariable, ref compiledValue);
        
        Root.Statements.Add(new AssignStatement(
            compiledValue.value.Bottom() is IIsClass ? "<=>" : "=",
            compiledVariable.scope with {IsList = true},
            compiledValue.scope with {IsList = true}
        ));
        return compiledVariable.value;
    }
    
    public IScriptValue AssignAdd(Func<IScriptValue> variable, Func<IScriptValue> value)
    {
        var compiledVariable = Compile(variable);
        var compiledValue = Compile(value);
        AssignBase(ref compiledVariable, ref compiledValue);
        
        Root.Statements.Add(new AssignStatement(
            "+=",
            compiledVariable.scope with {IsList = true},
            compiledValue.scope with {IsList = true}
        ));
        return Compile(() => Add(variable, value)).value;
    }
    
    public IScriptValue AssignMultiply(Func<IScriptValue> variable, Func<IScriptValue> value)
    {
        var compiledVariable = Compile(variable);
        var compiledValue = Compile(value);
        AssignBase(ref compiledVariable, ref compiledValue);
        
        Root.Statements.Add(new AssignStatement(
            "*=",
            compiledVariable.scope with {IsList = true},
            compiledValue.scope with {IsList = true}
        ));
        return Compile(() => Add(variable, value)).value;
    }
    
    public IScriptValue AssignDivide(Func<IScriptValue> variable, Func<IScriptValue> value)
    {
        var compiledVariable = Compile(variable);
        var compiledValue = Compile(value);
        AssignBase(ref compiledVariable, ref compiledValue);
        
        Root.Statements.Add(new AssignStatement(
            "/=",
            compiledVariable.scope with {IsList = true},
            compiledValue.scope with {IsList = true}
        ));
        return Compile(() => Add(variable, value)).value;
    }
}
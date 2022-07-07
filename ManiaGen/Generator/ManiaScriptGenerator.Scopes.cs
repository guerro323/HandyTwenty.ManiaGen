using ManiaGen.Generator.Statements;
using ManiaGen.ManiaPlanet;

namespace ManiaGen.Generator;

using static IScriptValue;

public partial class ManiaScriptGenerator
{
    private (bool next, bool added) AddBranch(Func<Boolean>[] conditions, BlockStatement blockToUse, Action then)
    {
        var compiledConditions = conditions.Select(Compile);
        
        // Use the scope if either it's an Else/ElseIf statement or if one of the condition isn't constant
        var useScope = blockToUse is ElseStatement;
        var execute = true;
        var hadRuntimeValue = false;
        foreach (var (condition, cs) in compiledConditions)
        {
            var conditionScope = (ManiaScriptStatement) cs;
            // if a condition is a method access make sure that it's not inlined
            // in the future, we'll put the inlined method outside (but we will need to keep the same execution order!)
            ManiaScriptStatement.RecursivelyUpdateStatement(
                ref conditionScope,
                s => s is MethodCallStatement, 
                s =>
                {
                    ((MethodCallStatement) s).Method.DisableInlining = true;
                    return s;
                }
            );
            
            if (!condition.IsConstant)
            {
                hadRuntimeValue = true;
                
                if (blockToUse is IfStatement ifStatement)
                {
                    ifStatement.Conditions.Add(conditionScope);
                }
                else if (blockToUse is ElseIfStatement elseIfStatement)
                {
                    elseIfStatement.Conditions.Add(conditionScope);
                }
                
                useScope = true;
                continue;
            }
            
            if (((Boolean) condition).Value)
                continue;

            execute = false;
        }

        if (!execute)
            return (true, false);

        // If it was an ElseIf statement, but empty because all true constants were eliminated,
        // we must ensure that further branches can be compiled successfully:
        // so add a True statement in it.
        if (!useScope && !hadRuntimeValue && blockToUse is ElseIfStatement statement)
        {
            /*useScope = true;
            statement.Conditions.Add(new RawStatement("True"));*/

            blockToUse = new ElseStatement(statement.Statements);
        }
        
        if (useScope || blockToUse is ElseStatement)
        {
            var oldRoot = Root;
            var scope = blockToUse;
            Root = scope;

            try
            {
                then();
            }
            catch (SuccessfulControlFlowException)
            {
                
            }

            Root = oldRoot;
            Root.Statements.Add(scope);
            return (blockToUse is not ElseStatement, true);
        }
        else
        {
            var oldRoot = Root;
            var scope = new BlockStatement(new List<ManiaScriptStatement>());
            Root = scope;

            try
            {
                then();
            }
            catch (SuccessfulControlFlowException)
            {
                
            }

            Root = oldRoot;
            Root.Statements.Add(scope);
            return (false, true);
        }
    }

    public IfGenerator If(Func<Boolean>[] conditions, Action then)
    {
        var ret = AddBranch(conditions, new IfStatement(new(), new()), then);
        return new IfGenerator(this, ret.next, ret.added);
    }
    
    public record struct IfGenerator(ManiaScriptGenerator Generator, bool Next, bool WasAdded)
    {
        public IfGenerator ElseIf(Func<Boolean>[] conditions, Action then)
        {
            if (!Next)
                return this;

            BlockStatement statement = WasAdded
                ? new ElseIfStatement(new List<ManiaScriptStatement>(), new List<ManiaScriptStatement>())
                : new IfStatement(new List<ManiaScriptStatement>(), new List<ManiaScriptStatement>());
            var ret = Generator.AddBranch(conditions, statement, then);
            Next = ret.next;
            WasAdded |= ret.added;
            
            return this;
        }

        public void Else(Action then)
        {
            if (!Next)
                return;
            
            var statement = WasAdded
                ? new ElseStatement(new List<ManiaScriptStatement>())
                : new BlockStatement(new List<ManiaScriptStatement>());
            
            Generator.AddBranch(Array.Empty<Func<Boolean>>(), statement, then);
        }
    }

    public void While(Func<Boolean>[] conditions, Action then)
    {
        var compiledConditions = conditions
            .Select(c => (ManiaScriptStatement) Compile(c).scope)
            .ToList();
        var compiledThen = Compile(() =>
        {
            try
            {
                then();
            }
            catch (SuccessfulControlFlowException){}
        });
        
        Root.Statements.Add(new WhileStatement(compiledConditions, compiledThen.Statements));
    }

    public void ForEach(Func<IScriptValue> enumerable, Action<IScriptValue> then)
    {
        var compiledEnumerable = Compile(enumerable);
        var valueVariable = VarFrom(
            $"fe_{_varCount++}",
            DefaultFrom(((IEnumerableValues) compiledEnumerable.value.Bottom()).ValueType)
        );
        var compiledThen = Compile(() =>
        {
            try
            {
                then(valueVariable);
            }
            catch (SuccessfulControlFlowException)
            {
            }
        });

        Root.Statements.Add(
            new ForEachStatement(compiledEnumerable.scope, valueVariable.ToStatement(), compiledThen.Statements)
        );
    }
}
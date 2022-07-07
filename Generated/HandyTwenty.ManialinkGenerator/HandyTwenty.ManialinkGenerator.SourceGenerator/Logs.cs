/*
Found Method warn
	LocalDeclarationStatementSyntax;;LocalDeclarationStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.Libs.MsTextLib.Compose(string, string)' '("WARN: %1", arg)'
Type: global::ManiaGen.ManiaPlanet.Libs.MsTextLib.ComposeApi
found: "WARN: %1"
	LiteralExpressionSyntax;;StringLiteralExpression
found: arg
	IdentifierNameSyntax;;IdentifierName
-> arg
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ParameterSymbol 'string'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.CManiaScript.log(string)' '(txt)'
Type: global::ManiaGen.ManiaPlanet.CManiaScript.Log
found: txt
	IdentifierNameSyntax;;IdentifierName
-> txt
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol 'txt'

using ManiaGen.Generator;
using ManiaGen.ManiaPlanet.Libs;
using static ManiaGen.ManiaPlanet.IScriptValue;
namespace ManiaGen.ManiaPlanet;

partial class CManiaScript
{
    private static readonly object MSToken_warn = new();
    public static Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_warn => gen =>
    {
        if (gen.GetLinkedValue(MSToken_warn) is not { } m_warn)
            m_warn = gen.LinkObject(MSToken_warn, gen.CreateMethod(args =>
            {
                var arg_arg = args[0];
                var v_txt = gen.Declare(() => 
                global::ManiaGen.ManiaPlanet.Libs.MsTextLib.ComposeApi.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "WARN: %1"), 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(arg_arg)
                ), "txt");
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(v_txt)
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.Text),
            }, allowGeneric: false));
        return m_warn;
    };
}
Found Method OnClick
	IfStatementSyntax;;IfStatement
	BinaryExpressionSyntax;;EqualsExpression
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol
Node: ev
Node: Type
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ParameterSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Type'
Scope=Field, FieldScope=Parameter, Name=ev, FieldType=ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent
Scope=Field, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Api.Type, FieldType=
DirectVar=False IsMsType=True Length=2
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol
Found type symbol: ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.EType
found the attribute!
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: OnClickLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> OnClickLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.Button.OnClickLabel'
diff=1142

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class Button
{
    private readonly object MSToken_OnClick = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnClick => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnClick) is not { } m_OnClick)
            m_OnClick = gen.LinkObject(MSToken_OnClick, gen.CreateMethod(args =>
            {
                var arg_ev = args[0];
                gen.If(new[]
                {
                    () => gen.Equal(() => global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Api.Type.Get(gen, arg_ev), () => (global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent.Api.Types.EventType) CMlScriptEvent.EType.MouseClick)
                },
                () => 
                {
                    gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(OnClickLabel),
                    });
                });
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
                typeof(global::ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent),
            }, allowGeneric: false));
        return m_OnClick;
    };
}
Found Method OnUpdate
	IfStatementSyntax;;IfStatement
	BinaryExpressionSyntax;;GreaterThanExpression
	IdentifierNameSyntax;;IdentifierName
-> _counter
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._counter'
	LiteralExpressionSyntax;;NumericLiteralExpression
	BlockSyntax;;Block
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.Symbols.CMlLabel.SetText(string)' '($"Clicked {_counter} time(s)")'
Type: global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol
Node: _label
Node: SetText
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._label'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlLabel.SetText(string)'
Scope=Field, FieldScope=Global, Name=_label, FieldType=ManiaGen.ManiaPlanet.Symbols.CMlLabel
Scope=Method, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText, FieldType=
DirectVar=True IsMsType=True Length=1
found: $"Clicked {_counter} time(s)"
	InterpolatedStringExpressionSyntax;;InterpolatedStringExpression
	InterpolatedStringTextSyntax;;InterpolatedStringText
	InterpolationSyntax;;Interpolation
	IdentifierNameSyntax;;IdentifierName
-> _counter
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._counter'
	InterpolatedStringTextSyntax;;InterpolatedStringText
	ExpressionStatementSyntax;;ExpressionStatement
	AssignmentExpressionSyntax;;AddAssignmentExpression
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol
Node: _label
Node: Rotation
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._label'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlControl.Rotation'
Scope=Field, FieldScope=Global, Name=_label, FieldType=ManiaGen.ManiaPlanet.Symbols.CMlLabel
Scope=Field, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativeRotation, FieldType=
DirectVar=False IsMsType=True Length=2
	BinaryExpressionSyntax;;MultiplyExpression
	LiteralExpressionSyntax;;NumericLiteralExpression
	IdentifierNameSyntax;;IdentifierName
-> _counter
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._counter'
	ExpressionStatementSyntax;;ExpressionStatement
	AssignmentExpressionSyntax;;SimpleAssignmentExpression
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol
Node: _button
Node: Position
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._button'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlControl.Position'
Scope=Field, FieldScope=Global, Name=_button, FieldType=ManiaGen.Button
Scope=Field, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3, FieldType=
DirectVar=False IsMsType=True Length=2
	ObjectCreationExpressionSyntax;;ObjectCreationExpression
	LiteralExpressionSyntax;;NumericLiteralExpression
	LiteralExpressionSyntax;;NumericLiteralExpression
	ExpressionStatementSyntax;;ExpressionStatement
	AssignmentExpressionSyntax;;SimpleAssignmentExpression
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol
Node: _button
Node: Position
Node: X
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._button'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlControl.Position'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'System.Numerics.Vector2.X'
Scope=Field, FieldScope=Global, Name=_button, FieldType=ManiaGen.Button
Scope=Field, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3, FieldType=
Scope=Field, FieldScope=Local, Name=X, FieldType=float
DirectVar=False IsMsType=True Length=1
DirectVar=False IsMsType=True Length=2
	LiteralExpressionSyntax;;NumericLiteralExpression

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    private readonly object MSToken_OnUpdate = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnUpdate => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnUpdate) is not { } m_OnUpdate)
            m_OnUpdate = gen.LinkObject(MSToken_OnUpdate, gen.CreateMethod(args =>
            {
                if (gen.GetLinkedValue(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter")) is not { } g__counter)
                	g__counter = gen.LinkObject(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter"), gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.Integer>(_counter, "_counter"));
                if (gen.GetLinkedValue(this._label) is not { } g_this__label)
                	g_this__label = gen.LinkObject(this._label, gen.Global<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>(this._label, "this__label"));
                if (gen.GetLinkedValue(this._button) is not { } g_this__button)
                	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
                gen.If(new[]
                {
                    () => gen.Greater(() => g__counter, () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 0)
                },
                () => 
                {
                    global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText.Call(gen, 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>>(g_this__label), 
                        () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(gen.InterpolatedText(new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                        {
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Text) "Clicked ",
                            () => g__counter,
                            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Text) " time(s)",
                        }))
                    );
                    gen.AssignAdd(() => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativeRotation.Get(gen, g_this__label), () => gen.Multiply(() => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 4, () => g__counter));
                    gen.Assign(() => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3.Get(gen, g_this__button), () => gen.NetMethod(typeof(global::System.Numerics.Vector2), ".ctor", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => (global::ManiaGen.ManiaPlanet.IScriptValue.Real) 0.4835f,
                        () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 0,
                    }));
                    gen.Assign(() => gen.NetMethod(typeof(global::System.Numerics.Vector2), "X", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                    {
                        () => global::ManiaGen.ManiaPlanet.Symbols.CMlControl.Api.Properties.RelativePosition_V3.Get(gen, g_this__button),
                    }), () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 4);
                });
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_OnUpdate;
    };
}
Found Method error
	LocalDeclarationStatementSyntax;;LocalDeclarationStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.Libs.MsTextLib.Compose(string, string)' '("ERROR: %1", arg)'
Type: global::ManiaGen.ManiaPlanet.Libs.MsTextLib.ComposeApi
found: "ERROR: %1"
	LiteralExpressionSyntax;;StringLiteralExpression
found: arg
	IdentifierNameSyntax;;IdentifierName
-> arg
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ParameterSymbol 'string'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.CManiaScript.log(string)' '(txt)'
Type: global::ManiaGen.ManiaPlanet.CManiaScript.Log
found: txt
	IdentifierNameSyntax;;IdentifierName
-> txt
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol 'txt'

using ManiaGen.Generator;
using ManiaGen.ManiaPlanet.Libs;
using static ManiaGen.ManiaPlanet.IScriptValue;
namespace ManiaGen.ManiaPlanet;

partial class CManiaScript
{
    private static readonly object MSToken_error = new();
    public static Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_error => gen =>
    {
        if (gen.GetLinkedValue(MSToken_error) is not { } m_error)
            m_error = gen.LinkObject(MSToken_error, gen.CreateMethod(args =>
            {
                var arg_arg = args[0];
                var v_txt = gen.Declare(() => 
                global::ManiaGen.ManiaPlanet.Libs.MsTextLib.ComposeApi.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "ERROR: %1"), 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(arg_arg)
                ), "txt");
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>(v_txt)
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.Text),
            }, allowGeneric: false));
        return m_error;
    };
}
Found Method OnClick
	ExpressionStatementSyntax;;ExpressionStatement
	AssignmentExpressionSyntax;;AddAssignmentExpression
	IdentifierNameSyntax;;IdentifierName
-> _counter
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._counter'
	LiteralExpressionSyntax;;NumericLiteralExpression
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.Symbols.CMlLabel.SetText(string)' '("Click me more!")'
Type: global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol
Node: _button
Node: Label
Node: SetText
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._button'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.Button.Label'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol 'ManiaGen.ManiaPlanet.Symbols.CMlLabel.SetText(string)'
Scope=Field, FieldScope=Global, Name=_button, FieldType=ManiaGen.Button
Scope=Field, FieldScope=Global, Name=Label, FieldType=ManiaGen.ManiaPlanet.Symbols.CMlLabel
Scope=Method, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText, FieldType=
DirectVar=True IsMsType=True Length=2
found: "Click me more!"
	LiteralExpressionSyntax;;StringLiteralExpression
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.ManiaPlanet.CManiaScript.log(string)' '("clicked!")'
Type: global::ManiaGen.ManiaPlanet.CManiaScript.Log
found: "clicked!"
	LiteralExpressionSyntax;;StringLiteralExpression

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    private readonly object MSToken_OnClick = new();
    private Func<global::ManiaGen.Generator.ManiaScriptGenerator, global::ManiaGen.ManiaPlanet.IScriptValue> MS_OnClick => gen =>
    {
        if (gen.GetLinkedValue(MSToken_OnClick) is not { } m_OnClick)
            m_OnClick = gen.LinkObject(MSToken_OnClick, gen.CreateMethod(args =>
            {
                if (gen.GetLinkedValue(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter")) is not { } g__counter)
                	g__counter = gen.LinkObject(new global::ManiaGen.Generator.ManiaScriptGenerator.LinkWithName(this, "MyPanel._counter"), gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.Integer>(_counter, "_counter"));
                if (gen.GetLinkedValue(this._button) is not { } g_this__button)
                	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
                if (gen.GetLinkedValue(_button.Label) is not { } g__button_Label)
                	g__button_Label = gen.LinkObject(_button.Label, gen.Global<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>(_button.Label, "_button_Label"));
                gen.AssignAdd(() => g__counter, () => (global::ManiaGen.ManiaPlanet.IScriptValue.Integer) 1);
                global::ManiaGen.ManiaPlanet.Symbols.CMlLabel.Api.Functions.SetText.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.Symbols.CMlLabel>>(g__button_Label), 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "Click me more!")
                );
                global::ManiaGen.ManiaPlanet.CManiaScript.Log.Call(gen, 
                    () => global::ManiaGen.ManiaPlanet.IScriptValue.CastTo<global::ManiaGen.ManiaPlanet.IScriptValue.Variable<global::ManiaGen.ManiaPlanet.IScriptValue.Text>>((global::ManiaGen.ManiaPlanet.IScriptValue.Text) "clicked!")
                );
                return gen.Return(() => global::ManiaGen.ManiaPlanet.IScriptValue.Void.Default);
            }, new Type[] {
                typeof(global::ManiaGen.ManiaPlanet.IScriptValue.NetObject),
            }, allowGeneric: false));
        return m_OnClick;
    };
}
Elapsed (scriptSub) 13,9128ms
Found Class MyPanel
Found a flow variable named '_button'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Subscribe(System.Action)' '(OnClick)'
found: _button.OnClickLabel.Subscribe
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol
Node: _button
Node: OnClickLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.MyPanel._button'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol 'ManiaGen.Button.OnClickLabel'
Scope=Field, FieldScope=Global, Name=_button, FieldType=ManiaGen.Button
Scope=Field, FieldScope=Global, Name=OnClickLabel, FieldType=ManiaGen.Generator.Flow.EventLabel
DirectVar=True IsMsType=False Length=2
found: OnClick
	IdentifierNameSyntax;;IdentifierName
-> OnClick
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol 'ManiaGen.MyPanel.OnClick()'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Subscribe(System.Action)' '(OnUpdate)'
found: UpdateLabel.Subscribe
	IdentifierNameSyntax;;IdentifierName
-> UpdateLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.UpdateLabel'
diff=1058
found: OnUpdate
	IdentifierNameSyntax;;IdentifierName
-> OnUpdate
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol 'ManiaGen.MyPanel.OnUpdate()'

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class MyPanel
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        if (!___generatedObjects.Contains(_button))
        {
            _button.Generate(gen, ___generatedObjects);
            ___generatedObjects.Add(_button);
        }
        if (gen.GetLinkedValue(this._button) is not { } g_this__button)
        	g_this__button = gen.LinkObject(this._button, gen.Global<global::ManiaGen.Button>(this._button, "this__button"));
        if (gen.GetLinkedValue(_button.OnClickLabel) is not { } g__button_OnClickLabel)
        	g__button_OnClickLabel = gen.LinkObject(_button.OnClickLabel, gen.Global<global::ManiaGen.ManiaPlanet.IScriptValue.NetObject>(new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(_button.OnClickLabel), "_button_OnClickLabel"));
        var converted_OnClick = MS_OnClick(gen);
        var converted_OnUpdate = MS_OnUpdate(gen);
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => g__button_OnClickLabel,
            () => converted_OnClick,
        });
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(UpdateLabel),
            () => converted_OnUpdate,
        });
    }
}
Found Class MlComposer
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: PreMainLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> PreMainLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.PreMainLabel'
diff=513
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: MainLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> MainLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.MainLabel'
diff=762
	WhileStatementSyntax;;WhileStatement
	LiteralExpressionSyntax;;TrueLiteralExpression
	BlockSyntax;;Block
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: PreUpdateLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> PreUpdateLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.PreUpdateLabel'
diff=1165
	ForEachStatementSyntax;;ForEachStatement
	MemberAccessExpressionSyntax;;SimpleMemberAccessExpression
first symbol: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol
Node: Nod
Node: PendingEvents
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.Nod'
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.ManiaPlanet.Symbols.CMlScript.PendingEvents'
Scope=Field, FieldScope=Global, Name=Nod, FieldType=ManiaGen.HTManialink.CMlScriptExtended
Scope=Field, FieldScope=Local, Name=global::ManiaGen.ManiaPlanet.Symbols.CMlScript.Api.Properties.PendingEvents, FieldType=
DirectVar=False IsMsType=True Length=2
	BlockSyntax;;Block
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>.Invoke(ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent)' '(ev)'
found: PendingEventLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> PendingEventLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.PendingEventLabel'
diff=1653
found: ev
	IdentifierNameSyntax;;IdentifierName
-> ev
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol 'ev'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: UpdateLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> UpdateLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.UpdateLabel'
diff=1975
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel.Invoke()' '()'
found: AnimateLabel.Invoke
	IdentifierNameSyntax;;IdentifierName
-> AnimateLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.AnimateLabel'
diff=2239

namespace ManiaGen.HTManialink;

partial class MlComposer
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        if (gen.GetLinkedValue(this.Nod) is not { } g_this_Nod)
        	g_this_Nod = gen.LinkObject(this.Nod, gen.Global<global::ManiaGen.HTManialink.CMlScriptExtended>(this.Nod, "this_Nod"));
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PreMainLabel),
        });
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(MainLabel),
        });
        gen.While(new[]
        {
            () => (global::ManiaGen.ManiaPlanet.IScriptValue.Boolean) true
        },
        () => 
        {
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PreUpdateLabel),
            });
            gen.ForEach(() => global::ManiaGen.ManiaPlanet.Symbols.CMlScript.Api.Properties.PendingEvents.Get(gen, g_this_Nod), 
            v_ev => 
            {
                gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
                {
                    () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PendingEventLabel),
                    () => v_ev,
                });
            });
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(UpdateLabel),
            });
            gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel), "Invoke", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
            {
                () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(AnimateLabel),
            });
        });
    }
}
Found Class MlComponent

using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.ManiaPlanet;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen.HTManialink;

partial class MlComponent
{
    public virtual void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
    }
}
Found Class Button
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.HTManialink.MlComponent.Entry()' '()'
	ExpressionStatementSyntax;;ExpressionStatement
	InvocationExpressionSyntax;;InvocationExpression
method: 'ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>.Subscribe(System.Action<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>)' '(OnClick)'
found: PendingEventLabel.Subscribe
	IdentifierNameSyntax;;IdentifierName
-> PendingEventLabel
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol 'ManiaGen.HTManialink.MlComponent.PendingEventLabel'
diff=729
found: OnClick
	IdentifierNameSyntax;;IdentifierName
-> OnClick
Symbol Type: Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.MethodSymbol 'ManiaGen.Button.OnClick(ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent)'

using System.Numerics;
using ManiaGen.Generator;
using ManiaGen.Generator.Flow;
using ManiaGen.HTManialink;
using ManiaGen.ManiaPlanet.Symbols;
namespace ManiaGen;

partial class Button
{
    public override void Generate(global::ManiaGen.Generator.ManiaScriptGenerator gen, global::System.Collections.Generic.HashSet<global::System.Object> ___generatedObjects)
    {
        GenerateOthers(gen, ___generatedObjects);
        var converted_OnClick = MS_OnClick(gen);base.Generate(gen, ___generatedObjects);
        gen.NetMethod(typeof(global::ManiaGen.Generator.Flow.EventLabel<ManiaGen.ManiaPlanet.Symbols.CMlScriptEvent>), "Subscribe", new Func<global::ManiaGen.ManiaPlanet.IScriptValue>[]
        {
            () => new global::ManiaGen.ManiaPlanet.IScriptValue.NetObject(PendingEventLabel),
            () => converted_OnClick,
        });
    }
}
Elapsed (flowSub) 5,8572ms
*/
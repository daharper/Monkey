namespace Monkey.Evaluating.Ast;

public class AstReturnValue(IAstObject? value) : IAstObject
{
    public IAstObject Value { get; } = value ?? Builtin.Null;

    public string Type() => AstTypes.ReturnValueObj;

    public string Inspect() => Value.Inspect();
}
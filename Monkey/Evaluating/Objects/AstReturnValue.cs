namespace Monkey.Evaluating.Objects;

public class AstReturnValue : IAstObject
{
    public IAstObject Value { get; init; }

    public string Type() => AstTypes.ReturnValueObj;

    public string Inspect() => Value.Inspect();
}
namespace Monkey.Evaluating.Objects;

public class ReturnObject(IObject? value) : IObject
{
    public IObject Value { get; } = value ?? Builtin.Null;

    public string Type() => ObjectTypes.ReturnValueObj;

    public override string ToString() => Value.ToString() ?? "";
}
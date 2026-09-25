namespace Monkey.Evaluating.Objects;

public class ReturnObject(MonkeyObject? value) : MonkeyObject(ObjectTypes.Return)
{
    public MonkeyObject Value { get; } = value ?? Builtin.Null;

    public override string ToString() => Value.ToString() ?? "";
}
namespace Monkey.Evaluating.Objects;

public class BooleanObject() : MonkeyObject(ObjectTypes.Boolean), IHashable
{
    public bool Value { get; init; }

    public HashKey HashKey => new(Type, Value ? 1 : 0);

    public override string ToString() => Convert.ToString(Value);
}

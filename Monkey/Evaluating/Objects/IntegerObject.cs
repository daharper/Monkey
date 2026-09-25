namespace Monkey.Evaluating.Objects;

public class IntegerObject(int value) : MonkeyObject(ObjectTypes.Integer), IHashable
{
    public int Value { get; } = value;

    public HashKey HashKey => new(Type, Value);

    public override string ToString() => Convert.ToString(Value);
}

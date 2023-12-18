namespace Monkey.Evaluating.Objects;

public class IntegerObject(int value) : MObject(ObjectTypes.Integer)
{
    public int Value { get; } = value;

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;
}
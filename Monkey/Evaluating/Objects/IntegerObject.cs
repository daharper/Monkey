namespace Monkey.Evaluating.Objects;

public class IntegerObject() : MObject(ObjectTypes.Integer)
{
    public int Value { get; set; }

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;
}
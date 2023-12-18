namespace Monkey.Evaluating.Objects;

public class IntegerObject : IObject
{
    public int Value { get; set; }

    public string Type() => ObjectTypes.IntegerObj;

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;
}
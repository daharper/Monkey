namespace Monkey.Evaluating.Objects;

public class BooleanObject : IObject
{
    public bool Value { get; init; }

    public string Type() => ObjectTypes.BooleanObj;
    
    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value ? 1 : 0;
}
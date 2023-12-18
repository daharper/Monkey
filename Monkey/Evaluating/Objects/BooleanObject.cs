namespace Monkey.Evaluating.Objects;

public class BooleanObject() : MObject(ObjectTypes.Boolean)
{
    public bool Value { get; init; }
    
    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value ? 1 : 0;
}
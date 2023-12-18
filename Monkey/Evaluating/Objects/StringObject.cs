namespace Monkey.Evaluating.Objects;

public class StringObject(string value = "") : MObject(ObjectTypes.String)
{
    public string Value { get; } = value;
    
    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();
}
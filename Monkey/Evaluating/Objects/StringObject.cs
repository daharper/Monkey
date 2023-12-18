namespace Monkey.Evaluating.Objects;

public class StringObject(string value = "") : IObject
{
    public string Value { get; } = value;

    public string Type() => ObjectTypes.StringObj;
    
    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();
}
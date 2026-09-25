namespace Monkey.Evaluating.Objects;

public class StringObject(string value = "") : MonkeyObject(ObjectTypes.String), IHashable
{
    public string Value { get; } = value;

    public HashKey HashKey => new(Type, Value.GetHashCode());

    public override string ToString() => Value;
}

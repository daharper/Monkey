namespace Monkey.Evaluating.Objects;

public class MString(string value = "") : IMObject
{
    public string Value { get; } = value;

    public string Type() => Types.StringObj;

    public string Inspect() => Value;
    
    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();

    // public MHashKey HashKey()
    //     => new()
    //     {
    //         Type = Types.StringObj,
    //         Value = Value.GetHashCode()
    //     };
}
namespace Monkey.Evaluating.Objects;

public class MInteger : IMObject
{
    public int Value { get; set; }

    public string Type() => Types.IntegerObj;

    public string Inspect() => Value.ToString();

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;

    // public MHashKey HashKey
    //     => new()
    //     {
    //         Type = Types.IntegerObj,
    //         Value = Value
    //     };
}
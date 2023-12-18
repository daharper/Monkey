namespace Monkey.Evaluating.Objects;

public class MBoolean : IMObject
{
    public bool Value { get; init; }

    public string Type() => Types.BooleanObj;

    public string Inspect() => Value.ToString();
    
    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value ? 1 : 0;

    // public MHashKey HashKey()
    //     => new()
    //     {
    //         Type = Types.BooleanObj,
    //         Value = Value ? 1 : 0
    //     };
}
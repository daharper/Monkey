namespace Monkey.Evaluating.Objects;

public class MBoolean : IMObject
{
    public bool Value { get; set; }

    public string Type() => Types.BooleanObj;

    public string Inspect() => Value.ToString();
}
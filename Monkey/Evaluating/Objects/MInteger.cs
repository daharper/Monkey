namespace Monkey.Evaluating.Objects;

public class MInteger : IMObject
{
    public int Value { get; set; }

    public string Type() => Types.IntegerObj;

    public string Inspect() => Value.ToString();
}
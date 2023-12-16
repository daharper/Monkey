namespace Monkey.Evaluating.Objects;

public class MReturnValue : IMObject
{
    public IMObject Value { get; set; }

    public string Type() => Types.ReturnValueObj;

    public string Inspect() => Value.Inspect();
}
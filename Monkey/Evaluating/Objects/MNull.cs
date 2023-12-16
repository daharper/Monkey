namespace Monkey.Evaluating.Objects;

public class MNull : IMObject
{
    public string Type() => Types.NullObj;

    public string Inspect() => "null";
}
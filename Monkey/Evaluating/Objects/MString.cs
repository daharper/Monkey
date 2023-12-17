namespace Monkey.Evaluating.Objects;

public class MString : IMObject
{
    public string Value { get; set; }
    
    public string Type() => Types.StringObj;

    public string Inspect() => Value;
}
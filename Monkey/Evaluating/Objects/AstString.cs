namespace Monkey.Evaluating.Objects;

public class AstString(string value = "") : IAstObject
{
    public string Value { get; } = value;

    public string Type() => AstTypes.StringObj;

    public string Inspect() => Value;
    
    public override string ToString() => Value;

    public override int GetHashCode() => Value.GetHashCode();
}
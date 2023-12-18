namespace Monkey.Evaluating.Objects;

public class AstInteger : IAstObject
{
    public int Value { get; set; }

    public string Type() => AstTypes.IntegerObj;

    public string Inspect() => Value.ToString();

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;
}
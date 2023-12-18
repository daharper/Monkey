namespace Monkey.Evaluating.Ast;

public class AstInteger : IAstObject
{
    public int Value { get; set; }

    public string Type() => AstTypes.IntegerObj;

    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value;
}
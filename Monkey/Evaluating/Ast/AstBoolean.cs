namespace Monkey.Evaluating.Ast;

public class AstBoolean : IAstObject
{
    public bool Value { get; init; }

    public string Type() => AstTypes.BooleanObj;

    public string Inspect() => Value.ToString();
    
    public override string ToString() => Convert.ToString(Value);

    public override int GetHashCode() => Value ? 1 : 0;
}
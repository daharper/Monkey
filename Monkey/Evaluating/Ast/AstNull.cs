namespace Monkey.Evaluating.Ast;

public class AstNull : IAstObject
{
    public string Type() => AstTypes.NullObj;
    
    public override string ToString() => "null";
}
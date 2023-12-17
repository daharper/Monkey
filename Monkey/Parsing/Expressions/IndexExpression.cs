using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Expressions;

public class IndexExpression : IExpression
{
    public Token Token { get; set; } = null!;
    
    public IExpression Left { get; set; } = null!;
    
    public IExpression Index { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }
    
    public override string ToString() => $"({Left}[{Index}])";
}
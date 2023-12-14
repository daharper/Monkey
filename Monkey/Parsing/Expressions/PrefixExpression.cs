using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Expressions;

public class PrefixExpression : IExpression
{
    public Token Token { get; set; } = null!;
    
    public string Operator { get; set; } = null!;
    
    public IExpression Right { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
    
    public void ExpressionNode() { }
    
    public override string ToString()
        => $"({Operator}{Right})";
}
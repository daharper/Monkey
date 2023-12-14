using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Expressions;

public class InfixExpression : IExpression
{
    public Token Token { get; set; } = null!;
    
    public IExpression Left { get; set; } = null!;
    
    public string Operator { get; set; } = null!;
    
    public IExpression Right { get; set; } = null!;
    
    public override string ToString()
        => $"({Left} {Operator} {Right})";
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }
}
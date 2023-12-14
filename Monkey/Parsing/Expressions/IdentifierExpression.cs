using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Expressions;

public class IdentifierExpression : IExpression
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public string TokenLiteral()
        => Token.Literal;

    public void ExpressionNode() { }
    
    public override string ToString() => Value;
}
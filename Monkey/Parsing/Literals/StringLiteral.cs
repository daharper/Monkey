using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Literals;

public class StringLiteral : ILiteral
{
    public Token Token { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }

    public override string ToString() => Token.Literal;
}
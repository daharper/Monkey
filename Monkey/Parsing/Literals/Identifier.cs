using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Literals;

public class Identifier : ILiteral
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public string TokenLiteral()
        => Token.Literal;

    public void ExpressionNode() { }
    
    public override string ToString() => Value;
}
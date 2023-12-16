using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Literals;

public class IntegerLiteral : ILiteral
{
    public Token Token { get; set; } = null!;
    
    public int Value { get; set; }
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() {}
    
    public override string ToString() => Token.Literal;
}
using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IntegerLiteral : INode
{
    public Token Token { get; set; } = null!;
    
    public int Value { get; set; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Token.Literal;
}
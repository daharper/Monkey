using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class BooleanLiteral : INode
{
    public Token Token { get; set; } = null!;
    
    public bool Value { get; set; }
    
    public string TokenLiteral() => Token.Literal;

    public override string ToString() => Token.Literal;
}
using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class StringLiteral : INode
{
    public Token Token { get; set; } = null!;
    
    public string Value { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public override string ToString() => Token.Literal; 
}
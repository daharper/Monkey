using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class Identifier : INode
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => Value;
}
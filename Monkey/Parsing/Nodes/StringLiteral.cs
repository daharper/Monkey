using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class StringLiteral(Token token, string value) : Node(token)
{
    public string Value { get; } = value;
    
    public override string ToString() => Token.Literal; 
}
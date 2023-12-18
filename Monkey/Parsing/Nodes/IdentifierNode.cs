using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IdentifierNode(Token token, string value) : Node(token)
{
    public string Value { get; } = value;
   
    public override string ToString() => Value;
}
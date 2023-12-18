using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class BooleanNode(Token token, bool value) : Node(token)
{
    public bool Value { get; } = value;

    public override string ToString() => Token.Literal;
}
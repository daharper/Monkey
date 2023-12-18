using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IntegerLiteral(Token token, int value) : Node(token)
{
    public int Value { get; private set; } = value;

    public override string ToString() => Token.Literal;
}
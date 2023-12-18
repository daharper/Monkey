using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ExpressionNode(Token token, Node expression) : Node(token)
{
    public Node Expression { get; } = expression;

    public override string ToString() => Expression.ToString()!;
}
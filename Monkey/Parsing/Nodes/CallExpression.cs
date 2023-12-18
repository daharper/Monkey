using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class CallExpression(Token token, Node function, List<Node> arguments) : Node(token)
{
    public Node Function { get; } = function;

    public List<Node> Arguments { get; } = arguments;
    
    public override string ToString()
        => $"{Function}({string.Join(", ", Arguments.Select(a => a.ToString()))})";
}
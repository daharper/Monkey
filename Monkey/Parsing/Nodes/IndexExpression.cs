using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IndexExpression(Token token, Node left) : Node(token)
{
    public Node Left { get; } = left;

    public Node Index { get; set; } = null!;
    
    public override string ToString() => $"({Left}[{Index}])";
}
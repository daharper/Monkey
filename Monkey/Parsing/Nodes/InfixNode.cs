using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class InfixNode(Token token, string @operator, Node left) : Node(token)
{
    public Node Left { get; } = left;

    public string Operator { get; } = @operator;

    public Node Right { get; set; } = null!;
    
    public override string ToString() => $"({Left} {Operator} {Right})";
}
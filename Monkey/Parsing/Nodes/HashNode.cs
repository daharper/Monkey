using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class HashNode(Token token) : Node(token)
{
    public Dictionary<Node, Node> Pairs { get; set; } = new();
    
    public override string ToString()
        => "{ " + string.Join(", ", Pairs.Select((k, v) => $"{k}: {v}")) + " }";
}
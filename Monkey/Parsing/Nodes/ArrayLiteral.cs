using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ArrayLiteral(Token token, List<Node> elements) : Node(token)
{
    public List<Node> Elements { get; } = elements;
    
    public override string ToString()
        =>$"[{string.Join(", ", Elements.Select(e => e.ToString()))}]";
}
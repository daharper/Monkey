using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class PrefixNode(Token token, string @operator) : Node(token)
{
    public string Operator { get; } = @operator;

    public Node Right { get; set; } = null!;
    
    public override string ToString() => $"({Operator}{Right})";
}
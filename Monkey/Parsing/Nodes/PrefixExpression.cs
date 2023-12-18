using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class PrefixExpression(Token token, string @operator) : Node(token)
{
    public string Operator { get; } = @operator;

    public Node Right { get; set; } = null!;
    
    public override string ToString() => $"({Operator}{Right})";
}
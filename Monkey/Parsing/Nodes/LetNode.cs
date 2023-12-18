using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class LetNode(Token token) : Node(token)
{
    public IdentifierNode Name { get; set; } = null!;
    
    public Node Value { get; set; } = null!;

    public override string ToString()
        => $"{TokenLiteral()} {Name} = {Value};";
}
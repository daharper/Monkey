using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class LetStatement(Token token) : Node(token)
{
    public Identifier Name { get; set; } = null!;
    
    public Node Value { get; set; } = null!;

    public override string ToString()
        => $"{TokenLiteral()} {Name} = {Value};";
}
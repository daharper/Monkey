using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IfNode(Token token) : Node(token)
{
    public Node Condition { get; set; } = null!;
    
    public BlockNode Consequence { get; set; } = null!;
    
    public BlockNode? Alternative { get; set; }

    public override string ToString()
    {
        var result = $"if {Condition} {Consequence}";

        if (Alternative != null)
            result += $" else {Alternative}";

        return result;
    }
}
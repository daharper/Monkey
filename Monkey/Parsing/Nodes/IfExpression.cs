using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IfExpression : INode
{
    public Token Token { get; set; } = null!;
    
    public INode Condition { get; set; } = null!;
    
    public BlockStatement Consequence { get; set; } = null!;
    
    public BlockStatement? Alternative { get; set; }

    public override string ToString()
    {
        var result = $"if {Condition} {Consequence}";

        if (Alternative != null)
            result += $" else {Alternative}";

        return result;
    }

    public string TokenLiteral() => Token.Literal;
}
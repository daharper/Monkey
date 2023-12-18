using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ReturnNode(Token token) : Node(token)
{
    // todo - if we want to support return without an expression, this should be nullable
    public Node ReturnValue { get; set; } = null!;
    
    public override string ToString() => $"{TokenLiteral()} {ReturnValue};";
}
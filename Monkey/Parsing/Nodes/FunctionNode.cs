using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class FunctionNode(Token token) : Node(token)
{
    public List<IdentifierNode> Parameters { get; set; } = null!;
    
    public BlockNode Body { get; set; } = null!;
    
    public override string ToString()
        =>$"{TokenLiteral()}({string.Join(", ", Parameters.Select(p => p.ToString()))}) {Body}";
}
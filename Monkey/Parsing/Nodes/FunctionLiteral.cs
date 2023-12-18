using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class FunctionLiteral(Token token) : Node(token)
{
    public List<Identifier> Parameters { get; set; } = null!;
    
    public BlockStatement Body { get; set; } = null!;
    
    public override string ToString()
        =>$"{TokenLiteral()}({string.Join(", ", Parameters.Select(p => p.ToString()))}) {Body}";
}
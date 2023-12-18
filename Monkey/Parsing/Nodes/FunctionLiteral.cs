using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class FunctionLiteral : INode
{
    public Token Token { get; set; } = null!;
    
    public List<Identifier> Parameters { get; set; } = null!;
    
    public BlockStatement Body { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
    {
        var parameters = Parameters.Select(p => p.ToString());
        return $"{TokenLiteral()}({string.Join(", ", parameters)}) {Body}";
    }
}
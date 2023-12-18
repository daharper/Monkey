using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ExpressionStatement : INode
{
    public Token Token { get; set; } = null!;
    
    public INode? Expression { get; set; }
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
        => Expression?.ToString() ?? "";
}
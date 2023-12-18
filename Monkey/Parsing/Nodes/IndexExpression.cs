using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class IndexExpression : INode
{
    public Token Token { get; set; } = null!;
    
    public INode Left { get; set; } = null!;
    
    public INode Index { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString() => $"({Left}[{Index}])";
}
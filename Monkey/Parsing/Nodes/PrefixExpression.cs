using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class PrefixExpression : INode
{
    public Token Token { get; set; } = null!;
    
    public string Operator { get; set; } = null!;
    
    public INode Right { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
     
    public override string ToString() => $"({Operator}{Right})";
}
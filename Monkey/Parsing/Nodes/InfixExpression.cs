using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class InfixExpression : INode
{
    public Token Token { get; set; } = null!;
    
    public INode Left { get; set; } = null!;
    
    public string Operator { get; set; } = null!;
    
    public INode Right { get; set; } = null!;
    
    public override string ToString() => $"({Left} {Operator} {Right})";
    
    public string TokenLiteral() => Token.Literal;
}
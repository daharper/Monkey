using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class LetStatement : INode
{
    public Token Token { get; set; } = null!;
    
    public Identifier Name { get; set; } = null!;
    
    public INode Value { get; set; } = null!;

    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
        => $"{TokenLiteral()} {Name} = {Value};";
}
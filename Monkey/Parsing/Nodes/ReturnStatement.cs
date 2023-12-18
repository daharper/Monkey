using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ReturnStatement : INode
{
    public Token Token { get; set; } = null!;

    // todo - if we want to support return without an expression, this should be nullable
    public INode ReturnValue { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
        => $"{TokenLiteral()} {ReturnValue};";
}
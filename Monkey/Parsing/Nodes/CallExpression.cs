using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class CallExpression : INode
{
    public Token Token { get; set; } = null!;
    
    public INode Function { get; set; } = null!;
    
    public List<INode> Arguments { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;
    
    public override string ToString()
    {
        var args = Arguments.Select(a => a.ToString());
        return $"{Function}({string.Join(", ", args)})";
    }
}
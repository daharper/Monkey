using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Expressions;

public class CallExpression : IExpression
{
    public Token Token { get; set; } = null!;
    
    public IExpression Function { get; set; } = null!;
    
    public List<IExpression> Arguments { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }
    
    public override string ToString()
    {
        var args = Arguments.Select(a => a.ToString());
        return $"{Function}({string.Join(", ", args)})";
    }
}
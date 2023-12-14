using Monkey.Lexing;
using Monkey.Parsing.Interfaces;
using Monkey.Parsing.Statements;

namespace Monkey.Parsing.Expressions;

public class IfExpression : IExpression
{
    public Token Token { get; set; } = null!;
    
    public IExpression Condition { get; set; } = null!;
    
    public BlockStatement Consequence { get; set; } = null!;
    
    public BlockStatement? Alternative { get; set; }

    public override string ToString()
    {
        var result = $"if {Condition} {Consequence}";

        if (Alternative != null)
            result += $" else {Alternative}";

        return result;
    }

    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }
}
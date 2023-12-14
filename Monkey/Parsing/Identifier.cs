using Monkey.Lexing;

namespace Monkey.Parsing;

public class Identifier : IExpression
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public string TokenLiteral()
        => Token.Literal;

    public void ExpressionNode() { }
}
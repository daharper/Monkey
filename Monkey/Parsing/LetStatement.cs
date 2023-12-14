using Monkey.Lexing;

namespace Monkey.Parsing;

public class LetStatement : IStatement
{
    public Token Token { get; set; } = null!;
    public Identifier Name { get; set; } = null!;
    public IExpression Value { get; set; } = null!;

    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }
}
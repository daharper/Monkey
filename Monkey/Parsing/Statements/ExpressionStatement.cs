using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Statements;

public class ExpressionStatement : IStatement
{
    public Token Token { get; set; } = null!;
    
    public IExpression Expression { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }
    
    public override string ToString()
        => Expression?.ToString() ?? "";
}
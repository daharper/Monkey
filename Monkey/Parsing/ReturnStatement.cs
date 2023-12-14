using Monkey.Lexing;

namespace Monkey.Parsing;

public class ReturnStatement : IStatement
{
    public Token Token { get; set; } = null!;

    // todo - if we want to support return without an expression, this should be nullable
    public IExpression ReturnValue { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }
}
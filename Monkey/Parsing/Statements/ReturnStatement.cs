using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Statements;

public class ReturnStatement : IStatement
{
    public Token Token { get; set; } = null!;

    // todo - if we want to support return without an expression, this should be nullable
    public IExpression ReturnValue { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }
    
    public override string ToString()
        => $"{TokenLiteral()} {ReturnValue};";
}
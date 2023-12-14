using Monkey.Lexing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Statements;

public class LetStatement : IStatement
{
    public Token Token { get; set; } = null!;
    
    public IdentifierExpression Name { get; set; } = null!;
    
    public IExpression Value { get; set; } = null!;

    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }
    
    public override string ToString()
        => $"{TokenLiteral()} {Name} = {Value};";
}
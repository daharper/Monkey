using System.Text;
using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Statements;

public class BlockStatement : IStatement
{
    public Token Token { get; set; } = null!;

    public List<IStatement> Statements { get; set; } = [];
    
    public string TokenLiteral() => Token.Literal;

    public void StatementNode() { }

    public override string ToString()
    {
        var builder = new StringBuilder();
        
        foreach (var statement in Statements)
        {
            builder.Append(statement);
        }

        return builder.ToString();
    }
}
using System.Text;
using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class BlockStatement : INode
{
    public Token Token { get; set; } = null!;

    public List<INode> Statements { get; set; } = [];
    
    public string TokenLiteral() => Token.Literal;

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
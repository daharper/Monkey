using System.Text;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing;

public class Programme : INode
{ 
    public List<IStatement> Statements { get; set; } = [];

    public string TokenLiteral()
        => Statements.Count > 0 ? Statements[0].TokenLiteral() : string.Empty;

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

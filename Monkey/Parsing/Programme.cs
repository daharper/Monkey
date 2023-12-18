using System.Text;
using Monkey.Parsing.Nodes;

namespace Monkey.Parsing;

public class Programme : INode
{ 
    public List<INode> Statements { get; set; } = [];

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

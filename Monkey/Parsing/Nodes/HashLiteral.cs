using System.Text;
using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class HashLiteral : INode
{
    public Token Token { get; set; } 
    
    public Dictionary<INode, INode> Pairs { get; set; } = new();
    
    public string TokenLiteral() => Token.Literal;

    public override string ToString()
    {
        var pairs = new List<string>();
        
        var sb = new StringBuilder();
        sb.Append("{");

        foreach (var (key, value) in Pairs)
        {
            sb.Append($"{key}: {value}, ");
        }
        
        if (Pairs.Count > 0)
        {
            sb.Length -= 2;
        }
        
        sb.Append("}");
        
        return sb.ToString();
    }
}
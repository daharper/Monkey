using System.Text;
using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Literals;

public class HashLiteral : ILiteral
{
    public Token Token { get; set; }
    
    public Dictionary<IExpression, IExpression> Pairs { get; set; } = new();
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }

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
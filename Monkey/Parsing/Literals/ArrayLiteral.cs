using System.Text;
using Monkey.Lexing;
using Monkey.Parsing.Interfaces;

namespace Monkey.Parsing.Literals;

public class ArrayLiteral : ILiteral
{
    public Token Token { get; set; } = null!;

    public List<IExpression> Elements { get; set; } = [];
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() {}

    // public override string ToString()
    //     => $"[{string.Join(", ", Elements.Select(e => e.ToString()))}]";

    public override string ToString()
    {
        var elements = new List<string>();
        
        foreach (var element in Elements)
        {
            elements.Add(element.ToString());
        }
        
        return $"[{string.Join(", ", elements)}]";
    }
}
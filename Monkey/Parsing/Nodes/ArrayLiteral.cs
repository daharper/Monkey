using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ArrayLiteral : INode
{
    public Token Token { get; set; } = null!;

    public List<INode> Elements { get; set; } = [];
    
    public string TokenLiteral() => Token.Literal;
    
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
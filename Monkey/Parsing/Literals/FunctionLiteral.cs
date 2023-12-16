using Monkey.Lexing;
using Monkey.Parsing.Interfaces;
using Monkey.Parsing.Statements;

namespace Monkey.Parsing.Literals;

public class FunctionLiteral : ILiteral
{
    public Token Token { get; set; } = null!;
    
    public List<Identifier> Parameters { get; set; } = null!;
    
    public BlockStatement Body { get; set; } = null!;
    
    public string TokenLiteral() => Token.Literal;

    public void ExpressionNode() { }
    
    public override string ToString()
    {
        var parameters = Parameters.Select(p => p.ToString());
        return $"{TokenLiteral()}({string.Join(", ", parameters)}) {Body}";
    }
}
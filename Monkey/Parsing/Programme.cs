using Monkey.Lexing;
using Monkey.Parsing.Nodes;

namespace Monkey.Parsing;

public class Programme : Node
{
    private Programme(Token token, List<Node> statements) : base(token)
        => Statements = statements;

    public List<Node> Statements { get; }

    public override string TokenLiteral()
        => Statements.Count > 0 ? Statements[0].TokenLiteral() : string.Empty;

    public override string ToString() 
        => string.Join("", Statements);
    
    public static Programme Create(List<Node>? statements = null) 
        => new(Token.Root, statements ?? []);
}

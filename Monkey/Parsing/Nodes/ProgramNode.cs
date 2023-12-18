using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class ProgramNode : Node
{
    private ProgramNode(Token token, List<Node> statements) : base(token)
        => Statements = statements;

    public List<Node> Statements { get; }

    public override string TokenLiteral()
        => Statements.Count > 0 ? Statements[0].TokenLiteral() : string.Empty;

    public override string ToString() 
        => string.Join("", Statements);
    
    public static ProgramNode Create(List<Node>? statements = null) 
        => new(Token.Root, statements ?? []);
}

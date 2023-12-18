using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class BlockStatement(Token token) : Node(token)
{
    public List<Node> Statements { get; } = [];
    
    public override string ToString() => string.Join("", Statements);
}
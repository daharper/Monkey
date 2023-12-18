using Monkey.Lexing;

namespace Monkey.Parsing.Nodes;

public class Node(Token token)
{
    public static readonly Node Null = new(Token.Null);
    
    public Token Token { get; } = token;

    public virtual string TokenLiteral() => Token.Literal;
}
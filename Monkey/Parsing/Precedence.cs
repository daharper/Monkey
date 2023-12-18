using System.Collections.Immutable;
using Monkey.Lexing;
using Monkey.Utils;

namespace Monkey.Parsing;

public enum Precedence
{
    Lowest,
    Equals, // ==
    LessGreater, // > or <
    Sum, // +
    Product, // *
    Prefix, // -X or !X
    Call, // myFunction(X)
    Index // array[index]
}

public static class Precedences
{
    private static readonly ImmutableDictionary<string, Precedence> TokenPrecedence 
        = new Dictionary<string, Precedence>
    {
        { Token.Eq, Precedence.Equals },
        { Token.NotEq, Precedence.Equals },
        { Token.Lt, Precedence.LessGreater },
        { Token.Gt, Precedence.LessGreater },
        { Token.Plus, Precedence.Sum },
        { Token.Minus, Precedence.Sum },
        { Token.Slash, Precedence.Product },
        { Token.Asterisk, Precedence.Product },
        { Token.LParen, Precedence.Call },
        { Token.LBracket, Precedence.Index }
    }.ToImmutableDictionary();
    
    public static Precedence Resolve(string tokenType)
        => CollectionExtensions.GetValueOrDefault(TokenPrecedence, tokenType, Precedence.Lowest);
}
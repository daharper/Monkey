using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record StringLexerFixture() : LexerFixture(
    """""
    "foobar"
    "foo bar"
    """x"
    """"",
    new List<Token>
    {
        new(Token.String, "foobar"),
        new(Token.String, "foo bar"),
        new(Token.String, ""),
        new(Token.String, "x"),
        new(Token.Eof)
    });

using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record StringLexerFixture() : LexerFixture(
    """
    "foobar"
    "foo bar"
    """,
    new List<Token>
    {
        new(Token.String, "foobar"),
        new(Token.String, "foo bar"),
        new(Token.Eof)
    });
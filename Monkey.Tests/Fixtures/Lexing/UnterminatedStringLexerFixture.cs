using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record UnterminatedStringLexerFixture() : LexerFixture(
    """
    "foo
    """,
    new List<Token>
    {
        new(Token.String, "foo"),
        new(Token.Eof)
    });

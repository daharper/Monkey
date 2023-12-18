using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record HashLexerFixture() : LexerFixture(
    """{"foo": "bar"}""",
    new List<Token>
    {
        new(Token.LBrace, "{"),
        new(Token.String, "foo"),
        new(Token.Colon, ":"),
        new(Token.String, "bar"),
        new(Token.RBrace, "}"),
        new(Token.Eof)
    });
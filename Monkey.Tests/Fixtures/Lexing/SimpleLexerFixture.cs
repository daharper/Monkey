using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record SimpleLexerFixture() : LexerFixture(
    "=+(){},;",
    new List<Token>
    {
        new(Token.Assign, "="),
        new(Token.Plus, "+"),
        new(Token.LParen, "("),
        new(Token.RParen, ")"),
        new(Token.LBrace, "{"),
        new(Token.RBrace, "}"),
        new(Token.Comma, ","),
        new(Token.Semicolon, ";"),
        new(Token.Eof)
    });

using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record ArrayLexerFixture() : LexerFixture(
    "[1, 2];",
    new List<Token>
    {
        new(Token.LBracket, "["),
        new(Token.Int, "1"),
        new(Token.Comma, ","),
        new(Token.Int, "2"),
        new(Token.RBracket, "]"),
        new(Token.Semicolon, ";"),
        new(Token.Eof)
    });
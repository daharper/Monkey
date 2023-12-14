using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record ComplexLexerFixture() : LexerFixture(
    """
    let five = 5;
    let ten = 10;

    let add = fn(x, y) {
        x + y;
    };

    let result = add(five, ten);
    """,
    new List<Token>
    {
        new(Token.Let, "let"),
        new(Token.Identifier, "five"),
        new(Token.Assign, "="),
        new(Token.Int, "5"),
        new(Token.Semicolon, ";"),
        new(Token.Let, "let"),
        new(Token.Identifier, "ten"),
        new(Token.Assign, "="),
        new(Token.Int, "10"),
        new(Token.Semicolon, ";"),
        new(Token.Let, "let"),
        new(Token.Identifier, "add"),
        new(Token.Assign, "="),
        new(Token.Function, "fn"),
        new(Token.LParen, "("),
        new(Token.Identifier, "x"),
        new(Token.Comma, ","),
        new(Token.Identifier, "y"),
        new(Token.RParen, ")"),
        new(Token.LBrace, "{"),
        new(Token.Identifier, "x"),
        new(Token.Plus, "+"),
        new(Token.Identifier, "y"),
        new(Token.Semicolon, ";"),
        new(Token.RBrace, "}"),
        new(Token.Semicolon, ";"),
        new(Token.Let, "let"),
        new(Token.Identifier, "result"),
        new(Token.Assign, "="),
        new(Token.Identifier, "add"),
        new(Token.LParen, "("),
        new(Token.Identifier, "five"),
        new(Token.Comma, ","),
        new(Token.Identifier, "ten"),
        new(Token.RParen, ")"),
        new(Token.Semicolon, ";"),
        new(Token.Eof)
    });


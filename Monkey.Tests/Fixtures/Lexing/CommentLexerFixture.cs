using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record CommentLexerFixture() : LexerFixture(
    """
    # leading comment
    let x = 5; # trailing comment
      # indented comment
    "a # not a comment"
    #
    """,
    new List<Token>
    {
        new(Token.Let, "let"),
        new(Token.Identifier, "x"),
        new(Token.Assign, "="),
        new(Token.Int, "5"),
        new(Token.Semicolon, ";"),
        new(Token.String, "a # not a comment"),
        new(Token.Eof)
    });

namespace Monkey.Tests;

using Monkey;

public class Tests
{
    #region private
    
    private record TokenTest(string Input, IEnumerable<Token> Tests);
    
    private static readonly TokenTest InitialTest = new TokenTest(
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

    private static readonly TokenTest SecondTest = new TokenTest(
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
    
    private static readonly TokenTest ThirdTest = new TokenTest(
        """
        let five = 5;
        let ten = 10;
        
        let add = fn(x, y) {
          x + y;
        };
        
        let result = add(five, ten);
        !-/*5;
        5 < 10 > 5;
        
        if (5 < 10) {
        	return true;
        } else {
        	return false;
        }
        
        10 == 10;
        10 != 9;
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
            new(Token.Bang, "!"),
            new(Token.Minus, "-"),
            new(Token.Slash, "/"),
            new(Token.Asterisk, "*"),
            new(Token.Int, "5"),
            new(Token.Semicolon, ";"),
            new(Token.Int, "5"),
            new(Token.Lt, "<"),
            new(Token.Int, "10"),
            new(Token.Gt, ">"),
            new(Token.Int, "5"),
            new(Token.Semicolon, ";"),
            new(Token.If, "if"),
            new(Token.LParen, "("),
            new(Token.Int, "5"),
            new(Token.Lt, "<"),
            new(Token.Int, "10"),
            new(Token.RParen, ")"),
            new(Token.LBrace, "{"),
            new(Token.Return, "return"),
            new(Token.True, "true"),
            new(Token.Semicolon, ";"),
            new(Token.RBrace, "}"),
            new(Token.Else, "else"),
            new(Token.LBrace, "{"),
            new(Token.Return, "return"),
            new(Token.False, "false"),
            new(Token.Semicolon, ";"),
            new(Token.RBrace, "}"),
            new(Token.Int, "10"),
            new(Token.Eq, "=="),
            new(Token.Int, "10"),
            new(Token.Semicolon, ";"),
            new(Token.Int, "10"),
            new(Token.NotEq, "!="),
            new(Token.Int, "9"),
            new(Token.Semicolon, ";"),
            new(Token.Eof)
        });
    
    #endregion
    
    [Test]
    public void TestNextToken()
    {
        TestToken(InitialTest);
        TestToken(SecondTest);
        TestToken(ThirdTest);
    }
    
    #region private methods
    
    private void TestToken(TokenTest test)
    {
        var lexer = new Lexer(test.Input);

        test.Tests.ForEach((i, tt) => {
            var tok = lexer.NextToken();

            Assert.Multiple(() =>
            {
                Assert.That(
                    tok.Type, Is.EqualTo(tt.Type),
                    $"tests[{i}] - tokentype wrong. expected={tt.Type}, got={tok.Type}");

                Assert.That(tok.Literal, Is.EqualTo(tt.Literal),
                    $"tests[{i}] - literal wrong. expected={tt.Literal}, got={tok.Literal}");
            });
        });
    }
    
    #endregion
}
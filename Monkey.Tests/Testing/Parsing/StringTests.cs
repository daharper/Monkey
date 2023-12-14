using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Statements;

namespace Monkey.Tests.Testing.Parsing;

public class StringTests : BaseParserTest
{
    [Test]
    public void TestToString()
    {
        var programme = new Programme()
        {
            Statements =
            [
                new LetStatement
                {
                    Token = new Token(Token.Let, "let"),
                    Name = new IdentifierExpression
                    {
                        Token = new Token(Token.Identifier, "myVar"),
                        Value = "myVar"
                    },
                    Value = new IdentifierExpression
                    {
                        Token = new Token(Token.Identifier, "anotherVar"),
                        Value = "anotherVar"
                    }
                }
            ]
        };
        
        Assert.AreEqual("let myVar = anotherVar;", programme.ToString(),
            $"expected 'let myVar = anotherVar;' got '{programme}'");
    }
}
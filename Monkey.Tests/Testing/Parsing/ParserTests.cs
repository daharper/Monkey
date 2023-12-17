using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Literals;
using Monkey.Parsing.Statements;

namespace Monkey.Tests.Testing.Parsing;

public class ParserTests : ParsingTestBase
{
    [Test]
    public void TestErrorReporting()
    {
        var lexer = new Lexer( "let x 5; let = 10; let 838383;");
        var parser = new Parser(lexer);
        
        parser.ParseProgramme();

        var result = Assert.Throws<AssertionException>(() => CheckErrors(parser));
        Console.WriteLine(result?.Message);
    }
    
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
                    Name = new Identifier
                    {
                        Token = new Token(Token.Identifier, "myVar"),
                        Value = "myVar"
                    },
                    Value = new Identifier
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
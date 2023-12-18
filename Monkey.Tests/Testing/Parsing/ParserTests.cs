using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

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
        var programme = Programme.Create(
        [
            new LetNode(new Token(Token.Let, "let"))
            {
                Name = new IdentifierNode(token: new Token(Token.Identifier, "myVar"), value: "myVar"),
                Value = new IdentifierNode(token: new Token(Token.Identifier, "anotherVar"), value: "anotherVar")
            }
        ]);
        
        Assert.That(programme.ToString(), Is.EqualTo("let myVar = anotherVar;"),
            $"expected 'let myVar = anotherVar;' got '{programme}'");
    }
}
using Monkey.Lexing;
using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public class ErrorTests : ParsingTestBase
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
}
using Monkey.Lexing;
using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public class ErrorTests : ParsingTestBase
{
    [TestCase("let x 5;")]
    [TestCase("let = 10;")]
    [TestCase("let 838383;")]
    [TestCase("fn(1) { 1 }")]
    [TestCase("fn(x, 2) { x }")]
    public void TestErrorReporting(string input)
    {
        Assert.Throws<ParserException>(() =>
        {
            var parser = new Parser(new Lexer(input));
            parser.ParseProgram();
        });
    }
}
using Monkey.Lexing;
using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public class ErrorTests : ParsingTestBase
{
    [TestCase("let x 5;")]
    [TestCase("let y = 10;")]
    [TestCase("let 838383;")]
    public void TestErrorReporting(string input)
    {
        Assert.Throws<InvalidProgramException>(() =>
        {
            var parser = new Parser(new Lexer("let x 5;"));
            parser.ParseProgram();
        });
    }
}
using Monkey.Lexing;
using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public abstract class ParsingTestBase : TestBase
{
    protected Programme AssertParse(string input, int expectedStatements)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);        
        
        Assert.AreEqual(expectedStatements, programme.Statements.Count, 
            $"expected {expectedStatements} got {programme.Statements.Count}");
        
        return programme;
    }
}
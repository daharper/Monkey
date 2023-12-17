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
        
        Assert.That(programme.Statements.Count, Is.EqualTo(expectedStatements), 
            $"expected {expectedStatements} got {programme.Statements.Count}");
        
        return programme;
    }
}
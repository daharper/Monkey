using Monkey.Lexing;
using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public abstract class TestBase
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
    
    protected T AssertCast<T>(object obj)
    {
        Assert.IsInstanceOf<T>(obj, $"expected {typeof(T).Name} got {obj.GetType().Name}");
        return (T)obj;
    }
    
    protected void CheckErrors(Parser parser)
    {
        Assert.IsFalse(
            parser.HasErrors, 
            $"parser has the following errors:{Environment.NewLine}{string.Join(Environment.NewLine, parser.Errors)}");
    }
}
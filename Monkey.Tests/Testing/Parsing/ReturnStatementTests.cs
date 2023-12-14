using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Statements;
using Monkey.Utils;

namespace Monkey.Tests.Testing.Parsing;

public class ReturnStatementTests : BaseParserTest
{
    [Test]
    public void TestReturnStatement()
    {
        const string input = "return 5; return 10; return 993322;";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        
        var programme = parser.ParseProgramme();
        CheckErrors(parser);
        
        Assert.AreEqual(3, programme.Statements.Count);
        
        programme.Statements.ForEach((i, statement) =>
        {
            Assert.Multiple(() =>
            {
                Assert.AreEqual(statement.TokenLiteral(), "return", 
                    $"expected 'return' got '{statement.TokenLiteral()}'");
                
                Assert.IsTrue(statement is ReturnStatement, 
                    $"expected 'ReturnStatement' got '{statement.GetType().Name}'");
            });
        });
    }
}
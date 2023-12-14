using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Utils;

namespace Monkey.Tests.Testing.Parsing;

public class LetStatementTests : BaseParserTest
{
    [Test]
    public void TestLetStatements()
    {
        List<string> identifiers = ["x", "y", "foobar"];
        
        var lexer = new Lexer("let x = 5; let y = 10; let foobar = 838383;");
        var parser = new Parser(lexer);
        
        var programme = parser.ParseProgramme();
        CheckErrors(parser);
        
        Assert.AreEqual(identifiers.Count, programme.Statements.Count);
        
        identifiers.ForEach((i, identifier) =>
        {
            var statement = programme.Statements[i];
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(statement.TokenLiteral(), "let", 
                    $"expected 'let' got '{statement.TokenLiteral()}'");
                
                Assert.IsTrue(statement is LetStatement, 
                    $"expected 'LetStatement' got '{statement.GetType().Name}'");
                
                var letStatement = (LetStatement) statement;
                
                Assert.AreEqual(letStatement.Name.Value, identifier,
                    $"expected '{identifier}' got '{letStatement.Name.Value}'");
                
                Assert.AreEqual(letStatement.Name.TokenLiteral(), identifier,
                    $"expected '{identifier}' got '{letStatement.Name.TokenLiteral()}'");
            });
        });
    }
}
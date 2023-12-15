using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Statements;
using Monkey.Utils;

namespace Monkey.Tests.Testing.Parsing;

public class StatementTests : TestBase
{
    [Test]
    public void TestLetStatements()
    {
        var tests = new List<(string input, string identifier, object value)>
        {
            ("let x = 5;", "x", 5),
            ("let y = true;", "y", "true"),
            ("let foobar = y;", "foobar", "y")
        };
        
        tests.ForEach((i, test) =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var programme = parser.ParseProgramme();
            
            CheckErrors(parser);
            
            Assert.AreEqual(1, programme.Statements.Count);
            
            var statement = programme.Statements[0];
            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(statement.TokenLiteral(), "let", 
                    $"expected 'let' got '{statement.TokenLiteral()}'");
                
                Assert.IsTrue(statement is LetStatement, 
                    $"expected 'LetStatement' got '{statement.GetType().Name}'");
                
                var letStatement = (LetStatement) statement;
                
                Assert.AreEqual(letStatement.Name.Value, test.identifier,
                    $"expected '{test.identifier}' got '{letStatement.Name.Value}'");
                
                Assert.AreEqual(letStatement.Name.TokenLiteral(), test.identifier,
                    $"expected '{test.identifier}' got '{letStatement.Name.TokenLiteral()}'");
                
                Assert.AreEqual(letStatement.Value.TokenLiteral(), test.value.ToString(),
                    $"expected '{test.value}' got '{letStatement.Value.TokenLiteral()}'");
            });
        });
    }
    
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
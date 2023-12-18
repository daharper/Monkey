using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class StatementTests : ParsingTestBase
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
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            
            CheckErrors(parser);
            
            Assert.That(program.Statements, Has.Count.EqualTo(1));
            
            var statement = program.Statements[0];
            
            Assert.Multiple(() =>
            {
                Assert.That(statement.TokenLiteral(), Is.EqualTo("let"), 
                    $"expected 'let' got '{statement.TokenLiteral()}'");
                
                Assert.That(statement is LetNode, Is.True, 
                    $"expected 'LetStatement' got '{statement.GetType().Name}'");
                
                var letStatement = (LetNode) statement;
                
                Assert.That(test.identifier, Is.EqualTo(letStatement.Name.Value),
                    $"expected '{test.identifier}' got '{letStatement.Name.Value}'");
                
                Assert.That(test.identifier, Is.EqualTo(letStatement.Name.TokenLiteral()),
                    $"expected '{test.identifier}' got '{letStatement.Name.TokenLiteral()}'");
                
                Assert.That(test.value.ToString(), Is.EqualTo(letStatement.Value.TokenLiteral()),
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
        var program = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(program.Statements, Has.Count.EqualTo(3));
        
        program.Statements.ForEach(statement =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(statement.TokenLiteral(), Is.EqualTo("return"), 
                    $"expected 'return' got '{statement.TokenLiteral()}'");
                
                Assert.That(statement is ReturnNode, Is.True, 
                    $"expected 'ReturnStatement' got '{statement.GetType().Name}'");
            });
        });
    }
}
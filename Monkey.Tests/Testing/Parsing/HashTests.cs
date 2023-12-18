using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class HashTests : ParsingTestBase
{
    [Test]
    public void TestHashParsing()
    {
        const string input = """
                             {"one" : 1, "two" : 2, "three" : 3}
                             """;
        
        var programme = AssertParse(input, 1);
        var statement = AssertCast<ExpressionNode>(programme.Statements[0]);
        var hash = AssertCast<HashNode>(statement.Expression);

        Assert.That(hash.Pairs, Has.Count.EqualTo(3),
            $"expected 3 pairs in hash got {hash.Pairs.Count}");

        var expected = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };
        
        foreach (var (key, value) in hash.Pairs)
        {
            
            var keyLiteral = AssertCast<StringNode>(key);
            var valueLiteral = AssertCast<IntegerNode>(value);
            var expectedValue = expected[keyLiteral.Value];
            
            Assert.That(valueLiteral.Value, Is.EqualTo(expectedValue),
                $"expected value to be {expectedValue} got {valueLiteral.Value}");
        }
    }
    
    [Test]
    public void TestEmptyHashParsing()
    {
        const string input = "{}";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        var statement = AssertCast<ExpressionNode>(programme.Statements[0]);
        var hash = AssertCast<HashNode>(statement.Expression);

        Assert.That(hash.Pairs, Is.Empty,
            $"expected 0 pairs in hash got {hash.Pairs.Count}");
    }
    
    [Test]
    public void TestHashParsingWithExpressions()
    {
        const string input = """
                             {"one" : 0 + 1, "two" : 10 - 8, "three" : 15 / 5}
                             """;

        var programme = AssertParse(input, 1);
        var statement = AssertCast<ExpressionNode>(programme.Statements[0]);
        var hash = AssertCast<HashNode>(statement.Expression);

        Assert.That(hash.Pairs, Has.Count.EqualTo(3),
            $"expected 3 pairs in hash got {hash.Pairs.Count}");

        var tests = new Dictionary<string, Action<Node>>
        {
             ["one"] = expression => TestInfixExpression(expression, 0, "+", 1),
             ["two"] = expression => TestInfixExpression(expression, 10, "-", 8),
             ["three"] = expression => TestInfixExpression(expression, 15, "/", 5)
        };
        
        foreach (var (key, value) in hash.Pairs)
        {
            var keyLiteral = AssertCast<StringNode>(key);
            var test = tests[keyLiteral.Value];
            
            test(value);
        }
    }
}
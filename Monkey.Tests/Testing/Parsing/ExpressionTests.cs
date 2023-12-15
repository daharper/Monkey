using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Statements;

namespace Monkey.Tests.Testing.Parsing;

public class ExpressionTests : TestBase
{
    [Test]
    public void TestIdentifierExpression()
    {
        var programme = AssertParse("foobar", 1);
        var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
        var expression = AssertCast<IdentifierExpression>(statement.Expression!);

        Assert.AreEqual("foobar", expression.Value, $"expected 'foobar' got '{expression.Value}'");
        Assert.AreEqual("foobar", expression.TokenLiteral(), $"expected 'foobar' got '{expression.TokenLiteral()}'");
    }
    
    [Test]
    public void TestIntegerExpression()
    {
        var programme = AssertParse("5", 1);
        var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
        var expression = AssertCast<IntegerExpression>(statement.Expression!);

        Assert.AreEqual(5, expression.Value, $"expected '5' got '{expression.Value}'");
        Assert.AreEqual("5", expression.TokenLiteral(), $"expected '5' got '{expression.TokenLiteral()}'");
    }
    
    [Test]
    public void TestPrefixExpression()
    {
        var tests = new List<(string input, string op, object value)>
        {
            ("!5;", "!", 5),
            ("-15;", "-", 15),
            ("!true;", "!", true),
            ("!false;", "!", false)
        };

        tests.ForEach(test =>
        {
            var programme = AssertParse(test.input, 1);
            var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
            var expression = AssertCast<PrefixExpression>(statement.Expression!);

            Assert.AreEqual(test.op, expression.Operator, $"expected '{test.op}' got '{expression.Operator}'");
            
            dynamic right = expression.Right is IntegerExpression
                ? AssertCast<IntegerExpression>(expression.Right)
                : AssertCast<BooleanExpression>(expression.Right);

            Assert.AreEqual(test.value, right.Value, $"expected '{test.value}' got '{right.Value}'");
        });
    }

    [Test]
    public void TestInfixExpression()
    {
        var tests = new List<(string input, object leftValue, string op, object rightValue)>
        {
            ("5 + 5;", 5, "+", 5),
            ("5 - 5;", 5, "-", 5),
            ("5 * 5;", 5, "*", 5),
            ("5 / 5;", 5, "/", 5),
            ("5 > 5;", 5, ">", 5),
            ("5 < 5;", 5, "<", 5),
            ("5 == 5;", 5, "==", 5),
            ("5 != 5;", 5, "!=", 5),
            ("true == true", true, "==", true),
            ("true != false", true, "!=", false),
            ("false == false", false, "==", false)
        };
        
        tests.ForEach(test =>
        {
            var programme = AssertParse(test.input, 1);
            var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
            var expression = AssertCast<InfixExpression>(statement.Expression!);

            Assert.AreEqual(test.op, expression.Operator, $"expected '{test.op}' got '{expression.Operator}'");

            dynamic left = expression.Left is IntegerExpression
                ? AssertCast<IntegerExpression>(expression.Left)
                : AssertCast<BooleanExpression>(expression.Left);
            
            Assert.AreEqual(test.leftValue, left.Value, $"expected '{test.leftValue}' got '{left.Value}'");
            
            dynamic right = expression.Right is IntegerExpression
                ? AssertCast<IntegerExpression>(expression.Right)
                : AssertCast<BooleanExpression>(expression.Right);
            
            Assert.AreEqual(test.rightValue, right.Value, $"expected '{test.rightValue}' got '{right.Value}'");
        });
    }

    [Test]
    public void TestOperatorPrecedence()
    {
        var tests = new List<(string input, string expected)>
        {
            ("-a * b", "((-a) * b)"),
            ("!-a", "(!(-a))"),
            ("a + b + c", "((a + b) + c)"),
            ("a + b - c", "((a + b) - c)"),
            ("a * b * c", "((a * b) * c)"),
            ("a * b / c", "((a * b) / c)"),
            ("a + b / c", "(a + (b / c))"),
            ("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
            ("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"),
            ("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
            ("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"),
            ("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            ("true", "true"),
            ("false", "false"),
            ("3 > 5 == false", "((3 > 5) == false)"),
            ("3 < 5 == true", "((3 < 5) == true)"),
            ("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            ("(5 + 5) * 2", "((5 + 5) * 2)"),
            ("2 / (5 + 5)", "(2 / (5 + 5))"),
            ("-(5 + 5)", "(-(5 + 5))"),
            ("!(true == true)", "(!(true == true))"),
            // ("a + add(b * c) + d", "((a + add((b * c))) + d)"),
            // ("add(a, b, 1, 2 * 3, 4 + 5, add(6,
        };
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var programme = parser.ParseProgramme();

            CheckErrors(parser);
            
            var actual = programme.ToString();
            
            Assert.AreEqual(test.expected, actual, $"expected '{test.expected}' got '{programme}'");
        });
    }
    
    [Test]
    public void TestBooleanExpression()
    {
        var tests = new List<(string input, string expected)>
        {
            ("true", "true"),
            ("false", "false"),
            ("3 > 5 == false", "((3 > 5) == false)"),
            ("3 < 5 == true", "((3 < 5) == true)"),
            // ("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            // ("(5 + 5) * 2", "((5 + 5) * 2)"),
            // ("2 / (5 + 5)", "(2 / (5 + 5))"),
            // ("-(5 + 5)", "(-(5 + 5))"),
            // ("!(true == true)", "(!(true == true))"),
        };
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var programme = parser.ParseProgramme();

            CheckErrors(parser);
            
            var actual = programme.ToString();
            
            Assert.AreEqual(test.expected, actual, $"expected '{test.expected}' got '{actual}'");
        });
    }

    [Test]
    public void TestIfExpression()
    {
        const string input = "if (x < y) { x }";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        Assert.AreEqual(1, programme.Statements.Count);
        
        var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
        var expression = AssertCast<IfExpression>(statement.Expression!);
        
        Assert.AreEqual("if", expression.TokenLiteral(), $"expected 'if' got '{expression.TokenLiteral()}'");
        Assert.AreEqual("(x < y)", expression.Condition.ToString(), $"expected '(x < y)' got '{expression.Condition}'");
        
        Assert.AreEqual(1, expression.Consequence.Statements.Count, 
            $"expected 1 statement in consequence got {expression.Consequence.Statements.Count}");
        
        var consequence = AssertCast<ExpressionStatement>(expression.Consequence.Statements[0]);
        Assert.AreEqual("x", consequence.Expression!.ToString(), $"expected 'x' got '{consequence.Expression}'");
        
        Assert.IsNull(expression.Alternative, "expected 'null' got 'not null'");
    }
    
    [Test]
    public void TestIfElseExpression()
    {
        const string input = "if (x < y) { x } else { y }";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        Assert.AreEqual(1, programme.Statements.Count);
        
        var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
        var expression = AssertCast<IfExpression>(statement.Expression!);
        
        Assert.AreEqual("if", expression.TokenLiteral(), $"expected 'if' got '{expression.TokenLiteral()}'");
        Assert.AreEqual("(x < y)", expression.Condition.ToString(), $"expected '(x < y)' got '{expression.Condition}'");
        
        Assert.AreEqual(1, expression.Consequence.Statements.Count, 
            $"expected 1 statement in consequence got {expression.Consequence.Statements.Count}");
        
        var consequence = AssertCast<ExpressionStatement>(expression.Consequence.Statements[0]);
        Assert.AreEqual("x", consequence.Expression!.ToString(), $"expected 'x' got '{consequence.Expression}'");
        
        Assert.AreEqual(1, expression.Alternative!.Statements.Count, 
            $"expected 1 statement in alternative got {expression.Alternative!.Statements.Count}");
        
        var alternative = AssertCast<ExpressionStatement>(expression.Alternative!.Statements[0]);
        
        Assert.AreEqual("y", alternative.Expression!.ToString(), $"expected 'y' got '{alternative.Expression}'");
    }
}
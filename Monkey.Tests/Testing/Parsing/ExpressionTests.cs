using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class ExpressionTests : ParsingTestBase
{
    [Test]
    public void TestIdentifierExpression()
    {
        var program = AssertParse("foobar", 1);
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var expression = AssertCast<IdentifierNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.Value, Is.EqualTo("foobar"), 
                $"expected 'foobar' got '{expression.Value}'");
            
            Assert.That(expression.TokenLiteral(), Is.EqualTo("foobar"), 
                $"expected 'foobar' got '{expression.TokenLiteral()}'");
        });
    }

    [Test]
    public void TestIntegerExpression()
    {
        var program = AssertParse("5", 1);
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var expression = AssertCast<IntegerNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.Value, Is.EqualTo(5), 
                $"expected '5' got '{expression.Value}'");
            
            Assert.That(expression.TokenLiteral(), Is.EqualTo("5"), 
                $"expected '5' got '{expression.TokenLiteral()}'");
        });
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
            var program = AssertParse(test.input, 1);
            var statement = AssertCast<ExpressionNode>(program.Statements[0]);
            var expression = AssertCast<PrefixNode>(statement.Expression);

            Assert.That(expression.Operator, Is.EqualTo(test.op), 
                $"expected '{test.op}' got '{expression.Operator}'");
            
            dynamic right = expression.Right is IntegerNode
                ? AssertCast<IntegerNode>(expression.Right)
                : AssertCast<BooleanNode>(expression.Right);

            Assert.AreEqual(test.value, right.Value, 
                $"expected '{test.value}' got '{right.Value}'");
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
            var program = AssertParse(test.input, 1);
            var statement = AssertCast<ExpressionNode>(program.Statements[0]);
            var expression = AssertCast<InfixNode>(statement.Expression);

            Assert.That(expression.Operator, Is.EqualTo(test.op), 
                $"expected '{test.op}' got '{expression.Operator}'");

            dynamic left = expression.Left is IntegerNode
                ? AssertCast<IntegerNode>(expression.Left)
                : AssertCast<BooleanNode>(expression.Left);
            
            Assert.AreEqual(test.leftValue, left.Value, 
                $"expected '{test.leftValue}' got '{left.Value}'");
            
            dynamic right = expression.Right is IntegerNode
                ? AssertCast<IntegerNode>(expression.Right)
                : AssertCast<BooleanNode>(expression.Right);
            
            Assert.AreEqual(test.rightValue, right.Value,
                $"expected '{test.rightValue}' got '{right.Value}'");
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
            ("a + add(b * c) + d", "((a + add((b * c))) + d)"),
            ("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"),
            ("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))"),
            ("a * [1, 2, 3, 4][b * c] * d", "((a * ([1, 2, 3, 4][(b * c)])) * d)"),
            ("add(a * b[2], b[1], 2 * [1, 2][1])", "add((a * (b[2])), (b[1]), (2 * ([1, 2][1])))")
        };
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            CheckErrors(parser);
            
            var actual = program.ToString();
            
            Assert.That(actual, Is.EqualTo(test.expected), 
                $"expected '{test.expected}' got '{actual}'");
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
            ("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            ("(5 + 5) * 2", "((5 + 5) * 2)"),
            ("2 / (5 + 5)", "(2 / (5 + 5))"),
            ("-(5 + 5)", "(-(5 + 5))"),
            ("!(true == true)", "(!(true == true))"),
        };
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            CheckErrors(parser);
            
            var actual = program.ToString();
            
            Assert.That(actual, Is.EqualTo(test.expected), 
                $"expected '{test.expected}' got '{actual}'");
        });
    }

    [Test]
    public void TestIfExpression()
    {
        const string input = "if (x < y) { x }";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var expression = AssertCast<IfNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.TokenLiteral(), Is.EqualTo("if"), 
                $"expected 'if' got '{expression.TokenLiteral()}'");
            
            Assert.That(expression.Condition.ToString(), Is.EqualTo("(x < y)"), 
                $"expected '(x < y)' got '{expression.Condition}'");

            Assert.That(expression.Consequence.Statements.Count, Is.EqualTo(1),
                $"expected 1 statement in consequence got {expression.Consequence.Statements.Count}");
        });
        
        var consequence = AssertCast<ExpressionNode>(expression.Consequence.Statements[0]);
        
        Assert.Multiple(() =>
        {
            Assert.That(consequence.Expression.ToString(), Is.EqualTo("x"), 
                $"expected 'x' got '{consequence.Expression}'");
            
            Assert.That(expression.Alternative, Is.Null, 
                "expected 'null' got 'not null'");
        });
    }

    [Test]
    public void TestIfElseExpression()
    {
        const string input = "if (x < y) { x } else { y }";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(program.Statements.Count, Is.EqualTo(1));
        
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var expression = AssertCast<IfNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.TokenLiteral(), Is.EqualTo("if"),
                $"expected 'if' got '{expression.TokenLiteral()}'");

            Assert.That(expression.Condition.ToString(), Is.EqualTo("(x < y)"),
                $"expected '(x < y)' got '{expression.Condition}'");

            Assert.That(expression.Consequence.Statements, Has.Count.EqualTo(1),
                $"expected 1 statement in consequence got {expression.Consequence.Statements.Count}");
        });
        
        var consequence = AssertCast<ExpressionNode>(expression.Consequence.Statements[0]);
        
        Assert.Multiple(() =>
        {
            Assert.That(consequence.Expression.ToString(), Is.EqualTo("x"),
                $"expected 'x' got '{consequence.Expression}'");

            Assert.That(expression.Alternative!.Statements, Has.Count.EqualTo(1),
                $"expected 1 statement in alternative got {expression.Alternative!.Statements.Count}");
        });
        
        var alternative = AssertCast<ExpressionNode>(expression.Alternative!.Statements[0]);
        
        Assert.That(alternative.Expression.ToString(), Is.EqualTo("y"), 
            $"expected 'y' got '{alternative.Expression}'");
    }

    [Test]
    public void TestFunctionParsing()
    {
        const string input = "fn(x, y) { x + y; }";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var function = AssertCast<FunctionNode>(statement.Expression);
        
        Assert.That(function.Parameters, Has.Count.EqualTo(2), 
            $"expected 2 parameters got {function.Parameters.Count}");
        
        Assert.Multiple(() =>
        {
            Assert.That(function.Parameters[0].ToString(), Is.EqualTo("x"),
                $"expected 'x' got '{function.Parameters[0]}'");

            Assert.That(function.Parameters[1].ToString(), Is.EqualTo("y"),
                $"expected 'y' got '{function.Parameters[1]}'");

            Assert.That(function.Body.Statements, Has.Count.EqualTo(1),
                $"expected 1 statement in body got {function.Body.Statements.Count}");
        });
        
        var body = AssertCast<ExpressionNode>(function.Body.Statements[0]);
        
        Assert.That(body.Expression.ToString(), Is.EqualTo("(x + y)"), 
            $"expected '(x + y)' got '{body.Expression}'");
    }

    [Test]
    public void TestFunctionParameterParsing()
    {
        var tests = new List<(string input, string expectedParams)>()
        {
            ("fn() {};", ""),
            ("fn(x) {};", "x"),
            ("fn(x, y, z) {};", "x, y, z")
        };
        
        tests.ForEach(test =>
        {
            var lexer = new Lexer(test.input);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            
            CheckErrors(parser);
            
            var statement = AssertCast<ExpressionNode>(program.Statements[0]);
            var function = AssertCast<FunctionNode>(statement.Expression);
            var parameters = string.Join(", ", function.Parameters.Select(p => p.ToString()));

            Assert.That(parameters, Is.EqualTo(test.expectedParams), 
                $"expected '{test.expectedParams}' got '{parameters}'");
        });
    }

    [Test]
    public void TestCallExpressionParsing()
    {
        const string input = "add(1, 2 * 3, 4 + 5);";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(program.Statements, Has.Count.EqualTo(1));
        
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        var expression = AssertCast<CallNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.Function.ToString(), Is.EqualTo("add"), 
                $"expected 'add' got '{expression.Function}'");
            
            Assert.That(expression.Arguments, Has.Count.EqualTo(3), 
                $"expected 3 arguments got {expression.Arguments.Count}");
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.Arguments[0].ToString(), Is.EqualTo("1"), 
                $"expected '1' got '{expression.Arguments[0]}'");
            
            Assert.That(expression.Arguments[1].ToString(), Is.EqualTo("(2 * 3)"), 
                $"expected '(2 * 3)' got '{expression.Arguments[1]}'");
            
            Assert.That(expression.Arguments[2].ToString(), Is.EqualTo("(4 + 5)"), 
                $"expected '(4 + 5)' got '{expression.Arguments[2]}'");
        });
    }
    
    [Test]
    public void TestParsingIndexExpressions()
    {
        const string input = "myArray[1 + 1]";
        
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgram();
        
        CheckErrors(parser);
        
        Assert.That(programme.Statements, Has.Count.EqualTo(1));
        
        var statement = AssertCast<ExpressionNode>(programme.Statements[0]);
        var expression = AssertCast<IndexNode>(statement.Expression);
        
        Assert.Multiple(() =>
        {
            Assert.That(expression.Left.ToString(), Is.EqualTo("myArray"), 
                $"expected 'myArray' got '{expression.Left}'");
            
            Assert.That(expression.Index.ToString(), Is.EqualTo("(1 + 1)"), 
                $"expected '(1 + 1)' got '{expression.Index}'");
        });
    }
}
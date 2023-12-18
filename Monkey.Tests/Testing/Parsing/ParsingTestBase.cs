using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public abstract class ParsingTestBase : TestBase
{
    protected static ProgramNode AssertParse(string input, int expectedStatementsCount)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgramme();
        
        CheckErrors(parser);        
        
        Assert.That(program.Statements, Has.Count.EqualTo(expectedStatementsCount), 
            $"expected {expectedStatementsCount} got {program.Statements.Count}");
        
        return program;
    }
    
    protected static void TestInfixExpression(Node expression, object left, string op, object right)
    {
        var infix = AssertCast<InfixNode>(expression);
        
        Assert.That(infix.Operator, Is.EqualTo(op), $"expected '{op}' got '{infix.Operator}'");

        dynamic l = infix.Left is IntegerNode
            ? AssertCast<IntegerNode>(infix.Left)
            : AssertCast<BooleanNode>(infix.Left);
            
        Assert.AreEqual(left, l.Value, $"expected '{left}' got '{l.Value}'");
            
        dynamic r = infix.Right is IntegerNode
            ? AssertCast<IntegerNode>(infix.Right)
            : AssertCast<BooleanNode>(infix.Right);
            
        Assert.AreEqual(right, r.Value, $"expected '{right}' got '{r.Value}'");
    }
}
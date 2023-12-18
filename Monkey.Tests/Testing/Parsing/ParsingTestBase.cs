using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

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
    
    protected void TestInfixExpression(INode expression, object leftValue, string op, object rightValue)
    {
        var infix = AssertCast<InfixExpression>(expression);
        
        Assert.That(infix.Operator, Is.EqualTo(op), $"expected '{op}' got '{infix.Operator}'");

        dynamic left = infix.Left is IntegerLiteral
            ? AssertCast<IntegerLiteral>(infix.Left)
            : AssertCast<BooleanLiteral>(infix.Left);
            
        Assert.AreEqual(leftValue, left.Value, $"expected '{leftValue}' got '{left.Value}'");
            
        dynamic right = infix.Right is IntegerLiteral
            ? AssertCast<IntegerLiteral>(infix.Right)
            : AssertCast<BooleanLiteral>(infix.Right);
            
        Assert.AreEqual(rightValue, right.Value, $"expected '{rightValue}' got '{right.Value}'");
    }
}
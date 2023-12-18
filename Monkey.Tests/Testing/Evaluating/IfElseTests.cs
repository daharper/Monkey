namespace Monkey.Tests.Testing.Evaluating;

public class IfElseTests : EvaluatingTestBase
{
    [TestCase("if (true) { 10 }", 10)]
    [TestCase("if (false) { 10 }", null)]
    [TestCase("if (1) { 10 }", 10)]
    [TestCase("if (1 < 2) { 10 }", 10)]
    [TestCase("if (1 > 2) { 10 }", null)]
    [TestCase("if (1 > 2) { 10 } else { 20 }", 20)]
    [TestCase("if (1 < 2) { 10 } else { 20 }", 10)]
    public void TestIfElseExpressions(string input, object expected)
    {
        var evaluated = TestEval(input);
        
        if (expected is int expectedInt)
        {
            TestInteger(evaluated, expectedInt);
        }
        else
        {
            TestNull(evaluated);
        }
    }
}
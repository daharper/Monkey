namespace Monkey.Tests.Testing.Evaluating;

public class ReturnStatementTests : EvaluatingTestBase
{
    [TestCase("return 10;", 10)]
    [TestCase("return 10; 9;", 10)]
    [TestCase("return 2 * 5; 9;", 10)]
    [TestCase("9; return 2 * 5; 9;", 10)]
    [TestCase("if (10 > 1) { if (10 > 1) { return 10; } return 1; }", 10)]
    public void TestReturnStatements(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestInteger(evaluated, expected);
    }
}
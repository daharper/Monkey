namespace Monkey.Tests.Testing.Evaluating;

public class IntegerTests : EvaluatingTestBase
{
    [TestCase("5", 5)]
    [TestCase("10", 10)]
    [TestCase("-5", -5)]
    [TestCase("-10", -10)]
    [TestCase("5 + 5 + 5 + 5 - 10", 10)]
    [TestCase("2 * 2 * 2 * 2 * 2", 32)]
    [TestCase("-50 + 100 + -50", 0)]
    [TestCase("5 * 2 + 10", 20)]
    [TestCase("5 + 2 * 10", 25)]
    [TestCase("20 + 2 * -10", 0)]
    [TestCase("50 / 2 * 2 + 10", 60)]
    [TestCase("2 * (5 + 10)", 30)]
    [TestCase("3 * 3 * 3 + 10", 37)]
    [TestCase("3 * (3 * 3) + 10", 37)]
    [TestCase("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50)]
    public void TestEvalIntegerExpressions(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestInteger(evaluated, expected);
    }
}
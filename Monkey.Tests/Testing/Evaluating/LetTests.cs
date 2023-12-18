namespace Monkey.Tests.Testing.Evaluating;

public class LetTests : EvaluatingTestBase
{
    [TestCase("let a = 5; a;", 5)]
    [TestCase("let a = 5 * 5; a;", 25)]
    [TestCase("let a = 5; let b = a; b;", 5)]
    [TestCase("let a = 5; let b = a; let c = a + b + 5; c;", 15)]
    public void TestLetStatements(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestInteger(evaluated, expected);
    }
}
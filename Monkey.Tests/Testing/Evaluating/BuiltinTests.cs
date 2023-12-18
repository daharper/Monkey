using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class BuiltinTests : EvaluatingTestBase
{
    [TestCase("len(\"\")", 0)]
    [TestCase("len(\"four\")", 4)]
    [TestCase("len(\"hello world\")", 11)]
    [TestCase("len(1)", "argument to `len` not supported, got INTEGER")]
    [TestCase("len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1")]
    public void TestBuiltinFunctions(string input, object expected)
    {
        var evaluated = TestEval(input);

        switch (expected)
        {
            case int expectedInt:
                TestInteger(evaluated, expectedInt);
                break;
            case bool expectedBool:
                TestBoolean(evaluated, expectedBool);
                break;
            case string expectedString when evaluated is ErrorObject error:
                Assert.That(error.Message, Is.EqualTo(expectedString));
                return;
            case string expectedString:
                TestString(evaluated, expectedString);
                break;
            default:
                Assert.Fail($"type of expected not handled. got={expected.GetType().Name}");
                break;
        }
    }
}
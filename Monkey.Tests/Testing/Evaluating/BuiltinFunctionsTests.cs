using Monkey.Evaluating.Ast;

namespace Monkey.Tests.Testing.Evaluating;

public class BuiltinFunctionsTests : EvaluatingTestBase
{
    [TestCase("len(\"\")", 0)]
    [TestCase("len(\"four\")", 4)]
    [TestCase("len(\"hello world\")", 11)]
    [TestCase("len(1)", "argument to `len` not supported, got INTEGER")]
    [TestCase("len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1")]
    public void TestBuiltinFunctions(string input, object expected)
    {
        var evaluated = TestEval(input);
        
        if (expected is int expectedInt)
        {
            TestInteger(evaluated, expectedInt);
        }
        else if (expected is bool expectedBool)
        {
            TestBoolean(evaluated, expectedBool);
        }
        else if (expected is string expectedString)
        {
            if (evaluated is AstError error)
            {
                Assert.That(error.Message, Is.EqualTo(expectedString));
                return;
            }
            
            TestString(evaluated, expectedString);
        }
        else
        {
            Assert.Fail($"type of expected not handled. got={expected.GetType().Name}");
        }
    }
}
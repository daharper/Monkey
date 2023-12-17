using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class ErrorHandlingTests : EvaluatingTestBase
{
    [TestCase("5 + true;", "type mismatch: INTEGER + BOOLEAN")]
    [TestCase("5 + true; 5;", "type mismatch: INTEGER + BOOLEAN")]
    [TestCase("-true", "unknown operator: -BOOLEAN")]
    [TestCase("true + false;", "unknown operator: BOOLEAN + BOOLEAN")]
    [TestCase("5; true + false; 5", "unknown operator: BOOLEAN + BOOLEAN")]
    [TestCase("if (10 > 1) { true + false; }", "unknown operator: BOOLEAN + BOOLEAN")]
    [TestCase("if (10 > 1) {  if (10 > 1) { return true + false; } return 1; }", "unknown operator: BOOLEAN + BOOLEAN")]   
    [TestCase("foobar", "identifier not found: foobar")]
    public void TestErrorHandling(string input, string expectedMessage)
    {
        var evaluated = TestEval(input);
        var error = evaluated as MError;
        
        Assert.That(error, Is.Not.Null, $"no error object returned, got {evaluated}=({evaluated.Inspect()})");
        Assert.That(error!.Message, Is.EqualTo(expectedMessage), $"wrong error message, got {error.Message}");
    }
}
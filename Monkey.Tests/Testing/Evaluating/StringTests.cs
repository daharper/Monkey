using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class StringTests : EvaluatingTestBase
{
    [Test]
    public void TestStringLiteral()
    {
        const string input = "\"Hello World!\"";
        
        var evaluated = TestEval(input);
        var str = AssertCast<StringObject>(evaluated);
        
        Assert.That(str.Value, Is.EqualTo("Hello World!"));
    }

    [Test]
    public void TestStringConcatenation()
    {
        const string input = "\"Hello\" + \" \" + \"World!\"";

        var evaluated = TestEval(input);
        
        if (evaluated is ErrorObject error)
        {
            Assert.Fail(error.Message);
        }
        
        var str = AssertCast<StringObject>(evaluated);

        Assert.That(str.Value, Is.EqualTo("Hello World!"));
    }
}
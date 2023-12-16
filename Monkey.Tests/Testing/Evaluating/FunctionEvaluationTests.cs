using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class FunctionEvaluationTests :  EvaluatingTestBase
{
    [Test]
    public void TestFunctionObject()
    {
        var evaluated = TestEval("fn(x) { x + 2; };");
        var fn = AssertCast<MFunction>(evaluated);
        
        Assert.That(fn, Is.Not.Null);
        Assert.That(fn!.Parameters, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(fn!.Parameters[0].ToString(), Is.EqualTo("x"));
            Assert.That(fn!.Body?.ToString(), Is.EqualTo("(x + 2)"));
        });
    }

    [TestCase("let identity = fn(x) { x ; }; identity(5);", 5)]
    [TestCase("let identity = fn(x) { return x; }; identity(5);", 5)]
    [TestCase("let double = fn(x) { x * 2; }; double(5);", 10)]
    [TestCase("let add = fn(x, y) { x + y; }; add(5, 5);", 10)]
    [TestCase("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20)]
    [TestCase("fn(x) { x; }(5)", 5)]
    public void TestFunctionApplication(string input, int expected)
    {
        var actual = TestEval(input);
        TestInteger(actual, expected);
    }

    [Test]
    public void TestClosures()
    {
        const string input = @"
            let newAdder = fn(x) {
                fn(y) { x + y };
            };
            
            let addTwo = newAdder(2);
            addTwo(2);";
        
        TestInteger(TestEval(input), 4);
    }
}
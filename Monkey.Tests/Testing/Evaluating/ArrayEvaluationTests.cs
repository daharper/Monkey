using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class ArrayEvaluationTests : EvaluatingTestBase 
{
    [Test]
    public void TestArrayLiterals()
    {
        var input = "[1, 2 * 2, 3 + 3]";
        var evaluated = TestEval(input);
        var result = AssertCast<ArrayObject>(evaluated);
        
        Assert.That(result.Elements.Count, Is.EqualTo(3), 
            $"expected 3 elements in array got {result.Elements.Count}");
        
        TestInteger(result.Elements[0], 1);
        TestInteger(result.Elements[1], 4);
        TestInteger(result.Elements[2], 6);
    }
    
    [TestCase("[1, 2, 3][0]", 1)]
    [TestCase("[1, 2, 3][1]", 2)]
    [TestCase("[1, 2, 3][2]", 3)]
    [TestCase("let i = 0; [1][i];", 1)]
    [TestCase("[1, 2, 3][1 + 1];", 3)]
    [TestCase("let myArray = [1, 2, 3]; myArray[2];", 3)]
    [TestCase("let myArray = [1, 2, 3]; myArray[0] + myArray[1] + myArray[2];", 6)]
    [TestCase("let myArray = [1, 2, 3]; let i = myArray[0]; myArray[i]", 2)]
    [TestCase("[1, 2, 3][3]", null)]
    [TestCase("[1, 2, 3][-1]", null)]
    public void TestArrayIndexExpressions(string input, object? expected)
    {
        var evaluated = TestEval(input);
        
        if (expected is null)
        {
            TestNull(evaluated);
        }
        else
        {
            TestInteger(evaluated, (int)expected);
        }
    }
}
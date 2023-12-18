namespace Monkey.Tests.Testing.Evaluating;

public class BooleanEvaluationTests : EvaluatingTestBase
{
    [Test]
    public void TestEvalBooleanExpressions()
    {
        var tests = new List<(string input, bool expected)>
        {
            ("true", true),
            ("false", false),
            ("1 < 2", true),
            ("1 > 2", false),
            ("1 < 1", false),
            ("1 > 1", false),
            ("1 == 1", true),
            ("1 != 1", false),
            ("1 == 2", false),
            ("1 != 2", true),
            ("true == true", true),
            ("false == false", true),
            ("true == false", false),
            ("true != false", true),
            ("false != true", true),
            ("(1 < 2) == true", true),
            ("(1 < 2) == false", false),
            ("(1 > 2) == true", false),
            ("(1 > 2) == false", true),
        };
        
        tests.ForEach(test =>
        {
            var evaluated = TestEval(test.input);
            TestBoolean(evaluated, test.expected);
        });
    }
}
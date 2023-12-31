using Monkey.Evaluating.Objects;

namespace Monkey.Tests.Testing.Evaluating;

public class HashTests : EvaluatingTestBase
{
    [Test]
    public void TestStringHashKey()
    {
        var hello1 = new StringObject("Hello World");
        var hello2 = new StringObject("Hello World");
        var diff1 = new StringObject("My name is johnny");
        var diff2 = new StringObject("My name is johnny");
        
        Assert.Multiple(() =>
        {
            Assert.That(hello1.GetHashCode(), Is.EqualTo(hello2.GetHashCode()),
                $"strings with same content have different hash keys");

            Assert.That(diff1.GetHashCode(), Is.EqualTo(diff2.GetHashCode()),
                $"strings with same content have different hash keys");
        });
        
        Assert.That(hello1.GetHashCode(), Is.Not.EqualTo(diff1.GetHashCode()),
            $"strings with different content have same hash keys");
    }

    [Test]
    public void TestHashLiterals()
    {
        const string input = """
                             let two = "two";
                             {
                                 "one": 10 - 9,
                                 two: 1 + 1,
                                 "thr" + "ee": 6 / 2,
                                 4: 4,
                                 true: 5,
                                 false: 6
                             }
                             """;
        
        var evaluated = TestEval(input);
        
        var hash = AssertCast<HashObject>(evaluated);
        
        List<(object key, object value)> expected = [
            ("one", 1),
            ("two", 2),
            ("three", 3),
            (4, 4),
            (true, 5),
            (false, 6)
        ];
        
        Assert.That(hash.Pairs, Has.Count.EqualTo(expected.Count),
            $"hash has wrong number of pairs. got {hash.Pairs.Count}, want {expected.Count}");

        foreach (var (key, value) in expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(hash.Pairs.TryGetValue(key.GetHashCode(), out var pair), Is.True);

                Assert.That(pair.Key.ToString(), Is.EqualTo(key.ToString()));
                Assert.That(pair.Value?.ToString() ?? "", Is.EqualTo(value.ToString()));
            });
        }
    }

    [TestCase("""{"foo": 5}["foo"]""", 5)]
    [TestCase("""{"foo": 5}["bar"]""", null)]
    [TestCase("""let key = "foo"; {"foo": 5}[key]""", 5)]
    [TestCase("""{}["foo"]""", null)]
    [TestCase("{5: 5}[5]", 5)]
    [TestCase("{true: 5}[true]", 5)]
    [TestCase("{false: 5}[false]", 5)]
    public void TestHashIndexExpressions(string input, object expected)
    {
        var evaluated = TestEval(input);

        switch (evaluated)
        {
            case ErrorObject error:
                Assert.True(false, $"Unexpected error, got={error.Message}");
                return;
            case NullObject:
                Assert.That(expected, Is.Null);
                return;
            case IntegerObject integer:
                Assert.That(expected, Is.EqualTo(integer.Value));
                return;
            case BooleanObject boolean:
                Assert.That(expected, Is.EqualTo(boolean.Value));
                return;
            default:
                TestString(evaluated, Convert.ToString(expected) ?? "");
                break;
        }
    }
}
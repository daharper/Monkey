using Monkey.Parsing;

namespace Monkey.Tests;

public class TestBase
{
    protected static T AssertCast<T>(object obj)
    {
        Assert.That(obj, Is.InstanceOf<T>(), $"expected {typeof(T).Name} got {obj.GetType().Name}");
        return (T)obj;
    }
    
    protected static void CheckErrors(Parser parser)
    {
        Assert.That(
            parser.HasErrors, Is.False, 
            $"parser has the following errors:{Environment.NewLine}{string.Join(Environment.NewLine, parser.Errors)}");
    }
}
using Monkey.Parsing;

namespace Monkey.Tests;

public class TestBase
{
    protected T AssertCast<T>(object obj)
    {
        Assert.IsInstanceOf<T>(obj, $"expected {typeof(T).Name} got {obj.GetType().Name}");
        return (T)obj;
    }
    
    protected void CheckErrors(Parser parser)
    {
        Assert.IsFalse(
            parser.HasErrors, 
            $"parser has the following errors:{Environment.NewLine}{string.Join(Environment.NewLine, parser.Errors)}");
    }
}
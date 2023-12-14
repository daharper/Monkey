using Monkey.Parsing;

namespace Monkey.Tests.Testing.Parsing;

public abstract class BaseParserTest
{
    protected void CheckErrors(Parser parser)
    {
        Assert.IsFalse(
            parser.HasErrors, 
            $"parser has the following errors:{Environment.NewLine}{string.Join(Environment.NewLine, parser.Errors)}");
    }
}
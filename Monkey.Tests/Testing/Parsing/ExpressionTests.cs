using Monkey.Parsing.Expressions;
using Monkey.Parsing.Statements;

namespace Monkey.Tests.Testing.Parsing;

public class ExpressionTests : TestBase
{
    [Test]
    public void TestIdentifierExpression()
    {
        var programme = AssertParse("foobar", 1);
        var statement = AssertCast<ExpressionStatement>(programme.Statements[0]);
        var expression = AssertCast<IdentifierExpression>(statement.Expression!);

        Assert.AreEqual("foobar", expression.Value, $"expected 'foobar' got '{expression.Value}'");
        Assert.AreEqual("foobar", expression.TokenLiteral(), $"expected 'foobar' got '{expression.TokenLiteral()}'");
    }
}
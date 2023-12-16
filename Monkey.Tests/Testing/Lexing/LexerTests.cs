using Monkey.Lexing;
using Monkey.Tests.Fixtures.Lexing;
using Monkey.Utils;

namespace Monkey.Tests.Testing.Lexing;

public class LexerTests
{
    [Test]
    public void TestNextToken()
    {
        TestToken(new SimpleLexerFixture());
        TestToken(new ComplexLexerFixture());
        TestToken(new ComplexLexerFixture());
        TestToken(new StringLexerFixture());
    }
    
    private void TestToken(LexerFixture data)
    {
        var lexer = new Lexer(data.Input);

        data.Tokens.ForEach((i, expected) => {
            var actual = lexer.NextToken();

            Assert.Multiple(() =>
            {
                Assert.That(
                    actual.Type, Is.EqualTo(expected.Type),
                    $"tests[{i}] - tokentype wrong. expected={expected.Type}, got={actual.Type}");

                Assert.That(actual.Literal, Is.EqualTo(expected.Literal),
                    $"tests[{i}] - literal wrong. expected={expected.Literal}, got={actual.Literal}");
            });
        });
    }
}
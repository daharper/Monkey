using Monkey.Lexing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class ObjectTests : ParsingTestBase
{
    [Test]
    public void TestToString()
    {
        var program = ProgramNode.Create(
        [
            new LetNode(new Token(Token.Let, "let"))
            {
                Name = new IdentifierNode(new Token(Token.Identifier, "myVar"), "myVar"),
                Value = new IdentifierNode(new Token(Token.Identifier, "anotherVar"), "anotherVar")
            }
        ]);
        
        Assert.That(program.ToString(), Is.EqualTo("let myVar = anotherVar;"),
            $"expected 'let myVar = anotherVar;' got '{program}'");
    }
}
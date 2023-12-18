using Monkey.Lexing;
using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class ObjectTests : ParsingTestBase
{
    [Test]
    public void TestToString()
    {
        var programme = ProgramNode.Create(
        [
            new LetNode(new Token(Token.Let, "let"))
            {
                Name = new IdentifierNode(token: new Token(Token.Identifier, "myVar"), value: "myVar"),
                Value = new IdentifierNode(token: new Token(Token.Identifier, "anotherVar"), value: "anotherVar")
            }
        ]);
        
        Assert.That(programme.ToString(), Is.EqualTo("let myVar = anotherVar;"),
            $"expected 'let myVar = anotherVar;' got '{programme}'");
    }
}
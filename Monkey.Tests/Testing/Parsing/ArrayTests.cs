using Monkey.Parsing.Nodes;

namespace Monkey.Tests.Testing.Parsing;

public class ArrayTests : ParsingTestBase
{
    [Test]
    public void TestArrayParsing()
    {
        var program = AssertParse("[1, 2 * 2, 3 + 3];", 1);
        
        Assert.That(program.Statements, Has.Count.EqualTo(1), 
            $"expected 1 statement got {program.Statements.Count}");
        
        var statement = AssertCast<ExpressionNode>(program.Statements[0]);
        
        Assert.That(statement.Expression, Is.Not.Null, 
            "expected expression not to be null");
        
        var array = AssertCast<ArrayNode>(statement.Expression);
        
        Assert.That(array.Elements, Has.Count.EqualTo(3), 
            $"expected 3 elements in array got {array.Elements.Count}");
        
        Assert.Multiple(() =>
        {
            Assert.That(array.Elements[0].ToString(), Is.EqualTo("1"),
                $"expected '1' got '{array.Elements[0]}'");

            Assert.That(array.Elements[1].ToString(), Is.EqualTo("(2 * 2)"),
                $"expected '(2 * 2)' got '{array.Elements[1]}'");

            Assert.That(array.Elements[2].ToString(), Is.EqualTo("(3 + 3)"),
                $"expected '(3 + 3)' got '{array.Elements[2]}'");
        });
    }
}
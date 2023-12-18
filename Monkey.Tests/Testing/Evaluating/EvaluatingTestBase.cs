using Monkey.Evaluating;
using Monkey.Evaluating.Objects;
using Monkey.Lexing;
using Monkey.Parsing;
using Environment = Monkey.Evaluating.Environment;

namespace Monkey.Tests.Testing.Evaluating;

public class EvaluatingTestBase : TestBase
{
    protected MObject TestEval(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        var environment = new Environment();
        return Evaluator.Eval(program, environment);
    }
    
    protected static void TestString(MObject obj, string expected)
    {
        Assert.That(obj is StringObject, Is.True, 
            $"object is not StringObject. got={obj.GetType().Name} ({obj})");
        
        var result = (StringObject) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");    
    }
    
    protected static void TestInteger(MObject obj, int expected)
    {
        Assert.That(obj is IntegerObject, Is.True, 
            $"object is not IntegerObject. got={obj.GetType().Name} ({obj})");
        
        var result = (IntegerObject) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected static void TestBoolean(MObject obj, bool expected)
    {
        Assert.That(obj is BooleanObject, Is.True, 
            $"object is not BooleanObject. got={obj.GetType().Name} ({obj})");
        
        var result = (BooleanObject) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected static void TestNull(MObject obj)
    {
        Assert.That(obj, Is.EqualTo(Builtin.Null), 
            $"object is not NullObject. got={obj.GetType().Name} ({obj})");
    }
}
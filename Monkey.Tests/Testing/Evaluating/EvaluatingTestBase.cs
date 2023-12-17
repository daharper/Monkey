using Monkey.Evaluating;
using Monkey.Evaluating.Objects;
using Monkey.Lexing;
using Monkey.Parsing;

using Environment = Monkey.Evaluating.Environment;

namespace Monkey.Tests.Testing.Evaluating;

public class EvaluatingTestBase : TestBase
{
    protected IMObject TestEval(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        var environment = new Environment();
        return Evaluator.Eval(programme, environment)!;
    }
    
    protected void TestString(IMObject obj, string expected)
    {
        Assert.That(obj is MString, Is.True, 
            $"object is not StringObject. got={obj.GetType().Name} ({obj})");
        
        var result = (MString) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");    
    }
    
    protected void TestInteger(IMObject obj, int expected)
    {
        Assert.That(obj is MInteger, Is.True, 
            $"object is not IntegerObject. got={obj.GetType().Name} ({obj})");
        
        var result = (MInteger) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected void TestBoolean(IMObject obj, bool expected)
    {
        Assert.That(obj is MBoolean, Is.True, 
            $"object is not BooleanObject. got={obj.GetType().Name} ({obj})");
        
        var result = (MBoolean) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected void TestNull(IMObject obj)
    {
        Assert.That(obj, Is.EqualTo(Evaluator.Null), 
            $"object is not NullObject. got={obj.GetType().Name} ({obj})");
    }
}
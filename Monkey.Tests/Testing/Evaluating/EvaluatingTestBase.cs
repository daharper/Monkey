using Monkey.Evaluating;
using Monkey.Evaluating.Ast;
using Monkey.Lexing;
using Monkey.Parsing;
using Environment = Monkey.Evaluating.Environment;

namespace Monkey.Tests.Testing.Evaluating;

public class EvaluatingTestBase : TestBase
{
    protected IAstObject TestEval(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var programme = parser.ParseProgramme();
        
        CheckErrors(parser);
        
        var environment = new Environment();
        return Evaluator.Eval(programme, environment);
    }
    
    protected void TestString(IAstObject obj, string expected)
    {
        Assert.That(obj is AstString, Is.True, 
            $"object is not StringObject. got={obj.GetType().Name} ({obj})");
        
        var result = (AstString) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");    
    }
    
    protected void TestInteger(IAstObject obj, int expected)
    {
        Assert.That(obj is AstInteger, Is.True, 
            $"object is not IntegerObject. got={obj.GetType().Name} ({obj})");
        
        var result = (AstInteger) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected void TestBoolean(IAstObject obj, bool expected)
    {
        Assert.That(obj is AstBoolean, Is.True, 
            $"object is not BooleanObject. got={obj.GetType().Name} ({obj})");
        
        var result = (AstBoolean) obj;
        
        Assert.That(result.Value, Is.EqualTo(expected), 
            $"object has wrong value. got={result.Value} want={expected}");
    }
    
    protected void TestNull(IAstObject obj)
    {
        Assert.That(obj, Is.EqualTo(Builtin.Null), 
            $"object is not NullObject. got={obj.GetType().Name} ({obj})");
    }
}
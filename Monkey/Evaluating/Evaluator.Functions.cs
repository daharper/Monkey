using Monkey.Evaluating.Objects;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating;

partial class Evaluator
{
    private static MObject EvalHashLiteral(Node node, Environment env)
    {   
        var pairs = new Dictionary<int, KeyValuePair<MObject, MObject>>();

        if (node is not HashNode hash) 
            return ErrorObject.Create("node is not HashLiteral");
        
        foreach (var (keyNode, valueNode) in hash.Pairs)
        {
            var key = Eval(keyNode, env);
            if (key is ErrorObject) return key;
            
            if (key is not IntegerObject and not BooleanObject and not StringObject) 
                return ErrorObject.Create("unusable as hash key: {0}", key.Type);

            var value = Eval(valueNode, env);
            if (value is ErrorObject) return value;

            var hashed = key.GetHashCode();
            pairs[hashed] = new KeyValuePair<MObject, MObject>(key, value);
        }

        return new HashObject(pairs); 
    }
    
    private static MObject EvalIndexExpression(MObject? left1, MObject index)
    {
        return (left1, index) switch
        {
            (ArrayObject array, IntegerObject integer) => EvalArrayIndexExpression(array, integer),
            (HashObject hash, _) => EvalHashIndexExpression(hash, index),
            _ => ErrorObject.Create("index operator not supported: {0}", left1?.Type ?? "null")
        };
    }

    private static MObject EvalHashIndexExpression(HashObject hash, MObject index)
    {
        if (index is not IntegerObject and not BooleanObject and not StringObject) 
            return ErrorObject.Create("unusable as hash key: {0}", index.Type);

        var hashed = index.GetHashCode();

        return !hash.Pairs.TryGetValue(hashed, out var pair) ? Builtin.Null : pair.Value;
    }

    private static MObject EvalArrayIndexExpression(ArrayObject array, IntegerObject integer)
    {   
        var index = integer.Value;
        var max = array.Elements.Count - 1;

        if (index < 0 || index > max)
        {
            return Builtin.Null;
        }

        return array.Elements[index];
    }
    
    private static List<MObject> EvalExpressions(List<Node> exps, Environment env)
    {
        var result = new List<MObject>();

        foreach (var exp in exps)
        {
            var evaluated = Eval(exp, env);
            if (evaluated is ErrorObject) return [evaluated];
            
            result.Add(evaluated);
        }
        
        return result;
    }

    private static MObject EvalIdentifierExpression(IdentifierNode identifier, Environment env)
    {
        if (env.TryGet(identifier.Value, out var value)) return value ?? Builtin.Null;
        
        return Builtin.Functions.TryGetValue(identifier.Value, out var builtin) 
            ? builtin 
            : ErrorObject.Create("identifier not found: " + identifier.Value);
    }
    
    private static MObject EvalIfExpression(IfNode ifExpression, Environment env)
    {
        var condition = Eval(ifExpression.Condition, env);
        
        if (condition is ErrorObject) return condition;
        if (IsTruthy(condition)) return Eval(ifExpression.Consequence, env);
        
        return ifExpression.Alternative != null 
            ? Eval(ifExpression.Alternative, env) 
            : Builtin.Null;
    }
    
    private static MObject EvalInfixExpression(string op, MObject? left, MObject? right)
    {
        if (left is null || right is null) 
            return ErrorObject.Create("missing value: {0} {1} {2}", left?.Type ?? "null", op, right?.Type ?? "null");
        
        if (left.Type != right.Type) 
            return ErrorObject.Create("type mismatch: {0} {1} {2}", left.Type, op, right.Type);

        if (left is IntegerObject integer)
            return EvalIntegerInfixExpression(op, integer, (IntegerObject)right);
        
        if (left is StringObject str)
            return EvalStringInfixExpression(op, str, (StringObject)right);
        
        if (op == "==")
            return left == right ? Builtin.True : Builtin.False;

        if (op == "!=")
            return left != right ? Builtin.True : Builtin.False;
        
        return ErrorObject.Create("unknown operator: {0} {1} {2}", left.Type, op, right.Type);
    }

    private static MObject EvalStringInfixExpression(string op, StringObject left, StringObject right)
    {
        return op == "+" 
            ? new StringObject(left.Value + right.Value)
            : ErrorObject.Create("unknown operator: {0} {1} {2}", left.Type, op, right.Type); 
    }

    private static MObject EvalIntegerInfixExpression(string op, IntegerObject leftInteger, IntegerObject rightInteger)
    {
        var left = leftInteger.Value;
        var right = rightInteger.Value;

        return op switch
        {
            "+" => new IntegerObject(left + right),
            "-" => new IntegerObject(left - right),
            "*" => new IntegerObject(left * right),
            "/" => new IntegerObject(left / right),
            "<" => left < right ? Builtin.True : Builtin.False,
            ">" => left > right ? Builtin.True : Builtin.False,
            "==" => left == right ? Builtin.True : Builtin.False,
            "!=" => left != right ? Builtin.True : Builtin.False,
            _ => ErrorObject.Create("unknown operator: {0} {1} {2}", left, op, right)
        };
    }
    
    private static MObject EvalPrefixExpression(string op, MObject right)
    {
        return op switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => ErrorObject.Create("unknown operator: {0}{1}", op, right.Type)
        };
    }
    
    private static BooleanObject EvalBangOperatorExpression(MObject right)
        => right switch
        {
            BooleanObject boolean => boolean.Value ? Builtin.False : Builtin.True,
            NullObject => Builtin.True,
            _ => Builtin.False
        };

    private static MObject EvalMinusPrefixOperatorExpression(MObject right)
    {
        if (right is not IntegerObject integer)
        {
            return ErrorObject.Create("unknown operator: -{0}", right.Type);
        }

        var value = integer.Value;
        return new IntegerObject(-value);
    }

    private static MObject EvalProgramme(List<Node> statements, Environment env)    
    {
        MObject result = Builtin.Null;
        
        foreach (var statement in statements)
        {
            result = Eval(statement, env);
            
            switch (result)
            {
                case ReturnObject returnValue:
                    return returnValue.Value;
                case ErrorObject:
                    return result;
            }
        }

        return result;
    }

    private static MObject EvalBlockStatement(BlockNode block, Environment env)
    {
        MObject result = Builtin.Null;
        
        foreach (var statement in block.Statements)
        {
            result = Eval(statement, env);

            if (result is ReturnObject or ErrorObject) 
            {
                return result;
            }
        }

        return result;
    }
}
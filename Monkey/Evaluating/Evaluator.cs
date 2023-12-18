using Monkey.Evaluating.Objects;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating;

public static class Evaluator
{
    public static MObject Eval(Node node, Environment environment)
    {
        switch (node)
        {
             case Programme programme:
                 return EvalProgramme(programme.Statements, environment);
             case ExpressionNode expressionStatement:
                 return Eval(expressionStatement.Expression, environment);
             case IntegerNode integerExpression:
                 return new IntegerObject { Value = integerExpression.Value };
             case BooleanNode booleanExpression:
                 return booleanExpression.Value ? Builtin.True : Builtin.False;
             case PrefixNode prefixExpression:
                 var right = Eval(prefixExpression.Right, environment);
                 return right is ErrorObject ? right : EvalPrefixExpression(prefixExpression.Operator, right);
             case InfixNode infixExpression:
                  var left = Eval(infixExpression.Left, environment);
                  if (left is ErrorObject) return left;
                  var right1 = Eval(infixExpression.Right, environment);
                  return right1 is ErrorObject ? right1 : EvalInfixExpression(infixExpression.Operator, left, right1);
             case IfNode ifExpression:
                  return EvalIfExpression(ifExpression, environment);
             case IdentifierNode identifierExpression:
                  return EvalIdentifierExpression(identifierExpression, environment);
             case BlockNode blockStatement:
                  return EvalBlockStatement(blockStatement, environment);
             case ReturnNode returnStatement:
                  var value = Eval(returnStatement.ReturnValue, environment);
                  return value is ErrorObject ? value : new ReturnObject(value);
             case LetNode letStatement:
                  var val = Eval(letStatement.Value, environment);
                  if (val is ErrorObject) return val;
                  environment.Set(letStatement.Name.Value, val);
                  break;
             case FunctionNode functionExpression:
                  return new FunctionObject(functionExpression.Parameters, functionExpression.Body, environment);
             case StringNode stringLiteral:
                  return new StringObject(stringLiteral.Value);
             case CallNode callExpression:
                 var function = Eval(callExpression.Function, environment);
                 if (function is ErrorObject) return function;
                 var args = EvalExpressions(callExpression.Arguments, environment);
                 if (args.Count == 1 && args[0] is ErrorObject) return args[0];
                 return ApplyFunction(function, args);
             case ArrayNode arrayLiteral:
                 var elements = EvalExpressions(arrayLiteral.Elements, environment);
                 if (elements.Count == 1 && elements[0] is ErrorObject) return elements[0];
                 return new ArrayObject { Elements = elements };
             case IndexNode indexExpression:
                 var left1 = Eval(indexExpression.Left, environment);
                 if (left1 is ErrorObject) return left1;
                 var index = Eval(indexExpression.Index, environment);
                 if (index is ErrorObject) return index;
                 return EvalIndexExpression(left1, index);
             case HashNode:
                 return EvalHashLiteral(node, environment);
             default:
                 throw new ArgumentException($"Unknown node type: {node.GetType().Name}");
         }

        return Builtin.Null;
    }

    private static MObject EvalHashLiteral(Node node, Environment environment)
    {   
        var pairs = new Dictionary<int, KeyValuePair<MObject, MObject>>();

        if (node is not HashNode hashLiteral) 
            return ErrorObject.Create("node is not HashLiteral");
        
        foreach (var (keyNode, valueNode) in hashLiteral.Pairs)
        {
            var key = Eval(keyNode, environment);
            if (key is ErrorObject) return key;
            
            if (key is not IntegerObject and not BooleanObject and not StringObject) 
                return ErrorObject.Create("unusable as hash key: {0}", key.Type);

            var value = Eval(valueNode, environment);
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

    private static MObject ApplyFunction(MObject? function, List<MObject> args)
    {
        switch (function)
        {
            case FunctionObject fn:
            {
                var extendedEnv = ExtendFunctionEnv(fn, args);
                var evaluated = Eval(fn.Body!, extendedEnv);
            
                return UnwrapReturnValue(evaluated);
            }
            case BuiltinObject builtin:
                return builtin.Function(args);
            default:
                return ErrorObject.Create("not a function: {0}", function?.Type ?? "null");
        }
    }

    private static MObject UnwrapReturnValue(MObject obj)
        => obj switch
            {
                ReturnObject returnValue => returnValue.Value,
                _ => obj
            };
    
    private static Environment ExtendFunctionEnv(FunctionObject fn, IReadOnlyList<MObject> args)
    {
        var env = new Environment { Outer = fn.Env };

        for (var i = 0; i < fn.Parameters.Count; i++)
        {
            env.Set(fn.Parameters[i].Value, args[i]);
        }

        return env;
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

    private static MObject EvalIdentifierExpression(IdentifierNode identifier, Environment environment)
    {
        if (environment.TryGet(identifier.Value, out var value)) return value ?? Builtin.Null;
        
        return Builtin.Functions.TryGetValue(identifier.Value, out var builtin) 
            ? builtin 
            : ErrorObject.Create("identifier not found: " + identifier.Value);
    }
    
    private static MObject EvalIfExpression(IfNode ifExpression, Environment environment)
    {
        var condition = Eval(ifExpression.Condition, environment);
        
        if (condition is ErrorObject) return condition;
        if (IsTruthy(condition)) return Eval(ifExpression.Consequence, environment);
        
        return ifExpression.Alternative != null 
            ? Eval(ifExpression.Alternative, environment) 
            : Builtin.Null;
    }

    private static bool IsTruthy(MObject? condition)
    {
        return condition switch
        {
            NullObject => false,
            BooleanObject boolean => boolean.Value,
            IntegerObject { Value: 0 } => false,
            _ => true
        };
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
            "+" => new IntegerObject { Value = left + right },
            "-" => new IntegerObject { Value = left - right },
            "*" => new IntegerObject { Value = left * right },
            "/" => new IntegerObject { Value = left / right },
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

        integer.Value = -integer.Value;
        return integer;
    }

    private static MObject EvalProgramme(List<Node> statements, Environment environment)    
    {
        MObject result = Builtin.Null;
        
        foreach (var statement in statements)
        {
            result = Eval(statement, environment);
            
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

    private static MObject EvalBlockStatement(BlockNode block, Environment environment)
    {
        MObject result = Builtin.Null;
        
        foreach (var statement in block.Statements)
        {
            result = Eval(statement, environment);

            if (result is ReturnObject or ErrorObject) 
            {
                return result;
            }
        }

        return result;
    }
}
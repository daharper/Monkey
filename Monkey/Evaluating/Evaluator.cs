using Monkey.Evaluating.Ast;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating;

public static class Evaluator
{
    public static IAstObject Eval(Node node, Environment environment)
    {
        switch (node)
        {
             case Programme programme:
                 return EvalProgramme(programme.Statements, environment);
             case ExpressionStatement expressionStatement:
                 return Eval(expressionStatement.Expression!, environment);
             case IntegerLiteral integerExpression:
                 return new AstInteger { Value = integerExpression.Value };
             case BooleanLiteral booleanExpression:
                 return booleanExpression.Value ? Builtin.True : Builtin.False;
             case PrefixExpression prefixExpression:
                 var right = Eval(prefixExpression.Right, environment);
                 return right is AstError ? right : EvalPrefixExpression(prefixExpression.Operator, right);
             case InfixExpression infixExpression:
                  var left = Eval(infixExpression.Left, environment);
                  if (left is AstError) return left;
                  var right1 = Eval(infixExpression.Right, environment);
                  return right1 is AstError ? right1 : EvalInfixExpression(infixExpression.Operator, left, right1);
             case IfExpression ifExpression:
                  return EvalIfExpression(ifExpression, environment);
             case Identifier identifierExpression:
                  return EvalIdentifierExpression(identifierExpression, environment);
             case BlockStatement blockStatement:
                  return EvalBlockStatement(blockStatement, environment);
             case ReturnStatement returnStatement:
                  var value = Eval(returnStatement.ReturnValue, environment);
                  return value is AstError ? value : new AstReturnValue(value);
             case LetStatement letStatement:
                  var val = Eval(letStatement.Value, environment);
                  if (val is AstError) return val;
                  environment.Set(letStatement.Name.Value, val);
                  break;
             case FunctionLiteral functionExpression:
                  return new AstFunction(functionExpression.Parameters, functionExpression.Body, environment);
             case StringLiteral stringLiteral:
                  return new AstString(stringLiteral.Value);
             case CallExpression callExpression:
                 var function = Eval(callExpression.Function, environment);
                 if (function is AstError) return function;
                 var args = EvalExpressions(callExpression.Arguments, environment);
                 if (args.Count == 1 && args[0] is AstError) return args[0];
                 return ApplyFunction(function, args);
             case ArrayLiteral arrayLiteral:
                 var elements = EvalExpressions(arrayLiteral.Elements, environment);
                 if (elements.Count == 1 && elements[0] is AstError) return elements[0];
                 return new AstArray { Elements = elements };
             case IndexExpression indexExpression:
                 var left1 = Eval(indexExpression.Left, environment);
                 if (left1 is AstError) return left1;
                 var index = Eval(indexExpression.Index, environment);
                 if (index is AstError) return index;
                 return EvalIndexExpression(left1, index);
             case HashLiteral:
                 return EvalHashLiteral(node, environment);
             default:
                 throw new ArgumentException($"Unknown node type: {node.GetType().Name}");
         }

        return Builtin.Null;
    }

    private static IAstObject EvalHashLiteral(Node node, Environment environment)
    {   
        var pairs = new Dictionary<int, KeyValuePair<IAstObject, IAstObject>>();

        if (node is not HashLiteral hashLiteral) 
            return AstError.Create("node is not HashLiteral");
        
        foreach (var (keyNode, valueNode) in hashLiteral.Pairs)
        {
            var key = Eval(keyNode, environment);
            if (key is AstError) return key;
            
            if (key is not AstInteger and not AstBoolean and not AstString) 
                return AstError.Create("unusable as hash key: {0}", key.Type());

            var value = Eval(valueNode, environment);
            if (value is AstError) return value;

            var hashed = key.GetHashCode();
            pairs[hashed] = new KeyValuePair<IAstObject, IAstObject>(key, value);
        }

        return new AstHash(pairs); 
    }

    private static IAstObject EvalIndexExpression(IAstObject? left1, IAstObject index)
    {
        return (left1, index) switch
        {
            (AstArray array, AstInteger integer) => EvalArrayIndexExpression(array, integer),
            (AstHash hash, _) => EvalHashIndexExpression(hash, index),
            _ => AstError.Create("index operator not supported: {0}", left1?.Type() ?? "null")
        };
    }

    private static IAstObject EvalHashIndexExpression(AstHash hash, IAstObject index)
    {
        if (index is not AstInteger and not AstBoolean and not AstString) 
            return AstError.Create("unusable as hash key: {0}", index.Type());

        var hashed = index.GetHashCode();

        return !hash.Pairs.TryGetValue(hashed, out var pair) ? Builtin.Null : pair.Value;
    }

    private static IAstObject EvalArrayIndexExpression(AstArray array, AstInteger integer)
    {   
        var index = integer.Value;
        var max = array.Elements.Count - 1;

        if (index < 0 || index > max)
        {
            return Builtin.Null;
        }

        return array.Elements[index];
    }

    private static IAstObject ApplyFunction(IAstObject? function, List<IAstObject> args)
    {
        switch (function)
        {
            case AstFunction fn:
            {
                var extendedEnv = ExtendFunctionEnv(fn, args);
                var evaluated = Eval(fn.Body!, extendedEnv);
            
                return UnwrapReturnValue(evaluated);
            }
            case AstBuiltin builtin:
                return builtin.Function(args);
            default:
                return AstError.Create("not a function: {0}", function?.Type() ?? "null");
        }
    }

    private static IAstObject UnwrapReturnValue(IAstObject obj)
        => obj switch
            {
                AstReturnValue returnValue => returnValue.Value,
                _ => obj
            };
    
    private static Environment ExtendFunctionEnv(AstFunction fn, IReadOnlyList<IAstObject> args)
    {
        var env = new Environment { Outer = fn.Env };

        for (var i = 0; i < fn.Parameters.Count; i++)
        {
            env.Set(fn.Parameters[i].Value, args[i]);
        }

        return env;
    }

    private static List<IAstObject> EvalExpressions(List<Node> exps, Environment env)
    {
        var result = new List<IAstObject>();

        foreach (var exp in exps)
        {
            var evaluated = Eval(exp, env);
            if (evaluated is AstError) return [evaluated];
            
            result.Add(evaluated);
        }
        
        return result;
    }

    private static IAstObject EvalIdentifierExpression(Identifier identifier, Environment environment)
    {
        if (environment.TryGet(identifier.Value, out var value)) return value ?? Builtin.Null;
        
        return Builtin.Functions.TryGetValue(identifier.Value, out var builtin) 
            ? builtin 
            : AstError.Create("identifier not found: " + identifier.Value);
    }
    
    private static IAstObject EvalIfExpression(IfExpression ifExpression, Environment environment)
    {
        var condition = Eval(ifExpression.Condition, environment);
        
        if (condition is AstError) return condition;
        if (IsTruthy(condition)) return Eval(ifExpression.Consequence, environment);
        
        return ifExpression.Alternative != null 
            ? Eval(ifExpression.Alternative, environment) 
            : Builtin.Null;
    }

    private static bool IsTruthy(IAstObject? condition)
    {
        return condition switch
        {
            AstNull => false,
            AstBoolean boolean => boolean.Value,
            AstInteger { Value: 0 } => false,
            _ => true
        };
    }

    private static IAstObject EvalInfixExpression(string op, IAstObject? left, IAstObject? right)
    {
        if (left is null || right is null) 
            return AstError.Create("missing value: {0} {1} {2}", left?.Type() ?? "null", op, right?.Type() ?? "null");
        
        if (left.Type() != right.Type()) 
            return AstError.Create("type mismatch: {0} {1} {2}", left.Type(), op, right.Type());

        if (left is AstInteger integer)
            return EvalIntegerInfixExpression(op, integer, (AstInteger)right);
        
        if (left is AstString str)
            return EvalStringInfixExpression(op, str, (AstString)right);
        
        if (op == "==")
            return left == right ? Builtin.True : Builtin.False;

        if (op == "!=")
            return left != right ? Builtin.True : Builtin.False;
        
        return AstError.Create("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
    }

    private static IAstObject EvalStringInfixExpression(string op, AstString left, AstString right)
    {
        return op == "+" 
            ? new AstString(left.Value + right.Value)
            : AstError.Create("unknown operator: {0} {1} {2}", left.Type(), op, right.Type()); 
    }

    private static IAstObject EvalIntegerInfixExpression(string op, AstInteger leftInteger, AstInteger rightInteger)
    {
        var left = leftInteger.Value;
        var right = rightInteger.Value;

        return op switch
        {
            "+" => new AstInteger { Value = left + right },
            "-" => new AstInteger { Value = left - right },
            "*" => new AstInteger { Value = left * right },
            "/" => new AstInteger { Value = left / right },
            "<" => left < right ? Builtin.True : Builtin.False,
            ">" => left > right ? Builtin.True : Builtin.False,
            "==" => left == right ? Builtin.True : Builtin.False,
            "!=" => left != right ? Builtin.True : Builtin.False,
            _ => AstError.Create("unknown operator: {0} {1} {2}", left, op, right)
        };
    }
    
    private static IAstObject EvalPrefixExpression(string op, IAstObject right)
    {
        return op switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => AstError.Create("unknown operator: {0}{1}", op, right.Type())
        };
    }
    
    private static AstBoolean EvalBangOperatorExpression(IAstObject right)
        => right switch
        {
            AstBoolean boolean => boolean.Value ? Builtin.False : Builtin.True,
            AstNull => Builtin.True,
            _ => Builtin.False
        };

    private static IAstObject EvalMinusPrefixOperatorExpression(IAstObject right)
    {
        if (right is not AstInteger integer)
        {
            return AstError.Create("unknown operator: -{0}", right.Type());
        }

        integer.Value = -integer.Value;
        return integer;
    }

    private static IAstObject EvalProgramme(List<Node> statements, Environment environment)    
    {
        IAstObject result = Builtin.Null;
        
        foreach (var statement in statements)
        {
            result = Eval(statement, environment);
            
            switch (result)
            {
                case AstReturnValue returnValue:
                    return returnValue.Value;
                case AstError:
                    return result;
            }
        }

        return result;
    }

    private static IAstObject EvalBlockStatement(BlockStatement block, Environment environment)
    {
        IAstObject result = Builtin.Null;
        
        foreach (var statement in block.Statements)
        {
            result = Eval(statement, environment);

            if (result is AstReturnValue or AstError) 
            {
                return result;
            }
        }

        return result;
    }
}
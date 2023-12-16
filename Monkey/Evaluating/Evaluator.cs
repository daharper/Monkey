using Monkey.Evaluating.Objects;
using Monkey.Parsing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Interfaces;
using Monkey.Parsing.Statements;

namespace Monkey.Evaluating;

public static class Evaluator
{
    public static readonly MNull Null = new();
    public static readonly MBoolean True = new() { Value = true };
    public static readonly MBoolean False = new() { Value = false };
    
    public static IMObject? Eval(INode node, Environment environment)
    {
        switch (node)
        {
            case Programme programme:
                return EvalProgramme(programme.Statements, environment);
            case ExpressionStatement expressionStatement:
                return Eval(expressionStatement.Expression!, environment);
            case IntegerExpression integerExpression:
                return new MInteger { Value = integerExpression.Value };
            case BooleanExpression booleanExpression:
                return booleanExpression.Value ? True : False;
            case PrefixExpression prefixExpression:
                var right = Eval(prefixExpression.Right, environment);
                return right is MError ? right : EvalPrefixExpression(prefixExpression.Operator, right);
            case InfixExpression infixExpression:
                 var left = Eval(infixExpression.Left, environment);
                 if (left is MError) return left;
                 var right1 = Eval(infixExpression.Right, environment);
                 return right1 is MError ? right1 : EvalInfixExpression(infixExpression.Operator, left, right1);
            case IfExpression ifExpression:
                 return EvalIfExpression(ifExpression, environment);
            case IdentifierExpression identifierExpression:
                 return EvalIdentifierExpression(identifierExpression, environment);
            case BlockStatement blockStatement:
                 return EvalBlockStatement(blockStatement, environment);
            case ReturnStatement returnStatement:
                 var value = Eval(returnStatement.ReturnValue, environment);
                 return value is MError ? value : new MReturnValue { Value = value };
            case LetStatement letStatement:
                 var val = Eval(letStatement.Value, environment);
                 if (val is MError) return val;
                 environment.Set(letStatement.Name.Value, val!);
                 break;
            case FunctionExpression functionExpression:
                 return new MFunction
                 {
                     Parameters = functionExpression.Parameters,
                     Body = functionExpression.Body,
                     Env = environment
                 };
            case CallExpression callExpression:
                var function = Eval(callExpression.Function, environment);
                if (function is MError) return function;
                var args = EvalExpressions(callExpression.Arguments, environment);
                if (args.Count == 1 && args[0] is MError) return args[0];
                return applyFunction(function, args);
            default:
                throw new ArgumentException($"Unknown node type: {node.GetType().Name}");
        }

        return null;
    }

    private static IMObject? applyFunction(IMObject? function, List<IMObject> args)
    {
        if (function is MFunction fn)
        {
            var extendedEnv = ExtendFunctionEnv(fn, args);
            var evaluated = Eval(fn.Body!, extendedEnv);
            return UnwrapReturnValue(evaluated);
        }

        // if (function is MBuiltin builtin)
        // {
        //     return builtin.Fn(args);
        // }

        return NewError("not a function: {0}", function?.Type() ?? "null");
    }

    private static IMObject? UnwrapReturnValue(IMObject? obj)
    {
        return obj switch
        {
            MReturnValue returnValue => returnValue.Value,
            _ => obj
        };
    }

    private static Environment ExtendFunctionEnv(MFunction fn, IReadOnlyList<IMObject> args)
    {
        var env = new Environment { Outer = fn.Env };

        for (var i = 0; i < fn.Parameters.Count; i++)
        {
            env.Set(fn.Parameters[i].Value, args[i]);
        }

        return env;
    }

    private static List<IMObject> EvalExpressions(List<IExpression> exps, Environment env)
    {
        var result = new List<IMObject>();

        foreach (var exp in exps)
        {
            var evaluated = Eval(exp, env);
            
            if (evaluated is MError) return [evaluated];
            
            result.Add(evaluated!);
        }
        
        return result;
    }

    private static IMObject? EvalIdentifierExpression(IdentifierExpression identifierExpression, Environment environment)
    {
        if (environment.TryGet(identifierExpression.Value, out var value))
        {
            return value;
        }

        return NewError("identifier not found: " + identifierExpression.Value);
    }

    private static IMObject? NewError(string message, params object[] args)
        => new MError { Message = string.Format(message, args) };

    private static IMObject? EvalIfExpression(IfExpression ifExpression, Environment environment)
    {
        var condition = Eval(ifExpression.Condition, environment);
        if (condition is MError) return condition;
        
        if (IsTruthy(condition))
        {
            return Eval(ifExpression.Consequence, environment);
        }
        
        if (ifExpression.Alternative != null)
        {
            return Eval(ifExpression.Alternative, environment);
        }
        
        return Null;
    }

    private static bool IsTruthy(IMObject? condition)
    {
        return condition switch
        {
            MNull => false,
            MBoolean boolean => boolean.Value,
            MInteger { Value: 0 } => false,
            _ => true
        };
    }

    private static IMObject? EvalInfixExpression(string infixExpressionOperator, IMObject? left, IMObject? right)
    {
        if (left is MInteger leftInteger && right is MInteger rightInteger)
        {
            return EvalIntegerInfixExpression(infixExpressionOperator, leftInteger, rightInteger);
        }

        if (left?.Type() != right?.Type()) 
        {
            return NewError("type mismatch: {0} {1} {2}", left.Type(), infixExpressionOperator, right.Type());
        }
        
        if (infixExpressionOperator == "==")
        {
             return left == right ? True : False;
        }
        
        if (infixExpressionOperator == "!=")
        {
            return left != right ? True : False;
        }
        
        return NewError("unknown operator: {0} {1} {2}", left.Type(), infixExpressionOperator, right.Type());
    }

    private static IMObject? EvalIntegerInfixExpression(string infixExpressionOperator, MInteger leftInteger, MInteger rightInteger)
    {
        var left = leftInteger.Value;
        var right = rightInteger.Value;

        return infixExpressionOperator switch
        {
            "+" => new MInteger { Value = left + right },
            "-" => new MInteger { Value = left - right },
            "*" => new MInteger { Value = left * right },
            "/" => new MInteger { Value = left / right },
            "<" => left < right ? True : False,
            ">" => left > right ? True : False,
            "==" => left == right ? True : False,
            "!=" => left != right ? True : False,
            _ => NewError("unknown operator: {0} {1} {2}", left, infixExpressionOperator, right)
        };
    }
    
    private static IMObject? EvalPrefixExpression(string prefixExpressionOperator, IMObject? right)
    {
        return prefixExpressionOperator switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => NewError("unknown operator: {0}{1}", prefixExpressionOperator, right?.Type() ?? "null")
        };
    }

    private static IMObject? EvalMinusPrefixOperatorExpression(IMObject? right)
    {
        if (right is not MInteger integer)
        {
            return NewError("unknown operator: -{0}", right?.Type() ?? "null");
        }

        integer.Value = -integer.Value;
        return integer;
    }

    private static IMObject? EvalBangOperatorExpression(IMObject? right)
    {
        return right switch
        {
            MBoolean boolean => boolean.Value ? False : True,
            MNull => True,
            _ => False
        };
    }

    private static IMObject? EvalProgramme(List<IStatement> statements, Environment environment)    
    {
        IMObject? result = null;
        
        foreach (var statement in statements)
        {
            result = Eval(statement, environment);
            
            switch (result)
            {
                case MReturnValue returnValue:
                    return returnValue.Value;
                case MError:
                    return result;
            }
        }

        return result;
    }

    private static IMObject? EvalBlockStatement(BlockStatement block, Environment environment)
    {
        IMObject? result = null;
        
        foreach (var statement in block.Statements)
        {
            result = Eval(statement, environment);

            if (result is MReturnValue or MError) 
            {
                return result;
            }
        }

        return result;
    }
}
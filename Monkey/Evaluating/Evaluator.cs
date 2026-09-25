using Monkey.Evaluating.Objects;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating;

public static class Evaluator
{
    public static MonkeyObject Eval(Node node, Context context)
    {
        switch (node)
        {
             case ProgramNode program:
                 return EvalProgramme(program.Statements, context);

             case ExpressionNode expression:
                 return Eval(expression.Expression, context);

             case IntegerNode integer:
                 return new IntegerObject(integer.Value);

             case BooleanNode boolean:
                 return boolean.Value ? Builtin.True : Builtin.False;

             case PrefixNode prefix:
                 var right = Eval(prefix.Right, context);
                 return right is ErrorObject ? right : EvalPrefixExpression(prefix.Operator, right);

             case InfixNode infix:
                  var left = Eval(infix.Left, context);
                  if (left is ErrorObject) return left;
                  var r = Eval(infix.Right, context);
                  return r is ErrorObject ? r : EvalInfixExpression(infix.Operator, left, r);

             case IfNode ifExpression:
                  return EvalIfExpression(ifExpression, context);

             case IdentifierNode identifier:
                  return EvalIdentifierExpression(identifier, context);

             case BlockNode block:
                  return EvalBlockStatement(block, context);

             case ReturnNode returnStatement:
                  var value = Eval(returnStatement.ReturnValue, context);
                  return value is ErrorObject ? value : new ReturnObject(value);

             case LetNode let:
                  var val = Eval(let.Value, context);
                  if (val is ErrorObject) return val;
                  context.Set(let.Name.Value, val);
                  break;

             case FunctionNode function:
                  return new FunctionObject(function.Parameters, function.Body, context);

             case StringNode str:
                  return new StringObject(str.Value);

             case CallNode call:
                 var func = Eval(call.Function, context);
                 if (func is ErrorObject) return func;
                 var args = EvalExpressions(call.Arguments, context);
                 if (args.Count == 1 && args[0] is ErrorObject) return args[0];
                 return ApplyFunction(func, args);

             case ArrayNode array:
                 var elements = EvalExpressions(array.Elements, context);
                 if (elements.Count == 1 && elements[0] is ErrorObject) return elements[0];
                 return new ArrayObject { Elements = elements };

             case IndexNode index:
                 var l = Eval(index.Left, context);
                 if (l is ErrorObject) return l;
                 var idx = Eval(index.Index, context);
                 return idx is ErrorObject ? idx : EvalIndexExpression(l, idx);

             case HashNode hash:
                 return EvalHashLiteral(hash, context);

             default:
                 throw new InvalidOperationException($"Unknown node type: {node.GetType().Name}");
         }

        return Builtin.Null;
    }

    #region private methods

    private static MonkeyObject EvalProgramme(List<Node> statements, Context context)
    {
        MonkeyObject result = Builtin.Null;

        foreach (var statement in statements)
        {
            result = Eval(statement, context);

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

    private static MonkeyObject EvalBlockStatement(BlockNode block, Context context)
    {
        MonkeyObject result = Builtin.Null;

        foreach (var statement in block.Statements)
        {
            result = Eval(statement, context);

            if (result is ReturnObject or ErrorObject)
            {
                return result;
            }
        }

        return result;
    }

    private static MonkeyObject EvalPrefixExpression(string op, MonkeyObject right)
    {
        return op switch
        {
            "!" => EvalBangOperatorExpression(right),
            "-" => EvalMinusPrefixOperatorExpression(right),
            _ => ErrorObject.Create("unknown operator: {0}{1}", op, right.Type)
        };
    }

    private static BooleanObject EvalBangOperatorExpression(MonkeyObject right)
        => right switch
        {
            BooleanObject boolean => boolean.Value ? Builtin.False : Builtin.True,
            NullObject => Builtin.True,
            _ => Builtin.False
        };

    private static MonkeyObject EvalMinusPrefixOperatorExpression(MonkeyObject right)
    {
        if (right is not IntegerObject integer)
        {
            return ErrorObject.Create("unknown operator: -{0}", right.Type);
        }

        var value = integer.Value;
        return new IntegerObject(-value);
    }

    private static MonkeyObject EvalInfixExpression(string op, MonkeyObject? left, MonkeyObject? right)
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

    private static MonkeyObject EvalIntegerInfixExpression(string op, IntegerObject leftInteger, IntegerObject rightInteger)
    {
        var left = leftInteger.Value;
        var right = rightInteger.Value;

        return op switch
        {
            "+" => new IntegerObject(left + right),
            "-" => new IntegerObject(left - right),
            "*" => new IntegerObject(left * right),
            "/" when right == 0 => ErrorObject.Create("division by zero: {0} / {1}", left, right),
            "/" => new IntegerObject(left / right),
            "<" => left < right ? Builtin.True : Builtin.False,
            ">" => left > right ? Builtin.True : Builtin.False,
            "==" => left == right ? Builtin.True : Builtin.False,
            "!=" => left != right ? Builtin.True : Builtin.False,
            _ => ErrorObject.Create("unknown operator: {0} {1} {2}", leftInteger.Type, op, rightInteger.Type)
        };
    }

    private static MonkeyObject EvalStringInfixExpression(string op, StringObject left, StringObject right)
    {
        return op == "+"
            ? new StringObject(left.Value + right.Value)
            : ErrorObject.Create("unknown operator: {0} {1} {2}", left.Type, op, right.Type);
    }

    private static MonkeyObject EvalIfExpression(IfNode ifExpression, Context context)
    {
        var condition = Eval(ifExpression.Condition, context);

        if (condition is ErrorObject) return condition;
        if (IsTruthy(condition)) return Eval(ifExpression.Consequence, context);

        return ifExpression.Alternative != null
            ? Eval(ifExpression.Alternative, context)
            : Builtin.Null;
    }

    private static bool IsTruthy(MonkeyObject? condition)
    {
        return condition switch
        {
            NullObject => false,
            BooleanObject boolean => boolean.Value,
            _ => true
        };
    }

    private static MonkeyObject EvalIdentifierExpression(IdentifierNode identifier, Context context)
    {
        if (context.TryGet(identifier.Value, out var value)) return value ?? Builtin.Null;

        return Builtin.Functions.TryGetValue(identifier.Value, out var builtin)
            ? builtin
            : ErrorObject.Create("identifier not found: " + identifier.Value);
    }

    private static List<MonkeyObject> EvalExpressions(List<Node> exps, Context context)
    {
        var result = new List<MonkeyObject>();

        foreach (var exp in exps)
        {
            var evaluated = Eval(exp, context);
            if (evaluated is ErrorObject) return [evaluated];

            result.Add(evaluated);
        }

        return result;
    }

    private static MonkeyObject ApplyFunction(MonkeyObject? function, List<MonkeyObject> args)
    {
        switch (function)
        {
            case FunctionObject func:
            {
                var extendedContext = ExtendContext(func, args);
                var evaluated = Eval(func.Body, extendedContext);

                return UnwrapReturnValue(evaluated);
            }
            case BuiltinObject builtin:
                return builtin.Function(args);
            default:
                return ErrorObject.Create("not a function: {0}", function?.Type ?? "null");
        }
    }

    private static Context ExtendContext(FunctionObject function, IReadOnlyList<MonkeyObject> args)
    {
        var context = new Context { Outer = function.Context };

        for (var i = 0; i < function.Parameters.Count; i++)
        {
            context.Set(function.Parameters[i].Value, args[i]);
        }

        return context;
    }

    private static MonkeyObject UnwrapReturnValue(MonkeyObject obj)
        => obj switch
            {
                ReturnObject returnValue => returnValue.Value,
                _ => obj
            };

    private static MonkeyObject EvalHashLiteral(HashNode hash, Context context)
    {
        var pairs = new Dictionary<HashKey, KeyValuePair<MonkeyObject, MonkeyObject>>();

        foreach (var (keyNode, valueNode) in hash.Pairs)
        {
            var key = Eval(keyNode, context);
            if (key is ErrorObject) return key;

            if (key is not IHashable hashable)
                return ErrorObject.Create("unusable as hash key: {0}", key.Type);

            var value = Eval(valueNode, context);
            if (value is ErrorObject) return value;

            pairs[hashable.HashKey] = new KeyValuePair<MonkeyObject, MonkeyObject>(key, value);
        }

        return new HashObject(pairs);
    }

    private static MonkeyObject EvalIndexExpression(MonkeyObject? left, MonkeyObject index)
    {
        return (left, index) switch
        {
            (ArrayObject array, IntegerObject integer) => EvalArrayIndexExpression(array, integer),
            (HashObject hash, _) => EvalHashIndexExpression(hash, index),
            _ => ErrorObject.Create("index operator not supported: {0}", left?.Type ?? "null")
        };
    }

    private static MonkeyObject EvalArrayIndexExpression(ArrayObject array, IntegerObject integer)
    {
        var index = integer.Value;
        var max = array.Elements.Count - 1;

        if (index < 0 || index > max)
        {
            return Builtin.Null;
        }

        return array.Elements[index];
    }

    private static MonkeyObject EvalHashIndexExpression(HashObject hash, MonkeyObject index)
    {
        if (index is not IHashable hashable)
            return ErrorObject.Create("unusable as hash key: {0}", index.Type);

        return hash.Pairs.TryGetValue(hashable.HashKey, out var pair) ? pair.Value : Builtin.Null;
    }

    #endregion
}

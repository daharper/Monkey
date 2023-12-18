using Monkey.Evaluating.Objects;
using Monkey.Parsing.Nodes;
using Monkey.Utils;

namespace Monkey.Evaluating;

public static partial class Evaluator
{
    public static MObject Eval(Node node, Environment env)
    {
        switch (node)
        {
             case ProgramNode program:
                 return EvalProgramme(program.Statements, env);
             
             case ExpressionNode expression:
                 return Eval(expression.Expression, env);
             
             case IntegerNode integer:
                 return new IntegerObject(integer.Value);
             
             case BooleanNode boolean:
                 return boolean.Value ? Builtin.True : Builtin.False;
             
             case PrefixNode prefix:
                 var right = Eval(prefix.Right, env);
                 return right is ErrorObject ? right : EvalPrefixExpression(prefix.Operator, right);

             case InfixNode infix:
                  var left = Eval(infix.Left, env);
                  if (left is ErrorObject) return left;
                  var r = Eval(infix.Right, env);
                  return r is ErrorObject ? r : EvalInfixExpression(infix.Operator, left, r);

             case IfNode ifExpression:
                  return EvalIfExpression(ifExpression, env);
             
             case IdentifierNode identifier:
                  return EvalIdentifierExpression(identifier, env);
             
             case BlockNode block:
                  return EvalBlockStatement(block, env);
             
             case ReturnNode returnStatement:
                  var value = Eval(returnStatement.ReturnValue, env);
                  return value is ErrorObject ? value : new ReturnObject(value);
             
             case LetNode let:
                  var val = Eval(let.Value, env);
                  if (val is ErrorObject) return val;
                  env.Set(let.Name.Value, val);
                  break;
             
             case FunctionNode function:
                  return new FunctionObject(function.Parameters, function.Body, env);
             
             case StringNode str:
                  return new StringObject(str.Value);
             
             case CallNode call:
                 var fn = Eval(call.Function, env);
                 if (fn is ErrorObject) return fn;
                 var args = EvalExpressions(call.Arguments, env);
                 if (args.Count == 1 && args[0] is ErrorObject) return args[0];
                 return ApplyFunction(fn, args);
             
             case ArrayNode array:
                 var elements = EvalExpressions(array.Elements, env);
                 if (elements.Count == 1 && elements[0] is ErrorObject) return elements[0];
                 return new ArrayObject { Elements = elements };
             
             case IndexNode index:
                 var l = Eval(index.Left, env);
                 if (l is ErrorObject) return l;
                 var idx = Eval(index.Index, env);
                 return idx is ErrorObject ? idx : EvalIndexExpression(l, idx);

             case HashNode:
                 return EvalHashLiteral(node, env);
             
             default:
                 throw Fatal.Error($"Unknown node type: {node.GetType().Name}");
         }

        return Builtin.Null;
    }

    #region private methods
    
    private static MObject ApplyFunction(MObject? function, List<MObject> args)
    {
        switch (function)
        {
            case FunctionObject fn:
            {
                var extendedEnv = ExtendFunctionEnv(fn, args);
                var evaluated = Eval(fn.Body, extendedEnv);
            
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
    
    #endregion
}
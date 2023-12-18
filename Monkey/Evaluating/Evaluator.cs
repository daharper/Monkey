using Monkey.Evaluating.Objects;
using Monkey.Parsing.Nodes;
using Monkey.Utils;

namespace Monkey.Evaluating;

public static partial class Evaluator
{
    public static MObject Eval(Node node, Context context)
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

             case HashNode:
                 return EvalHashLiteral(node, context);
             
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

    private static MObject UnwrapReturnValue(MObject obj)
        => obj switch
            {
                ReturnObject returnValue => returnValue.Value,
                _ => obj
            };
    
    private static Context ExtendContext(FunctionObject function, IReadOnlyList<MObject> args)
    {
        var context = new Context { Outer = function.Context };

        for (var i = 0; i < function.Parameters.Count; i++)
        {
            context.Set(function.Parameters[i].Value, args[i]);
        }

        return context;
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
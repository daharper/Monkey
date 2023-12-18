using System.Collections.Immutable;
using Monkey.Evaluating.Objects;

namespace Monkey.Evaluating;

public static class Builtin
{
    public static readonly NullObject Null = new();
    public static readonly BooleanObject True = new() { Value = true };
    public static readonly BooleanObject False = new() { Value = false };

    public static readonly ImmutableDictionary<string, BuiltinObject> Functions = 
        new Dictionary<string, BuiltinObject>
        {
            ["len"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalLen(args[0])   
                    : ErrorObject.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["first"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalFirst(args[0]) 
                    : ErrorObject.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["last"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalLast(args[0])  
                    : ErrorObject.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["rest"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalRest(args[0])  
                    : ErrorObject.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["push"] = new()
            {
                Function = args => args.Count == 2 
                    ? EvalPush(args[0], args[1]) 
                    : ErrorObject.Create("wrong number of arguments. got={0}, want=2", args.Count)
            },
            ["puts"] = new()
            {
                Function = args => { foreach (var arg in args) Console.WriteLine(arg); return Null; } 
            }
        }.ToImmutableDictionary();

    private static IObject EvalPush(IObject container, IObject item)
        => container switch
        {
            ArrayObject array => new ArrayObject { Elements = array.Elements.Append(item).ToList() },
            _ => ErrorObject.Create("argument to `push` must be ARRAY, got {0}", container.Type())
        };        
    
    private static IObject EvalRest(IObject container)
        => container switch
        {
            ArrayObject array => array.Elements.Count > 0 
                ? new ArrayObject { Elements = array.Elements.Skip(1).ToList() } 
                : Null,
            _ => ErrorObject.Create("argument to `rest` must be ARRAY, got {0}", container.Type())
        };
    
    private static IObject EvalLast(IObject container)
        => container switch
        {
            ArrayObject array => array.Elements.Count > 0 ? array.Elements[^1] : Null,
            _ => ErrorObject.Create("argument to `last` must be ARRAY, got {0}", container.Type())
        };
    
    private static IObject EvalFirst(IObject container)
        => container switch
        {
            ArrayObject array => array.Elements.Count > 0 ? array.Elements[0] : Null,
            _ => ErrorObject.Create("argument to `first` must be ARRAY, got {0}", container.Type())
        };

    private static IObject EvalLen(IObject obj)
        => obj switch
        {
            ArrayObject array => new IntegerObject { Value = array.Elements.Count },
            StringObject str => new IntegerObject { Value = str.Value.Length },
            _ => ErrorObject.Create("argument to `len` not supported, got {0}", obj.Type())
        };
}
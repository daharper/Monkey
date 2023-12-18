using System.Collections.Immutable;
using Monkey.Evaluating.Ast;

namespace Monkey.Evaluating;

public static class Builtin
{
    public static readonly AstNull Null = new();
    public static readonly AstBoolean True = new() { Value = true };
    public static readonly AstBoolean False = new() { Value = false };

    public static readonly ImmutableDictionary<string, AstBuiltin> Functions = 
        new Dictionary<string, AstBuiltin>
        {
            ["len"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalLen(args[0])   
                    : AstError.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["first"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalFirst(args[0]) 
                    : AstError.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["last"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalLast(args[0])  
                    : AstError.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["rest"] = new()
            {
                Function = args => args.Count == 1 
                    ? EvalRest(args[0])  
                    : AstError.Create("wrong number of arguments. got={0}, want=1", args.Count)
            },
            ["push"] = new()
            {
                Function = args => args.Count == 2 
                    ? EvalPush(args[0], args[1]) 
                    : AstError.Create("wrong number of arguments. got={0}, want=2", args.Count)
            },
            ["puts"] = new()
            {
                Function = args => { foreach (var arg in args) Console.WriteLine(arg.Inspect()); return Null; } 
            }
        }.ToImmutableDictionary();

    private static IAstObject EvalPush(IAstObject container, IAstObject item)
        => container switch
        {
            AstArray array => new AstArray { Elements = array.Elements.Append(item).ToList() },
            _ => AstError.Create("argument to `push` must be ARRAY, got {0}", container.Type())
        };        
    
    private static IAstObject EvalRest(IAstObject container)
        => container switch
        {
            AstArray array => array.Elements.Count > 0 
                ? new AstArray { Elements = array.Elements.Skip(1).ToList() } 
                : Null,
            _ => AstError.Create("argument to `rest` must be ARRAY, got {0}", container.Type())
        };
    
    private static IAstObject EvalLast(IAstObject container)
        => container switch
        {
            AstArray array => array.Elements.Count > 0 ? array.Elements[^1] : Null,
            _ => AstError.Create("argument to `last` must be ARRAY, got {0}", container.Type())
        };
    
    private static IAstObject EvalFirst(IAstObject container)
        => container switch
        {
            AstArray array => array.Elements.Count > 0 ? array.Elements[0] : Null,
            _ => AstError.Create("argument to `first` must be ARRAY, got {0}", container.Type())
        };

    private static IAstObject EvalLen(IAstObject obj)
        => obj switch
        {
            AstArray array => new AstInteger { Value = array.Elements.Count },
            AstString str => new AstInteger { Value = str.Value.Length },
            _ => AstError.Create("argument to `len` not supported, got {0}", obj.Type())
        };
}
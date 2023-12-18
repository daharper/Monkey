namespace Monkey.Evaluating.Objects;

public class AstBuiltin : IAstObject
{
    public Func<List<IAstObject>, IAstObject> Fn { get; init; }
    
    public string Type() => AstTypes.BuiltinObj;

    public string Inspect() => "builtin function";
}
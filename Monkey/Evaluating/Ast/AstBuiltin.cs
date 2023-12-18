namespace Monkey.Evaluating.Ast;

public class AstBuiltin : IAstObject
{
    public Func<List<IAstObject>, IAstObject> Function { get; init; } = null!;
    
    public string Type() => AstTypes.BuiltinObj;
    
    public override string ToString() => "builtin function";
}
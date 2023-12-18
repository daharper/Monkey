namespace Monkey.Evaluating.Objects;

public class BuiltinObject : IObject
{
    public Func<List<IObject>, IObject> Function { get; init; } = null!;
    
    public string Type() => ObjectTypes.BuiltinObj;
    
    public override string ToString() => "builtin function";
}
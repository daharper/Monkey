namespace Monkey.Evaluating.Objects;

public class BuiltinObject() : MObject(ObjectTypes.Builtin)
{
    public Func<List<MObject>, MObject> Function { get; init; } = null!;
    
    public override string ToString() => "builtin function";
}
namespace Monkey.Evaluating.Objects;

public class BuiltinObject() : MonkeyObject(ObjectTypes.Builtin)
{
    public Func<List<MonkeyObject>, MonkeyObject> Function { get; init; } = null!;
    
    public override string ToString() => "builtin function";
}
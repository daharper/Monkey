namespace Monkey.Evaluating.Objects;

public class ReturnObject(MObject? value) : MObject(ObjectTypes.Return)
{
    public MObject Value { get; } = value ?? Builtin.Null;

    public override string ToString() => Value.ToString() ?? "";
}
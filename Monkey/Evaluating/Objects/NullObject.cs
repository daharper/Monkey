namespace Monkey.Evaluating.Objects;

public class NullObject() : MObject(ObjectTypes.Null)
{
    public override string ToString() => "null";
}
namespace Monkey.Evaluating.Objects;

public class NullObject() : MonkeyObject(ObjectTypes.Null)
{
    public override string ToString() => "null";
}
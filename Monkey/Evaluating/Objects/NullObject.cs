namespace Monkey.Evaluating.Objects;

public class NullObject : IObject
{
    public string Type() => ObjectTypes.NullObj;
    
    public override string ToString() => "null";
}
namespace Monkey.Evaluating.Objects;

public class ArrayObject() : MonkeyObject(ObjectTypes.Array)
{
    private List<MonkeyObject>? _elements;

    public List<MonkeyObject> Elements
    {
        get => _elements ??= [];
        init => _elements = value;
    }

    public override string ToString()
    {
        var str = _elements is null 
            ? "" 
            : string.Join(", ", Elements.Select(e => e.ToString()));
        
        return $"[{str}]";
    }
}
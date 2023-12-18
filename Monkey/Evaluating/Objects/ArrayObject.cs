namespace Monkey.Evaluating.Objects;

public class ArrayObject() : MObject(ObjectTypes.Array)
{
    private List<MObject>? _elements;

    public List<MObject> Elements
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
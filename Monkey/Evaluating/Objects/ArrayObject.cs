namespace Monkey.Evaluating.Objects;

public class ArrayObject : IObject
{
    private List<IObject>? _elements;

    public List<IObject> Elements
    {
        get => _elements ??= [];
        init => _elements = value;
    }
    
    public string Type() => ObjectTypes.ArrayObj;

    public override string ToString()
    {
        var s = _elements is null ? "" : string.Join(", ", Elements.Select(e => e.ToString()));
        return $"[{s}]";
    }
}
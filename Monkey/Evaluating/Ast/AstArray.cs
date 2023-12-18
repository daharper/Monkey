namespace Monkey.Evaluating.Ast;

public class AstArray : IAstObject
{
    private List<IAstObject>? _elements;

    public List<IAstObject> Elements
    {
        get => _elements ??= [];
        init => _elements = value;
    }
    
    public string Type() => AstTypes.ArrayObj;

    public override string ToString()
    {
        var s = _elements is null ? "" : string.Join(", ", Elements.Select(e => e.ToString()));
        return $"[{s}]";
    }
}
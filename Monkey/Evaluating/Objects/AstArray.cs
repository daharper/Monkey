namespace Monkey.Evaluating.Objects;

public class AstArray : IAstObject
{
    public List<IAstObject> Elements { get; init; } = [];
    
    public string Type() => AstTypes.ArrayObj;

    public string Inspect()
    {
        var elements = new List<string>();
        
        foreach (var element in Elements)
        {
            elements.Add(element.Inspect());
        }
        
        return $"[{string.Join(", ", elements)}]";
    }
}
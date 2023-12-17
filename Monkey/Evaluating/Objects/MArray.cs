namespace Monkey.Evaluating.Objects;

public class MArray : IMObject
{
    public List<IMObject> Elements { get; set; } = new();
    
    
    public string Type() => Types.ArrayObj;

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
namespace Monkey.Evaluating.Objects;

public class AstHash : IAstObject
{
    public Dictionary<int, KeyValuePair<IAstObject, IAstObject?>> Pairs { get; set; } = new();
    
    public string Type() => AstTypes.HashObj;

    public string Inspect()
    {
        var pairs = Pairs.Values.Select(pair => $"{pair.Key.Inspect()}: {pair.Value?.Inspect() ?? string.Empty}");
        return $"{{{string.Join(", ", pairs)}}}";
    }
}
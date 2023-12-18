namespace Monkey.Evaluating.Objects;

public class MHash : IMObject
{
    public Dictionary<int, KeyValuePair<IMObject, IMObject?>> Pairs { get; set; } = new();
    
    public string Type() => Types.HashObj;

    public string Inspect()
    {
        var pairs = Pairs.Values.Select(pair => $"{pair.Key.Inspect()}: {pair.Value?.Inspect() ?? string.Empty}");
        return $"{{{string.Join(", ", pairs)}}}";
    }
}
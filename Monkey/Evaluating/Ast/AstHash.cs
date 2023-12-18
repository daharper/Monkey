namespace Monkey.Evaluating.Ast;

public class AstHash(Dictionary<int, KeyValuePair<IAstObject, IAstObject?>> pairs) : IAstObject
{
    public Dictionary<int, KeyValuePair<IAstObject, IAstObject?>> Pairs { get; } = pairs;

    public string Type() => AstTypes.HashObj;

    public string Inspect()
    {
        var p = Pairs.Values.Select(pair => $"{pair.Key.Inspect()}: {pair.Value?.Inspect() ?? string.Empty}");
        return $"{{{string.Join(", ", p)}}}";
    }
}
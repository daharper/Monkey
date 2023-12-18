namespace Monkey.Evaluating.Ast;

public class AstHash(Dictionary<int, KeyValuePair<IAstObject, IAstObject>> pairs) : IAstObject
{
    public Dictionary<int, KeyValuePair<IAstObject, IAstObject>> Pairs { get; } = pairs;

    public string Type() => AstTypes.HashObj;

    public override string ToString() 
    {
        var p = Pairs.Values.Select(pair => $"{pair.Key}: {pair.Value}");
        return $"{{{string.Join(", ", p)}}}";
    }
}
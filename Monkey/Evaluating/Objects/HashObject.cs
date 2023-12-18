namespace Monkey.Evaluating.Objects;

public class HashObject(Dictionary<int, KeyValuePair<IObject, IObject>> pairs) : IObject
{
    public Dictionary<int, KeyValuePair<IObject, IObject>> Pairs { get; } = pairs;

    public string Type() => ObjectTypes.HashObj;

    public override string ToString() 
    {
        var p = Pairs.Values.Select(pair => $"{pair.Key}: {pair.Value}");
        return $"{{{string.Join(", ", p)}}}";
    }
}
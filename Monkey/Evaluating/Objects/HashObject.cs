namespace Monkey.Evaluating.Objects;

public class HashObject(Dictionary<int, KeyValuePair<MObject, MObject>> pairs) 
    : MObject(ObjectTypes.Hash)
{
    public Dictionary<int, KeyValuePair<MObject, MObject>> Pairs { get; } = pairs;

    public override string ToString() 
    {
        var items = Pairs.Values.Select(pair => $"{pair.Key}: {pair.Value}");
        return $"{{{string.Join(", ", items)}}}";
    }
}
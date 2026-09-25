namespace Monkey.Evaluating.Objects;

public class HashObject(Dictionary<HashKey, KeyValuePair<MonkeyObject, MonkeyObject>> pairs)
    : MonkeyObject(ObjectTypes.Hash)
{
    public Dictionary<HashKey, KeyValuePair<MonkeyObject, MonkeyObject>> Pairs { get; } = pairs;

    public override string ToString()
    {
        var items = Pairs.Values.Select(pair => $"{pair.Key}: {pair.Value}");
        return $"{{{string.Join(", ", items)}}}";
    }
}

using Monkey.Evaluating.Objects;

namespace Monkey.Evaluating;

public class Context(Context? outer = null)
{
    private readonly Dictionary<string, MonkeyObject> _store = new();

    public Context? Outer { get; init; } = outer;

    public bool TryGet(string name, out MonkeyObject? result)
    {
        if (_store.TryGetValue(name, out result)) return true;
        
        if (Outer != null) return Outer.TryGet(name, out result);

        result = null;
        return false;
    }
    
    public MonkeyObject Set(string name, MonkeyObject value)
    {
        _store[name] = value;
        return value;
    }
}
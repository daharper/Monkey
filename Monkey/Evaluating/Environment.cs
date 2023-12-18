using Monkey.Evaluating.Objects;

namespace Monkey.Evaluating;

public class Environment(Environment? outer = null)
{
    private readonly Dictionary<string, IObject> _store = new();

    public Environment? Outer { get; init; } = outer;

    public bool TryGet(string name, out IObject? result)
    {
        if (_store.TryGetValue(name, out result)) 
            return true;
        
        if (Outer != null) 
            return Outer.TryGet(name, out result);

        result = null;
        return false;
    }
    
    public IObject Set(string name, IObject value)
    {
        _store[name] = value;
        return value;
    }
}
using Monkey.Evaluating.Objects;

namespace Monkey.Evaluating;

public class Environment(Environment? outer = null)
{
    private readonly Dictionary<string, IAstObject> _store = new();

    public Environment? Outer { get; init; } = outer;

    public bool TryGet(string name, out IAstObject? result)
    {
        if (_store.TryGetValue(name, out result)) 
            return true;
        
        if (Outer != null) 
            return Outer.TryGet(name, out result);

        result = null;
        return false;
    }
    
    public IAstObject Set(string name, IAstObject value)
    {
        _store[name] = value;
        return value;
    }
}
using Monkey.Evaluating.Objects;

namespace Monkey.Evaluating;

public class Environment(Environment? outer = null)
{
    private readonly Dictionary<string, IMObject> _store = new();

    public Environment? Outer { get; set; } = outer;

    public bool TryGet(string name, out IMObject? result)
    {
        if (_store.TryGetValue(name, out result)) 
            return true;
        
        if (Outer != null) 
            return Outer.TryGet(name, out result);

        result = null;
        return false;
    }
    
    public IMObject Set(string name, IMObject value)
    {
        _store[name] = value;
        return value;
    }
}
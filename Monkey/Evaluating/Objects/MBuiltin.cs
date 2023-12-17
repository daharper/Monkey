namespace Monkey.Evaluating.Objects;

//public delegate IMObject BuiltinFunction(params IMObject[] args);

public class MBuiltin : IMObject
{
    public Func<List<IMObject>, IMObject> Fn { get; set; }
    
    public string Type() => Types.BuiltinObj;

    public string Inspect() => "builtin function";
}
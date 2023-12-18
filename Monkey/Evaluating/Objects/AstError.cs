namespace Monkey.Evaluating.Objects;

public class AstError : IAstObject
{
    public string Message { get; init; } = "";

    public string Type() => AstTypes.ErrorObj;

    public string Inspect() => $"ERROR: {Message}";
    
    public static IAstObject Create(string message, params object[] args)
        => new AstError { Message = string.Format(message, args) };
}
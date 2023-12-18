namespace Monkey.Evaluating.Ast;

public class AstError(string message) : IAstObject
{
    public string Message { get; } = message;

    public string Type() => AstTypes.ErrorObj;

    public override string ToString() => $"ERROR: {Message}";

    public static IAstObject Create(string message, params object[] args)
        => new AstError(string.Format(message, args));
}
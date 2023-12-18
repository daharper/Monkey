namespace Monkey.Evaluating.Objects;

public class ErrorObject(string message) : IObject
{
    public string Message { get; } = message;

    public string Type() => ObjectTypes.ErrorObj;

    public override string ToString() => $"ERROR: {Message}";

    public static IObject Create(string message, params object[] args)
        => new ErrorObject(string.Format(message, args));
}
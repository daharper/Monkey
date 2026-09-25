namespace Monkey.Evaluating.Objects;

public class ErrorObject(string message) : MonkeyObject(ObjectTypes.Error)
{
    public string Message { get; } = message;

    public override string ToString() => $"ERROR: {Message}";

    public static MonkeyObject Create(string message, params object[] args)
        => new ErrorObject(string.Format(message, args));
}
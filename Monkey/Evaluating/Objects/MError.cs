namespace Monkey.Evaluating.Objects;

public class MError : IMObject
{
    public string Message { get; set; } = "";

    public string Type() => Types.ErrorObj;

    public string Inspect() => $"ERROR: {Message}";
}
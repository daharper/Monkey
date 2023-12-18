namespace Monkey.Utils;

public static class Fatal
{
    public static Exception Error(string message)
        => new InvalidProgramException($"ERROR: {message}");
}
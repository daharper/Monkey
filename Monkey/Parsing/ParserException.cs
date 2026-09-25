using Monkey.Lexing;

namespace Monkey.Parsing;

/// <summary>
/// Thrown when the parser meets a token it cannot continue from.
/// </summary>
public class ParserException(string message, Token token) : Exception(message)
{
    /// <summary>
    /// The token that caused the error.
    /// </summary>
    public Token Token { get; } = token;
}

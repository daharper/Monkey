namespace Monkey.Evaluating.Objects;

/// <summary>
/// Implemented by objects that can be used as hash keys.
/// </summary>
public interface IHashable
{
    HashKey HashKey { get; }
}

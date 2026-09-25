namespace Monkey.Evaluating.Objects;

/// <summary>
/// Key used to store a value in a hash. Including the object type keeps
/// keys of different types apart, e.g. 1 and true.
/// </summary>
public readonly record struct HashKey(string Type, int Value);

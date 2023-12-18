namespace Monkey.Evaluating.Objects;

/// <summary>
/// Monkey Object - base object in the Monkey programming language,
/// instantiated from the ast and evaluated by the evaluator.
/// </summary>
public abstract class MObject(string type)
{
    public string Type { get; } = type;
}
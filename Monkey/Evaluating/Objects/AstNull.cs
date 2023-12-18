namespace Monkey.Evaluating.Objects;

public class AstNull : IAstObject
{
    public string Type() => AstTypes.NullObj;

    public string Inspect() => "null";
}
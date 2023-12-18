using System.Text;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating.Objects;

public class FunctionObject(List<IdentifierNode> parameters, BlockNode? body, Environment? env) : IObject
{
    public List<IdentifierNode> Parameters { get; private set; } = parameters;

    public BlockNode? Body { get; init; } = body;

    public Environment? Env { get; init; } = env;

    public string Type() => ObjectTypes.FunctionObj;

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("fn(");
        sb.Append(string.Join(", ", Parameters));
        sb.AppendLine(") {");
        sb.Append(Body);
        sb.AppendLine("}");

        return sb.ToString();
    }
}
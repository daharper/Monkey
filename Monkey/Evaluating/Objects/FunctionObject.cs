using System.Text;
using Monkey.Parsing.Nodes;

namespace Monkey.Evaluating.Objects;

public class FunctionObject(List<IdentifierNode> parameters, BlockNode body, Context context) 
    : MObject(ObjectTypes.Function)
{
    public List<IdentifierNode> Parameters { get; private set; } = parameters;

    public BlockNode Body { get; } = body;

    public Context Context { get; } = context;

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
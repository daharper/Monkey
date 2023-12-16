using System.Text;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Literals;
using Monkey.Parsing.Statements;

namespace Monkey.Evaluating.Objects;

public class MFunction : IMObject
{
    public List<Identifier> Parameters { get; set; } = new();
    public BlockStatement? Body { get; set; }
    
    public Environment? Env { get; set; }

    public string Type() => Types.FunctionObj;

    public string Inspect()
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
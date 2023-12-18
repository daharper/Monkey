using System.Text;
using Monkey.Parsing.Literals;
using Monkey.Parsing.Statements;

namespace Monkey.Evaluating.Ast;

public class AstFunction(List<Identifier> parameters, BlockStatement? body, Environment? env) : IAstObject
{
    public List<Identifier> Parameters { get; private set; } = parameters;

    public BlockStatement? Body { get; init; } = body;

    public Environment? Env { get; init; } = env;

    public string Type() => AstTypes.FunctionObj;

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
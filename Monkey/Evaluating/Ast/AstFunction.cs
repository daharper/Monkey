using System.Text;
using Monkey.Parsing.Literals;
using Monkey.Parsing.Statements;

namespace Monkey.Evaluating.Ast;

public class AstFunction : IAstObject
{
    public List<Identifier> Parameters { get; init; } = [];
    
    public BlockStatement? Body { get; init; }
    
    public Environment? Env { get; init; }

    public string Type() => AstTypes.FunctionObj;

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
namespace Monkey.Parsing;

public class Programme : INode
{ 
    public List<IStatement> Statements { get; set; } = [];

    public string TokenLiteral()
        => Statements.Count > 0 ? Statements[0].TokenLiteral() : string.Empty;
}

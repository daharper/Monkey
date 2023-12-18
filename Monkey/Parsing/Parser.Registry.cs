using Monkey.Lexing;
using Monkey.Parsing.Nodes;

namespace Monkey.Parsing;

/// <summary>
/// Function and error registry for the parser.
/// </summary>
partial class Parser
{
    #region private
    
    private readonly List<string> _errors = [];
    private readonly Dictionary<string, Func<Node>> _prefixFunctions = new();
    private readonly Dictionary<string, Func<Node, Node>> _infixFunctions = new();

    #endregion
    
    public IEnumerable<string> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;
    
    #region private methods
    
    private void RegisterParserFunctions()
    {
        RegisterPrefix(Token.Identifier,
            () => new IdentifierNode(CurrentToken, CurrentToken.Literal));
    
        RegisterPrefix(Token.Int,
            () => new IntegerNode(CurrentToken, int.Parse(CurrentToken.Literal)));
    
        RegisterPrefix(Token.True, 
            () => new BooleanNode(CurrentToken, CurrentToken.Is(Token.True)));
    
        RegisterPrefix(Token.False, 
            () => new BooleanNode(CurrentToken, CurrentToken.Is(Token.True)));
        
        RegisterPrefix(Token.String, 
            () => new StringNode(CurrentToken, CurrentToken.Literal));
        
        RegisterPrefix(Token.Bang, ParsePrefixExpression);
        RegisterPrefix(Token.Minus, ParsePrefixExpression);
        RegisterPrefix(Token.LParen, ParseGroupedExpression);
        RegisterPrefix(Token.If, ParseIfExpression);
        RegisterPrefix(Token.Function, ParseFunctionExpression);
        RegisterPrefix(Token.LBracket, ParseArrayLiteral);
        RegisterPrefix(Token.LBrace, ParseHashLiteral);
        
        RegisterInfix(Token.Plus, ParseInfixExpression);
        RegisterInfix(Token.Minus, ParseInfixExpression);
        RegisterInfix(Token.Slash, ParseInfixExpression);
        RegisterInfix(Token.Asterisk, ParseInfixExpression);
        RegisterInfix(Token.Eq, ParseInfixExpression);
        RegisterInfix(Token.NotEq, ParseInfixExpression);
        RegisterInfix(Token.Lt, ParseInfixExpression);
        RegisterInfix(Token.Gt, ParseInfixExpression);
        RegisterInfix(Token.LParen, ParseCallExpression);
        RegisterInfix(Token.LBracket, ParseIndexExpression);
    }
    
    private void RegisterPrefix(string type, Func<Node> fn)
        => _prefixFunctions[type] = fn;
    
    private void RegisterInfix(string type, Func<Node, Node> fn)
        => _infixFunctions[type] = fn;
    
    #endregion
}
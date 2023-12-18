using System.Collections.Immutable;
using Monkey.Lexing;
using Monkey.Parsing.Nodes;

namespace Monkey.Parsing;

public class Parser
{
    private readonly List<string> _errors = new();

    private readonly Dictionary<string, Func<Node>> _prefixParseFns = new();
    
    private readonly Dictionary<string, Func<Node, Node>> _infixParseFns = new();

    private readonly ImmutableDictionary<string, Precedence> _precedences = new Dictionary<string, Precedence>
    {
        { Token.Eq, Precedence.Equals },
        { Token.NotEq, Precedence.Equals },
        { Token.Lt, Precedence.LessGreater },
        { Token.Gt, Precedence.LessGreater },
        { Token.Plus, Precedence.Sum },
        { Token.Minus, Precedence.Sum },
        { Token.Slash, Precedence.Product },
        { Token.Asterisk, Precedence.Product },
        { Token.LParen, Precedence.Call },
        { Token.LBracket, Precedence.Index }
    }.ToImmutableDictionary();
    
    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        RegisterParserFunctions();
        
        NextToken();
        NextToken();
    }

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

    private Node ParseHashLiteral()
    {
        var hash = new HashNode(CurrentToken);

        // is it an empty hash?
        if (PeekToken.Is(Token.RBrace))
        {
            NextToken();
            return hash;
        }
        
        while (!PeekToken.Is(Token.Eof)) 
        {
            // read key
            NextToken();
            var key = ParseExpression(Precedence.Lowest);
            
            if (!TryAdvanceTo(Token.Colon)) return null;
            
            // read value
            NextToken();
            var value = ParseExpression(Precedence.Lowest);
            
            hash.Pairs.Add(key, value);

            if (PeekToken.Is(Token.RBrace))
            {
                NextToken();
                return hash;
            }

            if (!PeekToken.Is(Token.Comma)) break;
            NextToken();
        }
        
        return null;
    }

    private Node ParseIndexExpression(Node left)
    {
        var expression = new IndexNode(CurrentToken, left);
        
        NextToken();
        
        expression.Index = ParseExpression(Precedence.Lowest);
        
        if (!TryAdvanceTo(Token.RBracket))
        {
            return null;
        }
        
        return expression;
    }

    private Node ParseArrayLiteral()
        => new ArrayNode(CurrentToken, ParseExpressionList(Token.RBracket));

    private List<Node> ParseExpressionList(string end)
    {
        var list = new List<Node>();
        
        if (PeekToken.Is(end))
        {
            NextToken();
            return list;
        }
        
        NextToken();
        list.Add(ParseExpression(Precedence.Lowest)!);
        
        while (PeekToken.Is(Token.Comma))
        {
            NextToken();
            NextToken();
            list.Add(ParseExpression(Precedence.Lowest)!);
        }

        return TryAdvanceTo(end) ? list : [];
    }

    private Node ParseCallExpression(Node function)
        => new CallNode(CurrentToken, function, ParseExpressionList(Token.RParen));

    private List<Node> ParseCallArguments()
    {
        var arguments = new List<Node>();
        
        if (PeekToken.Is(Token.RParen))
        {
            NextToken();
            return arguments;
        }
        
        NextToken();
        
        arguments.Add(ParseExpression(Precedence.Lowest));
        
        while (PeekToken.Is(Token.Comma))
        {
            NextToken();
            NextToken();
            
            arguments.Add(ParseExpression(Precedence.Lowest));
        }
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        return arguments;    
    }
    
    
    private Node ParseFunctionExpression()
    {
        var expression = new FunctionNode(CurrentToken);
        
        if (!TryAdvanceTo(Token.LParen))
        {
            return null;
        }
        
        expression.Parameters = ParseFunctionParameters();
        
        if (!TryAdvanceTo(Token.LBrace))
        {
            return null;
        }
        
        expression.Body = ParseBlockStatement();
        
        return expression;    
    }
    
    private List<IdentifierNode> ParseFunctionParameters()
    {
        var identifiers = new List<IdentifierNode>();
        
        if (PeekToken.Is(Token.RParen))
        {
            NextToken();
            return identifiers;
        }
        
        NextToken();
        
        var identifier = new IdentifierNode(token: CurrentToken, value: CurrentToken.Literal);
        
        identifiers.Add(identifier);
        
        while (PeekToken.Is(Token.Comma))
        {
            NextToken();
            NextToken();
            
            identifier = new IdentifierNode(token: CurrentToken, value: CurrentToken.Literal);
            
            identifiers.Add(identifier);
        }
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        return identifiers;
    }
    
    private Node ParsePrefixExpression()
    {
        var expression = new PrefixNode(CurrentToken, CurrentToken.Literal);
        
        NextToken();
        
        expression.Right = ParseExpression(Precedence.Prefix);
        
        return expression;
    }
    
    private Node ParseInfixExpression(Node left)
    {
        var expression = new InfixNode(CurrentToken, CurrentToken.Literal, left);

        var precedence = CurrentPrecedence();
        
        NextToken();
        
        expression.Right = ParseExpression(precedence);
        
        return expression;
    }
    
    private Node ParseGroupedExpression()
    {
        NextToken();
        
        var expression = ParseExpression(Precedence.Lowest);
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        return expression;
    }
    
    private Node ParseIfExpression()
    {
        var expression = new IfNode(CurrentToken);
        
        if (!TryAdvanceTo(Token.LParen))
        {
            return null;
        }
        
        NextToken();
        
        expression.Condition = ParseExpression(Precedence.Lowest);
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        if (!TryAdvanceTo(Token.LBrace))
        {
            return null;
        }
        
        expression.Consequence = ParseBlockStatement();
        
        if (PeekToken.Is(Token.Else))
        {
            NextToken();
            
            if (!TryAdvanceTo(Token.LBrace))
            {
                return null;
            }
            
            expression.Alternative = ParseBlockStatement();
        }
        
        return expression;
    }
    
    private BlockNode ParseBlockStatement()
    {
        var block = new BlockNode(CurrentToken);
        
        NextToken();
        
        while (!CurrentToken.Is(Token.RBrace) && !CurrentToken.Is(Token.Eof))
        {
            var statement = ParseStatement();
            
            if (statement != null)
            {
                block.Statements.Add(statement);
            }
            
            NextToken();
        }
        
        return block;
    }
    
    public IEnumerable<string> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;
    
    public Lexer Lexer { get; }
    
    public Token CurrentToken { get; set; }
    
    public Token PeekToken { get; set; }
    
    public void NextToken()
    {
        CurrentToken = PeekToken;
        PeekToken = Lexer.NextToken();
    }
    
    public Programme ParseProgramme()
    {
        var programme = Programme.Create();
        
        while (!CurrentToken.Is(Token.Eof))
        {
            var statement = ParseStatement();
            
            if (statement != null)
            {
                programme.Statements.Add(statement);
            }
            
            NextToken();
        }
        
        return programme;
    }

    private Node? ParseStatement()
    {
        return CurrentToken.Type switch
        {
            Token.Let => ParseLetStatement(),
            Token.Return => ParseReturnStatement(),
            _ => ParseExpressionStatement()
        };
    }

    private Node? ParseExpressionStatement()
    {
        var statement = new ExpressionNode(CurrentToken, ParseExpression(Precedence.Lowest));

        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }

        return statement;
    }

    private Node? ParseExpression(Precedence precedence)
    {
        if (!_prefixParseFns.TryGetValue(CurrentToken.Type, out var prefix))
        {
            _errors.Add($"no prefix parse function for {CurrentToken.Type} found");
            return null;
        }
        
        var leftExpression = prefix();
        
        while (!PeekToken.Is(Token.Semicolon) && precedence < PeekPrecedence())
        {
            if (!_infixParseFns.TryGetValue(PeekToken.Type, out var infix))
            {
                return leftExpression;
            }
            
            NextToken();
            
            leftExpression = infix(leftExpression);
        }
        
        return leftExpression;
    }

    private Node? ParseReturnStatement()
    {
        var statement = new ReturnNode(CurrentToken);
        
        NextToken();
        
        statement.ReturnValue = ParseExpression(Precedence.Lowest);
        
        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }
        
        return statement;
    }

    private LetNode? ParseLetStatement()
    {
        var statement = new LetNode(CurrentToken);
        
        if (!TryAdvanceTo(Token.Identifier))
        {
            return null;
        }
        
        statement.Name = new IdentifierNode(CurrentToken, CurrentToken.Literal);
        
        if (!TryAdvanceTo(Token.Assign))
        {
            return null;
        }
        
        NextToken();

        statement.Value = ParseExpression(Precedence.Lowest);
        
        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }
        
        return statement;
    }
    
    // this was called 'expectPeek' in the book, renamed to better reflect what it does
    private bool TryAdvanceTo(string type)
    {
        if (PeekToken.Is(type))
        {
            NextToken();
            return true;
        }

        _errors.Add($"expected next token to be {type}, got {PeekToken.Type} instead");
        
        return false;
    }
    
    private void RegisterPrefix(string type, Func<Node> fn)
        => _prefixParseFns[type] = fn;
    
    private void RegisterInfix(string type, Func<Node, Node> fn)
        => _infixParseFns[type] = fn;
 
    private Precedence PeekPrecedence()
        => CollectionExtensions.GetValueOrDefault(_precedences, PeekToken.Type, Precedence.Lowest);
    
    private Precedence CurrentPrecedence()
        => CollectionExtensions.GetValueOrDefault(_precedences, CurrentToken.Type, Precedence.Lowest);
}
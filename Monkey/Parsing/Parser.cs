using Monkey.Lexing;
using Monkey.Parsing.Nodes;
using Monkey.Utils;

namespace Monkey.Parsing;

public class Parser
{
    private readonly List<string> _errors = [];

    private readonly Dictionary<string, Func<Node>> _prefixParseFns = new();
    private readonly Dictionary<string, Func<Node, Node>> _infixParseFns = new();
    
    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        RegisterParserFunctions();
        
        NextToken();
        NextToken();
    }

    public IEnumerable<string> Errors => _errors;

    public bool HasErrors => _errors.Count > 0;

    private Lexer Lexer { get; }

    private Token CurrentToken { get; set; } = Token.Null;

    private Token PeekToken { get; set; } = Token.Null;
    
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
        
        if (PeekToken.Is(Token.RBrace))
        {
            NextToken();
            return hash;
        }
        
        while (!PeekToken.Is(Token.Eof)) 
        {
            NextToken();
            var key = ParseExpression(Precedence.Lowest);
            
            Consume(Token.Colon);

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
        
        throw Fatal.Error($"expected next token to be {Token.RBrace}, got {PeekToken.Type} instead");
    }

    private Node ParseIndexExpression(Node left)
    {
        var expression = new IndexNode(CurrentToken, left);
        
        NextToken();
        
        expression.Index = ParseExpression(Precedence.Lowest);

        Consume(Token.RBracket);
        
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
        list.Add(ParseExpression(Precedence.Lowest));
        
        while (PeekToken.Is(Token.Comma))
        {
            NextToken();
            NextToken();
            list.Add(ParseExpression(Precedence.Lowest));
        }

        Consume(end);

        return list;
    }

    private Node ParseCallExpression(Node function)
        => new CallNode(CurrentToken, function, ParseExpressionList(Token.RParen));
    
    private Node ParseFunctionExpression()
    {
        var expression = new FunctionNode(CurrentToken);
        
        Consume(Token.LParen);
        
        expression.Parameters = ParseFunctionParameters();
        
        Consume(Token.LBrace);
        
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
        
        Consume(Token.RParen);
        
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

        var precedence = Precedences.Resolve(CurrentToken.Type);
        
        NextToken();
        
        expression.Right = ParseExpression(precedence);
        
        return expression;
    }
    
    private Node ParseGroupedExpression()
    {
        NextToken();
        
        var expression = ParseExpression(Precedence.Lowest);
        
        Consume(Token.RParen);
        
        return expression;
    }
    
    private Node ParseIfExpression()
    {
        var expression = new IfNode(CurrentToken);

        Consume(Token.LParen);
        
        NextToken();
        
        expression.Condition = ParseExpression(Precedence.Lowest);
        
        Consume(Token.RParen);
        Consume(Token.LBrace);
        
        expression.Consequence = ParseBlockStatement();

        if (!PeekToken.Is(Token.Else)) return expression;
        
        NextToken();
            
        Consume(Token.LBrace);
            
        expression.Alternative = ParseBlockStatement();

        return expression;
    }
    
    private BlockNode ParseBlockStatement()
    {
        var block = new BlockNode(CurrentToken);
        
        NextToken();
        
        while (!CurrentToken.Is(Token.RBrace) && !CurrentToken.Is(Token.Eof))
        {
            var statement = ParseStatement();
            
            // should we exit if null?
            if (statement != Node.Null)
            {
                block.Statements.Add(statement);
            }
            
            NextToken();
        }
        
        return block;
    }
    
    public ProgramNode ParseProgramme()
    {
        var programme = ProgramNode.Create();
        
        while (!CurrentToken.Is(Token.Eof))
        {
            var statement = ParseStatement();
            
            // should we exit if null?
            if (statement != Node.Null)
            {
                programme.Statements.Add(statement);
            }
            
            NextToken();
        }
        
        return programme;
    }

    private Node ParseStatement()
    {
        return CurrentToken.Type switch
        {
            Token.Let => ParseLetStatement(),
            Token.Return => ParseReturnStatement(),
            _ => ParseExpressionStatement()
        };
    }

    private Node ParseExpressionStatement()
    {
        var statement = new ExpressionNode(CurrentToken, ParseExpression(Precedence.Lowest));

        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }

        return statement;
    }

    private Node ParseExpression(Precedence precedence)
    {
        if (!_prefixParseFns.TryGetValue(CurrentToken.Type, out var prefix))
        {
            _errors.Add($"no prefix parse function for {CurrentToken.Type} found");
            return Node.Null;
        }
        
        var leftExpression = prefix();
        
        while (!PeekToken.Is(Token.Semicolon) && precedence < Precedences.Resolve(PeekToken.Type))
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

    private Node ParseReturnStatement()
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

    private Node ParseLetStatement()
    {
        var statement = new LetNode(CurrentToken);
        
        Consume(Token.Identifier);
        
        statement.Name = new IdentifierNode(CurrentToken, CurrentToken.Literal);

        Consume(Token.Assign);
        
        NextToken();

        statement.Value = ParseExpression(Precedence.Lowest);
        
        // todo: shouldn't this be an error if it is not a semicolon?
        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }
        
        return statement;
    }
    
    private void NextToken()
    {
        CurrentToken = PeekToken;
        PeekToken = Lexer.NextToken();
    }

    private void Consume(string type)
    {
        if (PeekToken.Is(type))
        {
            NextToken();
            return;
        }
        
        throw Fatal.Error($"expected next token to be {type}, got {PeekToken.Type} instead");
    }
    
    private void RegisterPrefix(string type, Func<Node> fn)
        => _prefixParseFns[type] = fn;
    
    private void RegisterInfix(string type, Func<Node, Node> fn)
        => _infixParseFns[type] = fn;
}
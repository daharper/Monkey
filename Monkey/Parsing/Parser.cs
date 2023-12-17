using System.Collections.Immutable;
using Monkey.Lexing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Interfaces;
using Monkey.Parsing.Literals;
using Monkey.Parsing.Statements;

using TokenType = string;

namespace Monkey.Parsing;

public class Parser
{
    private readonly List<string> _errors = new();

    private readonly Dictionary<TokenType, Func<IExpression>> _prefixParseFns = new();
    
    private readonly Dictionary<TokenType, Func<IExpression, IExpression>> _infixParseFns = new();

    private readonly ImmutableDictionary<TokenType, Precedence> _precedences = new Dictionary<TokenType, Precedence>
    {
        { Token.Eq, Precedence.Equals },
        { Token.NotEq, Precedence.Equals },
        { Token.Lt, Precedence.LessGreater },
        { Token.Gt, Precedence.LessGreater },
        { Token.Plus, Precedence.Sum },
        { Token.Minus, Precedence.Sum },
        { Token.Slash, Precedence.Product },
        { Token.Asterisk, Precedence.Product },
        { Token.LParen, Precedence.Call }
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
            () => new Identifier { Token = CurrentToken, Value = CurrentToken.Literal });

        RegisterPrefix(Token.Int,
            () => new IntegerLiteral { Token = CurrentToken, Value = int.Parse(CurrentToken.Literal) });

        RegisterPrefix(Token.True, 
            () => new BooleanLiteral { Token = CurrentToken, Value = CurrentToken.Is(Token.True) });

        RegisterPrefix(Token.False, 
            () => new BooleanLiteral { Token = CurrentToken, Value = CurrentToken.Is(Token.True) });
        
        RegisterPrefix(Token.String, 
            () => new StringLiteral { Token = CurrentToken, Value = CurrentToken.Literal });
        
        RegisterPrefix(Token.Bang, ParsePrefixExpression);
        RegisterPrefix(Token.Minus, ParsePrefixExpression);
        RegisterPrefix(Token.LParen, ParseGroupedExpression);
        RegisterPrefix(Token.If, ParseIfExpression);
        RegisterPrefix(Token.Function, ParseFunctionExpression);
        
        RegisterInfix(Token.Plus, ParseInfixExpression);
        RegisterInfix(Token.Minus, ParseInfixExpression);
        RegisterInfix(Token.Slash, ParseInfixExpression);
        RegisterInfix(Token.Asterisk, ParseInfixExpression);
        RegisterInfix(Token.Eq, ParseInfixExpression);
        RegisterInfix(Token.NotEq, ParseInfixExpression);
        RegisterInfix(Token.Lt, ParseInfixExpression);
        RegisterInfix(Token.Gt, ParseInfixExpression);
        RegisterInfix(Token.LParen, ParseCallExpression);
    }

    private IExpression ParseCallExpression(IExpression function)
    {
        var expression = new CallExpression { Token = CurrentToken, Function = function };
        
        expression.Arguments = ParseCallArguments();
        
        return expression;
    }

    private List<IExpression> ParseCallArguments()
    {
        var arguments = new List<IExpression>();
        
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
    
    
    private IExpression ParseFunctionExpression()
    {
        var expression = new FunctionLiteral { Token = CurrentToken };
        
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
    
    private List<Identifier> ParseFunctionParameters()
    {
        var identifiers = new List<Identifier>();
        
        if (PeekToken.Is(Token.RParen))
        {
            NextToken();
            return identifiers;
        }
        
        NextToken();
        
        var identifier = new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };
        
        identifiers.Add(identifier);
        
        while (PeekToken.Is(Token.Comma))
        {
            NextToken();
            NextToken();
            
            identifier = new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };
            
            identifiers.Add(identifier);
        }
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        return identifiers;
    }
    
    private IExpression ParsePrefixExpression()
    {
        var expression = new PrefixExpression
        {
            Token = CurrentToken,
            Operator = CurrentToken.Literal
        };
        
        NextToken();
        
        expression.Right = ParseExpression(Precedence.Prefix);
        
        return expression;
    }
    
    private IExpression ParseInfixExpression(IExpression left)
    {
        var expression = new InfixExpression
        {
            Token = CurrentToken,
            Operator = CurrentToken.Literal,
            Left = left
        };

        var precedence = CurrentPrecedence();
        
        NextToken();
        
        expression.Right = ParseExpression(precedence);
        
        return expression;
    }
    
    private IExpression ParseGroupedExpression()
    {
        NextToken();
        
        var expression = ParseExpression(Precedence.Lowest);
        
        if (!TryAdvanceTo(Token.RParen))
        {
            return null;
        }
        
        return expression;
    }
    
    private IExpression ParseIfExpression()
    {
        var expression = new IfExpression { Token = CurrentToken };
        
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
    
    private BlockStatement ParseBlockStatement()
    {
        var block = new BlockStatement { Token = CurrentToken };
        
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
        var programme = new Programme();
        
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

    private IStatement? ParseStatement()
    {
        return CurrentToken.Type switch
        {
            Token.Let => ParseLetStatement(),
            Token.Return => ParseReturnStatement(),
            _ => ParseExpressionStatement()
        };
    }

    private IStatement? ParseExpressionStatement()
    {   
        var statement = new ExpressionStatement
        {
            Token = CurrentToken,
            Expression = ParseExpression(Precedence.Lowest)
        };

        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }

        return statement;
    }

    private IExpression? ParseExpression(Precedence precedence)
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

    private IStatement? ParseReturnStatement()
    {
        var statement = new ReturnStatement { Token = CurrentToken };
        
        NextToken();
        
        statement.ReturnValue = ParseExpression(Precedence.Lowest);
        
        if (PeekToken.Is(Token.Semicolon))
        {
            NextToken();
        }
        
        return statement;
    }

    private LetStatement? ParseLetStatement()
    {
        var statement = new LetStatement { Token = CurrentToken };
        
        if (!TryAdvanceTo(Token.Identifier))
        {
            return null;
        }
        
        statement.Name = new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };
        
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
    private bool TryAdvanceTo(TokenType type)
    {
        if (PeekToken.Is(type))
        {
            NextToken();
            return true;
        }

        _errors.Add($"expected next token to be {type}, got {PeekToken.Type} instead");
        
        return false;
    }
    
    private void RegisterPrefix(TokenType type, Func<IExpression> fn)
        => _prefixParseFns[type] = fn;
    
    private void RegisterInfix(TokenType type, Func<IExpression, IExpression> fn)
        => _infixParseFns[type] = fn;
 
    private Precedence PeekPrecedence()
        => CollectionExtensions.GetValueOrDefault(_precedences, PeekToken.Type, Precedence.Lowest);
    
    private Precedence CurrentPrecedence()
        => CollectionExtensions.GetValueOrDefault(_precedences, CurrentToken.Type, Precedence.Lowest);
}
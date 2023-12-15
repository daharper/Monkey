using Monkey.Lexing;
using Monkey.Parsing.Expressions;
using Monkey.Parsing.Interfaces;
using Monkey.Parsing.Statements;

using TokenType = string;

namespace Monkey.Parsing;

public class Parser
{
    private readonly List<string> _errors = new();

    private readonly Dictionary<TokenType, Func<IExpression>> _prefixParseFns = new();
    
    private readonly Dictionary<TokenType, Func<IExpression, IExpression>> _infixParseFns = new();
    
    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        RegisterParserFunctions();
        
        NextToken();
        NextToken();
    }

    private void RegisterParserFunctions()
    {
        _prefixParseFns.Add(
            Token.Identifier, 
            () => new IdentifierExpression { Token = CurrentToken, Value = CurrentToken.Literal });
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
        
        // while (!PeekToken.Is(Token.Semicolon)) && precedence < PeekPrecedence())
        // {
        //     var infix = _infixParseFns[PeekToken.Type];
        //     
        //     if (infix == null)
        //     {
        //         return leftExpression;
        //     }
        //     
        //     NextToken();
        //     
        //     leftExpression = infix(leftExpression);
        // }
        
        return leftExpression;
    }

    private IStatement? ParseReturnStatement()
    {
        var statement = new ReturnStatement { Token = CurrentToken };
        
        NextToken();
        
        while(!CurrentToken.Is(Token.Semicolon))
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
        
        statement.Name = new IdentifierExpression { Token = CurrentToken, Value = CurrentToken.Literal };
        
        if (!TryAdvanceTo(Token.Assign))
        {
            return null;
        }
        
        while(!CurrentToken.Is(Token.Semicolon))
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
    
    public void RegisterPrefix(TokenType type, Func<IExpression> fn)
        => _prefixParseFns[type] = fn;
    
    public void RegisterInfix(TokenType type, Func<IExpression, IExpression> fn)
        => _infixParseFns[type] = fn;
}
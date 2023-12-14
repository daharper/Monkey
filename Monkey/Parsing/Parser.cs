using Monkey.Lexing;

using TokenType = string;

namespace Monkey.Parsing;

public class Parser
{
    private readonly List<string> _errors = new();
    
    public Parser(Lexer lexer)
    {
        Lexer = lexer; 
        
        NextToken();
        NextToken();
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
            _ => null
        };
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
        
        statement.Name = new Identifier { Token = CurrentToken, Value = CurrentToken.Literal };
        
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
}
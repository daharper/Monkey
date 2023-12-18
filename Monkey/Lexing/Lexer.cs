using Monkey.Utils;

namespace Monkey.Lexing;

/// <summary>
/// Class for tokenizing input strings.
/// </summary>
public class Lexer
{
    /// <summary>
    /// Creates a new instance of the Lexer class with the specified input.
    /// </summary>
    /// <param name="input">The input string to be processed by the Lexer.</param>
    /// <returns>A new instance of the Lexer class initialized with the specified input.</returns>
    public Lexer(string input)
    {
        Input = input;
        ReadChar();
    }

    // The input string to be tokenized
    private string Input { get; set; } 
    
    // Points to the current character under examination
    private int Position { get; set; }
    
    // Points to the next character after the current one
    private int ReadPosition { get; set; }
    
    // The current character under examination
    private char Ch { get; set; }

    public Token NextToken()
    {
        Token tok;
        
        SkipWhitespace();
        
        switch (Ch)
        {
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = Ch;
                    ReadChar();
                    var literal = $"{ch}{Ch}";
                    tok = new Token(Token.Eq, literal);
                }
                else
                {
                    tok = new Token(Token.Assign, Ch);
                }
                break;
            case '+':
                tok = new Token(Token.Plus, Ch);
                break;
            case '-':
                tok = new Token(Token.Minus, Ch);
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = Ch;
                    ReadChar();
                    var literal = $"{ch}{Ch}";
                    tok = new Token(Token.NotEq, literal);
                }
                else
                {
                    tok = new Token(Token.Bang, Ch);
                }
                break;
            case '/':
                tok = new Token(Token.Slash, Ch);
                break;
            case '*':
                tok = new Token(Token.Asterisk, Ch);
                break;
            case '<':
                tok = new Token(Token.Lt, Ch);
                break;
            case '>':
                tok = new Token(Token.Gt, Ch);
                break;
            case ';':
                tok = new Token(Token.Semicolon, Ch);
                break;
            case ',':
                tok = new Token(Token.Comma, Ch);
                break;
            case ':':
                tok = new Token(Token.Colon, Ch);
                break;
            case '(':
                tok = new Token(Token.LParen, Ch);
                break;
            case ')':
                tok = new Token(Token.RParen, Ch);
                break;
            case '{':
                tok = new Token(Token.LBrace, Ch);
                break;
            case '}':
                tok = new Token(Token.RBrace, Ch);
                break;
            case '[':
                tok = new Token(Token.LBracket, Ch);
                break;
            case ']':
                tok = new Token(Token.RBracket, Ch);
                break;
            case '"':
                ReadChar();
                var str = ReadSubstring(ch => ch != '"');
                tok = new Token(Token.String, str);
                if (PeekChar() == '"')
                {
                    ReadChar();    
                }
                break;
            case '\0':
                tok = new Token(Token.Eof);
                break;
            default:
                if (char.IsLetter(Ch))
                {
                    var identifier = ReadLiteral();
                    var type = LookupIdent(identifier);
                    
                    tok = new Token(type, identifier);
                }
                else if (char.IsDigit(Ch))
                {
                    var value = ReadNumber();
                    tok = new Token(Token.Int, value);
                }
                else
                {
                    tok = new Token(Token.Illegal, Ch);
                }
                break;
        }

        ReadChar();
        return tok;
    }
    
    #region private methods
    
    private char PeekChar()
        => ReadPosition >= Input.Length ? '\0' : Input[ReadPosition];
    
    private void ReadChar()
    {
        Ch = ReadPosition >= Input.Length ? '\0' : Input[ReadPosition];
        
        Position = ReadPosition;
        ++ReadPosition;
    }
   
    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(Ch))
            ReadChar();
    }

    private string ReadSubstring(Func<char, bool> predicate)
    {
        var (index, value) = Input.TakeFrom(Position, predicate);

        if (index > Position)
        {
            Position = index - 1;
            ReadPosition = index;
        }
        
        return value;
    }

    private string ReadNumber()
        => ReadSubstring(char.IsDigit);
    
    private string ReadLiteral()
        => ReadSubstring(ch => char.IsLetter(ch) || ch == '_');
    
    private static string LookupIdent(string ident)
        => Token.Keywords.GetValueOrDefault(ident, Token.Identifier);
    
    #endregion
}
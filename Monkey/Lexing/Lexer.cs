using Monkey.Utils;

namespace Monkey.Lexing;

public class Lexer
{
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
        Token token;
        
        SkipWhitespaceAndComments();
        
        switch (Ch)
        {
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = Ch;
                    ReadChar();
                    var literal = $"{ch}{Ch}";
                    token = new Token(Token.Eq, literal);
                }
                else
                {
                    token = new Token(Token.Assign, Ch);
                }
                break;
            case '+':
                token = new Token(Token.Plus, Ch);
                break;
            case '-':
                token = new Token(Token.Minus, Ch);
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = Ch;
                    ReadChar();
                    var literal = $"{ch}{Ch}";
                    token = new Token(Token.NotEq, literal);
                }
                else
                {
                    token = new Token(Token.Bang, Ch);
                }
                break;
            case '/':
                token = new Token(Token.Slash, Ch);
                break;
            case '*':
                token = new Token(Token.Asterisk, Ch);
                break;
            case '<':
                token = new Token(Token.Lt, Ch);
                break;
            case '>':
                token = new Token(Token.Gt, Ch);
                break;
            case ';':
                token = new Token(Token.Semicolon, Ch);
                break;
            case ',':
                token = new Token(Token.Comma, Ch);
                break;
            case ':':
                token = new Token(Token.Colon, Ch);
                break;
            case '(':
                token = new Token(Token.LParen, Ch);
                break;
            case ')':
                token = new Token(Token.RParen, Ch);
                break;
            case '{':
                token = new Token(Token.LBrace, Ch);
                break;
            case '}':
                token = new Token(Token.RBrace, Ch);
                break;
            case '[':
                token = new Token(Token.LBracket, Ch);
                break;
            case ']':
                token = new Token(Token.RBracket, Ch);
                break;
            case '"':
                token = new Token(Token.String, ReadString());
                break;
            case '\0':
                token = new Token(Token.Eof);
                break;
            default:
                if (char.IsLetter(Ch))
                {
                    var identifier = ReadLiteral();
                    var type = LookupIdent(identifier);
                    
                    token = new Token(type, identifier);
                }
                else if (char.IsDigit(Ch))
                {
                    var value = ReadNumber();
                    token = new Token(Token.Int, value);
                }
                else
                {
                    token = new Token(Token.Illegal, Ch);
                }
                break;
        }

        ReadChar();
        return token;
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
   
    // Comments run from '#' to the end of the line
    private void SkipWhitespaceAndComments()
    {
        while (true)
        {
            while (char.IsWhiteSpace(Ch))
            {
                ReadChar();
            }

            if (Ch != '#') return;

            while (Ch != '\n' && Ch != '\0')
            {
                ReadChar();
            }
        }
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

    // Reads up to the closing quote, or the end of input if the string is unterminated
    private string ReadString()
    {
        var start = Position + 1;

        do
        {
            ReadChar();
        } while (Ch != '"' && Ch != '\0');

        return Input[start..Math.Min(Position, Input.Length)];
    }

    private string ReadNumber()
        => ReadSubstring(char.IsDigit);
    
    private string ReadLiteral()
        => ReadSubstring(ch => char.IsLetter(ch) || ch == '_');
    
    private static string LookupIdent(string ident)
        => Token.Keywords.GetValueOrDefault(ident, Token.Identifier);
    
    #endregion
}
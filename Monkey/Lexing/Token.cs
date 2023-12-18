using System.Collections.Immutable;

namespace Monkey.Lexing;

public class Token(string type, string literal = "")
{
    public static readonly Token Null = new("", "");
    public static readonly Token Root = new("ROOT", "");
    
    public const string Illegal = "ILLEGAL";
    public const string Eof = "EOF";
 
    // Identifiers + literals
    public const string Identifier = "IDENT";
    public const string Int = "INT";
    
    // Operators
    public const string Assign = "=";
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Bang = "!";
    public const string Asterisk = "*";
    public const string Slash = "/";
    
    public const string Lt = "<";
    public const string Gt = ">";
    public const string Eq = "==";
    public const string NotEq = "!=";
    
    // Delimiters
    public const string Comma = ",";
    public const string Semicolon = ";";
    public const string Colon = ":";
   
    public const string LParen = "(";
    public const string RParen = ")";
    public const string LBrace = "{";
    public const string RBrace = "}";
    public const string LBracket = "[";
    public const string RBracket = "]";
    
    // Keywords
    public const string Function = "FUNCTION";
    public const string Let = "LET";
    public const string True = "TRUE";
    public const string False = "FALSE";
    public const string If = "IF";
    public const string Else = "ELSE";
    public const string Return = "RETURN";
    public const string String = "STRING";
    
    public static readonly ImmutableDictionary<string, string> Keywords 
        = new Dictionary<string, string>
    {
        {"fn", Function},
        {"let", Let},
        {"true", True},
        {"false", False},
        {"if", If},
        {"else", Else},
        {"return", Return}
    }.ToImmutableDictionary();

    public Token(string type, char ch) : this(type, ch.ToString()) { }
    
    public string Type { get; set; } = type;
    
    public string Literal { get; set; } = literal;

    public bool Is(string tokenType) => Type == tokenType;
    
    public bool Is(Token token) => Type == token.Type;
    
    public override string ToString() => $"Token({Type}, {Literal})";
}
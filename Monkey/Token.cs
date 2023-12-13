// ReSharper disable UseSymbolAlias

using System.Collections.Immutable;

namespace Monkey;

using TokenType = string;

public class Token(TokenType type, string literal = "")
{
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
   
    public const string LParen = "(";
    public const string RParen = ")";
    public const string LBrace = "{";
    public const string RBrace = "}";
    
    // Keywords
    public const string Function = "FUNCTION";
    public const string Let = "LET";
    public const string True = "TRUE";
    public const string False = "FALSE";
    public const string If = "IF";
    public const string Else = "ELSE";
    public const string Return = "RETURN";
    
    public static readonly ImmutableDictionary<string, string> Keywords = new Dictionary<string, string>
    {
        {"fn", Function},
        {"let", Let},
        {"true", True},
        {"false", False},
        {"if", If},
        {"else", Else},
        {"return", Return}
    }.ToImmutableDictionary();

    public Token(TokenType type, char ch) : this(type, ch.ToString()) { }
    
    public TokenType Type { get; set; } = type;
    
    public string Literal { get; set; } = literal;

    public override string ToString()
        => $"Token({Type}, {Literal})";
}
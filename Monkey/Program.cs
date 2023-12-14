using Monkey.Lexing;
using static System.Console;

const string prompt = ">> ";

WriteLine("Hello! This is the Monkey programming language!");
WriteLine("Feel free to type in commands, or type 'exit' to exit.");

while (true)
{
    Write(prompt);
    var input = ReadLine();

    if (string.IsNullOrWhiteSpace(input)) continue;
    if (input == "exit") break;

    var lexer = new Lexer(input);

    for (var token = lexer.NextToken(); token.Type != Token.Eof; token = lexer.NextToken())
        WriteLine(token);
}
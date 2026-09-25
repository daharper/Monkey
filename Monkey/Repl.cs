using Monkey.Evaluating;
using Monkey.Evaluating.Objects;
using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Parsing.Nodes;
using Monkey.Utils;

using static System.Console;

namespace Monkey;

public static class Repl
{
    private const string MonkeyFace =
            """
            
               .--.  .-"     "-.  .--.
              / .. \/  .-. .-.  \/ .. \
             | |  '|  /   Y   \  |'  | |
             | \   \  \ 0 | 0 /  /   / |
              \ '- ,\.-~~~~~~~-./, -' /
                ''-'/_   ^ ^   _\ '-''
                   |  \._   _./  |
                   \   \ '~' /   /
                    '._ '-=-' _.'
                       '-----'
                       
            """;
    public static void Run()
    {
        const string prompt = ">> ";
     
        DisplayWelcomeMessage();

        var context = new Context();
        
        while (true)
        {
            Write(prompt);
            var input = ReadLine();

            if (input is null) break;
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.TrimStart().StartsWith('#')) continue;
            if (input == "exit") break;

            try
            {
                Execute(input, context);
            }
            catch (Exception e)
            {
                DisplayException(e);
            }
            
            WriteLine();
        }
        
        DisplayGoodbyeMessage();
    }

    private static void Execute(string input, Context context)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        
        ProgramNode program;

        try
        {
            program = parser.ParseProgram();
        }
        catch (ParserException e)
        {
            DisplayParserErrors(parser.Errors.Append(e.Message));
            return;
        }

        if (parser.HasErrors)
        {
            DisplayParserErrors(parser.Errors);
            return;
        }
                
        var evaluated = Evaluator.Eval(program, context);
                
        if (evaluated is not NullObject)
        {
            WriteLine(evaluated);
        }
    }
    
    private static void DisplayWelcomeMessage()
    {
        WriteLine(MonkeyFace);
        WriteLine("Hello! This is the Monkey programming language!");
        WriteLine("Feel free to type in commands" + Environment.NewLine);
    }
    
    private static void DisplayGoodbyeMessage()
    {
        WriteLine("Goodbye!");
    }
    
    private static void DisplayParserErrors(IEnumerable<string> errors)
    {
        WriteLine("Whoops! We ran into some monkey business here!");
        WriteLine();
        WriteLine("parser errors:");
        errors.ForEach((i, e) => WriteLine($"\t{i+1}. {e}"));
    }
    
    private static void DisplayException(Exception exception)
    {
        WriteLine("Whoops! We ran into some monkey business here!");
        WriteLine();
        WriteLine($"{exception.Message}");
    }
}
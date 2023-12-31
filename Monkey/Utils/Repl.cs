using Monkey.Evaluating;
using Monkey.Evaluating.Objects;
using Monkey.Lexing;
using Monkey.Parsing;
using static System.Console;

namespace Monkey.Utils;

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

            if (string.IsNullOrWhiteSpace(input)) continue;
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
        var program = parser.ParseProgram();

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
        WriteLine("Hello! This is the Monkey programming language!");
        WriteLine("Feel free to type in commands");
    }
    
    private static void DisplayGoodbyeMessage()
    {
        WriteLine("Goodbye!");
    }
    
    private static void DisplayParserErrors(IEnumerable<string> errors)
    {
        WriteLine(MonkeyFace);
        WriteLine("Whoops! We ran into some monkey business here!");
        WriteLine();
        WriteLine("parser errors:");
        errors.ForEach((i, e) => WriteLine($"\t{i+1}. {e}"));
    }
    
    private static void DisplayException(Exception exception)
    {
        WriteLine(MonkeyFace);
        WriteLine("Whoops! We ran into some monkey business here!");
        WriteLine();
        WriteLine($"{exception.Message}");
    }
}
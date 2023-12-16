using Monkey.Evaluating;
using Monkey.Lexing;
using Monkey.Parsing;
using Monkey.Utils;
using Environment = Monkey.Evaluating.Environment;

namespace Monkey;

using static System.Console;

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
    public static void Execute()
    {
        const string prompt = ">> ";
     
        WriteLine("Hello! This is the Monkey programming language!");
        WriteLine("Feel free to type in commands, or type 'exit' to exit.");

        var environment = new Environment();
        
        while (true)
        {
            Write(prompt);
            var input = ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input == "exit") break;

            var lexer = new Lexer(input);
            var parser = new Parser(lexer);
            var programme = parser.ParseProgramme();
            
            if (parser.HasErrors)
            {
                WriteLine();
                WriteLine(MonkeyFace);
                WriteLine("Woops! We ran into some monkey business here!");
                WriteLine();
                WriteLine("parser errors:");
                parser.Errors.ForEach((i, e) => WriteLine($"\t{i+1}. {e}"));
                WriteLine();
                continue;
            }
            
            var evaluated = Evaluator.Eval(programme, environment);
            
            if (evaluated is not null)
            {
                WriteLine(evaluated.Inspect());
            }
            
            WriteLine();
        }
    }
}
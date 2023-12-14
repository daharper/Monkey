using Monkey.Lexing;

namespace Monkey.Tests.Fixtures.Lexing;

public record LexerFixture(string Input, IList<Token> Tokens);

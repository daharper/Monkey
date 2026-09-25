# Monkey

C# implementation of the Monkey language from "Writing an Interpreter in Go"

I found coding a C# implementation of the Go Interpreter helped in understanding the demonstrated concepts.

I stayed close-ish to the Go structure so I could follow along in the book.

[Please see here for more details:](https://delphitnt.com/post/97)

### Technology

- C# 15 (preview language features)
- .NET 11 (preview SDK required until .NET 11 is released)
- NUnit 5
- Rider, or any editor with .NET 11 support

### Getting started

Start the REPL:

```
dotnet run --project Monkey
```

Run the tests:

```
dotnet test
```

The REPL reads one line at a time, and anything defined with `let` is kept for the session. Type `exit` to quit.

### Examples

```
>> 5 + 5 * 2
15
>> let person = {"name": "Monkey", "age": 7}; person["name"]
Monkey
>> let fib = fn(n) { if (n < 2) { n } else { fib(n - 1) + fib(n - 2) } }; fib(15)
610
>> let newAdder = fn(x) { fn(y) { x + y } }; let addTwo = newAdder(2); addTwo(40)
42
>> let map = fn(arr, f) { let iter = fn(a, acc) { if (len(a) == 0) { acc } else { iter(rest(a), push(acc, f(first(a)))) } }; iter(arr, []) }; map([1, 2, 3, 4], fn(x) { x * x })
[1, 4, 9, 16]
```

Built-in functions: `len`, `first`, `last`, `rest`, `push` and `puts`.

### License

[MIT](LICENSE)

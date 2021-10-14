namespace ParserLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ParserExtensions
    {
        public static Parser<IEnumerable<T>> OneOrMore<T>(this Parser<T> lhs)
        {
            return input =>
            {
                var results = new List<T>();
                var beginning = lhs(input);
                var currentInput = input;
                if (beginning.WasSuccessful)
                {
                    currentInput = beginning.Remainder;
                    results.Add(beginning.Value);
                    var rest = ZeroOrMore(lhs)(currentInput);
                    if (rest.WasSuccessful)
                    {
                        results.AddRange(rest.Value);
                        currentInput = rest.Remainder;
                    }
                    return Result.Success(results, currentInput);
                }
                else
                {
                    return Result.Fail<IEnumerable<T>>(
                        input, $"Expected 1 or more {typeof(T).Name}", Enumerable.Empty<string>());
                }
            };
        }

        public static Parser<IEnumerable<T>> ZeroOrMore<T>(this Parser<T> lhs)
        {
            return input =>
            {
                var currentInput = input;
                var results = new List<T>();
                var isComplete = false;
                do
                {
                    if (currentInput.IsEof())
                    {
                        isComplete = true;
                    }
                    else
                    {
                        var result = lhs(currentInput);
                        if (result.WasSuccessful)
                        {
                            results.Add(result.Value);
                            currentInput = result.Remainder;
                        }
                        else
                        {
                            isComplete = true;
                        }
                    }
                } while (!isComplete);
                return Result.Success(results, currentInput);
            };
        }

        public static Parser<U> Then<T, U>(this Parser<T> lhs, Func<T, Parser<U>> func, string msg)
        {
            return input =>
            {
                var results = lhs(input);
                if (results.WasSuccessful)
                {
                    return func(results.Value)(results.Remainder);
                }
                return Result.Fail<U>(input, msg, Enumerable.Empty<string>());
            };
        }
    }
}
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

        public static Parser<U> Then<T, U>(this Parser<T> lhs, Func<T, Parser<U>> func)
        {
            return input => lhs(input)
                .IfSuccess(lhsResult =>
                    func(lhsResult.Value)
                    (lhsResult.Remainder));
        }

        public static Parser<T> Or<T>(this Parser<T> lhs, Parser<T> rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }

            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            return i =>
            {
                var lhsResult = lhs(i);
                if (!lhsResult.WasSuccessful)
                {
                    return rhs(i).IfFailure(rhsResult =>
                        DetermineBestError(lhsResult, rhsResult));
                }
                return lhsResult;
            };
        }

        private static IResult<T> DetermineBestError<T>(IResult<T> lhs, IResult<T> rhs)
        {
            if (rhs.Remainder.Position > lhs.Remainder.Position)
            {
                return rhs;
            }
            if (lhs.Remainder.Position > rhs.Remainder.Position)
            {
                return lhs;
            }
            return Result.Fail<T>(lhs.Remainder, lhs.Message,
                lhs.Expectations.Union(rhs.Expectations));
        }

        public static Parser<U> Select<T, U>(this Parser<T> lhs, Func<T, U> convert)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException(nameof(lhs));
            }
            if (convert == null)
            {
                throw new ArgumentNullException(nameof(convert));
            }
            return lhs.Then(v => Parse.Return(convert(v)));
        }

        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }
    }
}
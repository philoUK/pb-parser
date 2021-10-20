namespace ParserLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Parse
    {
        private static Parser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
            return input =>
            {
                if (input.IsEof())
                {
                    return Result.Fail<char>(input,
                    "Unexpected end of input",
                    new[] { description });
                }
                if (predicate(input.CurrentValue().Value))
                {
                    return Result.Success(input.CurrentValue().Value, input.Advance());
                }
                return Result.Fail<char>(input,
                $"Unexpected {input.CurrentValue().Value}", new[] { description });
            };
        }

        public static Parser<char> Char(char ch)
        {
            return Char(c => c == ch, ch.ToString());
        }

        public static Parser<char> Digit()
        {
            return Char(char.IsDigit, "[0-9]");
        }

        public static Parser<char> Letter()
        {
            return Char(char.IsLetter, "[A-Za-z]");
        }

        public static Parser<char> LetterOrDigit()
        {
            return Char(c => char.IsLetterOrDigit(c), "Letter or digit");
        }

        public static Parser<char> Whitespace()
        {
            return Char(c => char.IsWhiteSpace(c), "Whitespace");
        }

        public static Parser<char> AnyChar()
        {
            return Char(c => true, "Any char");
        }

        public static Parser<char> AnyCaseChar(char ch)
        {
            return Char(c => char.ToLowerInvariant(c) == char.ToLowerInvariant(ch), $"{char.ToUpperInvariant(ch)} or {char.ToLowerInvariant(ch)}");
        }

        public static Parser<char> AnyCharExcept(char ch)
        {
            return Char(c => c != ch, $"Char should not = {ch}");
        }

        public static Parser<string> Text(string s)
        {
            return i =>
            {
                var currentInput = i;
                foreach (var ch in s)
                {
                    var results = Char(ch)(currentInput);
                    if (results.WasSuccessful)
                    {
                        currentInput = results.Remainder;
                    }
                    else
                    {
                        return Result.Fail<string>(i,
                        $"Expected string of {s}",
                        Enumerable.Empty<string>());
                    }
                }
                return Result.Success(s, currentInput);
            };
        }
        public static Parser<string> String(this Parser<IEnumerable<char>> lhs)
        {
            return input =>
            {
                var results = lhs(input);
                if (results.WasSuccessful)
                {
                    return Result.Success(
                        string.Concat(results.Value), results.Remainder
                    );
                }
                else
                {
                    return Result.Fail<string>(input,
                    "Expected sequence of characters",
                    Enumerable.Empty<string>());
                }
            };
        }

        public static Parser<long> Numeric(this Parser<IEnumerable<char>> lhs)
        {
            return from numberString in lhs.String()
                   select long.Parse(numberString);
        }
    }
}
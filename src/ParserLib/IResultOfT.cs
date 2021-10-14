namespace ParserLib
{
    using System.Collections.Generic;

    public interface IResult<out T>
    {
        T Value { get; }

        bool WasSuccessful { get; }

        string Message { get; }

        IEnumerable<string> Expectations { get; }

        IInput Remainder { get; }
    }
}
namespace ParserLib
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Result
    {
        public static IResult<T> Success<T>(T value, IInput remainder) => new ResultOf<T>(value, remainder);

        public static IResult<T> Fail<T>(IInput remainder, string message, IEnumerable<string> expecations) => new ResultOf<T>(remainder, message, expecations);

        private class ResultOf<T> : IResult<T>
        {
            private readonly T value;
            private readonly IInput remainder;
            private readonly bool successful;
            private readonly string message;
            private readonly IEnumerable<string> expectations;

            public ResultOf(T value, IInput remainder)
            {
                this.value = value;
                this.remainder = remainder;
                this.successful = true;
                this.message = null;
                this.expectations = Enumerable.Empty<string>();
            }

            public ResultOf(IInput remainder,
                            string message,
                            IEnumerable<string> expecations)
            {
                this.value = default(T);
                this.remainder = remainder;
                this.successful = false;
                this.message = message;
                this.expectations = expecations;
            }
            public T Value => this.value;

            public bool WasSuccessful => this.successful;

            public string Message => this.message;

            public IEnumerable<string> Expectations => this.expectations;

            public IInput Remainder => this.remainder;
        }
    }
}
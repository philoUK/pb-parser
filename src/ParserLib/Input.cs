namespace ParserLib
{
    using System;

    public interface IInput
    {
        IInput Advance();
        CurrentChar CurrentValue();
        bool HasValue();
        bool IsEof();

        int Position { get; }
    }

    public class Input : IInput
    {
        private readonly int lineNumber;
        private readonly int columnNumber;

        private readonly int internalPosition;

        public Input(string source)
            : this(source, 0, 1, 1)
        {
        }

        private Input(string source, int internalPosition, int columnNumber, int lineNumber)
        {
            this.source = source;
            this.internalPosition = internalPosition;
            this.columnNumber = columnNumber;
            this.lineNumber = lineNumber;
        }

        private string source;

        public bool IsEof() => this.internalPosition >= (this.source ?? string.Empty).Length;

        public bool HasValue() => !this.IsEof();

        public int Position => this.internalPosition;

        public CurrentChar CurrentValue()
        {
            if (this.IsEof())
            {
                throw new InvalidOperationException("No CurrentValue at End Of File");
            }
            return new CurrentChar(this.lineNumber, this.columnNumber, this.source[this.internalPosition]);
        }

        public IInput Advance()
        {
            if (this.IsEof())
            {
                throw new InvalidOperationException("Cannot advance past end of file");
            }
            var newPosition = new Tuple<int, int, int>(this.internalPosition + 1, this.columnNumber + 1, this.lineNumber);
            var results = SkipNewLines(newPosition);
            return new Input(this.source, results.Item1, results.Item2, results.Item3);
        }

        private Tuple<int, int, int> SkipNewLines(Tuple<int, int, int> t)
        {
            var newInternalPosition = t.Item1;
            var newColumnNumber = t.Item2;
            var newLineNumber = t.Item3;

            while (newInternalPosition < this.source.Length && this.source[newInternalPosition] == '\n')
            {
                newInternalPosition++;
                newColumnNumber = 1;
                newLineNumber++;
            }

            return new Tuple<int, int, int>(newInternalPosition, newColumnNumber, newLineNumber);
        }
    }
}
namespace ParserLib
{
    public class CurrentChar
    {
        private readonly int lineNumber;
        private readonly int columnNumber;
        private readonly char value;

        public CurrentChar(int lineNumber, int columnNumber, char value)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
            this.value = value;
        }

        public int Line => this.lineNumber;

        public int Column => this.columnNumber;

        public char Value => this.value;
    }
}
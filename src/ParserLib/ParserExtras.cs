namespace ParserLib
{
    public static partial class Parse
    {
        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        public static DeferredParser<T> Defer<T>()
        {
            return new DeferredParser<T>();
        }
    }
}
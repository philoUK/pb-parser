namespace ParserLib
{
    using System;
    public class DeferredParser<T>
    {
        private Func<Parser<T>> implementation;

        public Parser<T> Parser()
        {
            if (this.implementation == null)
            {
                return Parse.Return(default(T));
            }
            return this.implementation();
        }

        public void Implentation(Func<Parser<T>> implementation)
        {
            this.implementation = implementation;
        }
    }
}
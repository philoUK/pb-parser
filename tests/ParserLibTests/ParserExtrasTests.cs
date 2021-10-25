namespace ParserLibTests
{
    using ParserLib;
    using Xunit;

    public class ParserExtrasTests
    {
        [Fact]
        public void Return_AlwaysWorks()
        {
            var results = Parse.Return("works")(
                new Input(""));
            Assert.True(results.WasSuccessful);
            Assert.Equal("works", results.Value);
        }

        [Fact]
        public void Defer_RecursiveParser_Works()
        {

            var intParser = Parse.Digit().OneOrMore().Numeric();
            var midParser = Parse.Defer<long>();
            var innerBracketedParser =
                from leftBrackets in Parse.Char('(')
                from value in midParser.Parser()
                from rightBrackets in Parse.Char(')')
                select value;
            var exprParser = intParser.Or(innerBracketedParser);
            midParser.Implentation(() => exprParser);
            var result = exprParser(new Input("(((3)))"));
            Assert.True(result.WasSuccessful);
            Assert.Equal(3, result.Value);
        }

        [Fact]
        public void Token_StripsLeadingAndTrailingWhitespace()
        {
            var textParser = Parse.AnyChar().OneOrMore().String().Token();
            var results = textParser(new Input("   hello world  \t\n"));
            Assert.True(results.WasSuccessful);
            Assert.Equal("hello world", results.Value);
        }
    }
}
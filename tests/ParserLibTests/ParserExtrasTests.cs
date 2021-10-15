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
    }
}
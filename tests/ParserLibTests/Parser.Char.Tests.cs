namespace ParserLibTests
{
    using System.Linq;
    using ParserLib;
    using Xunit;

    public class ParserCharTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("not x")]
        public void Char_FailsWhenInputIs(string input)
        {
            var sut = Parse.Char('x');
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [Fact]
        public void Char_PassesWhenFirstCharacterMatches()
        {
            var sut = Parse.Char('t');
            var results = sut(new Input("test"));
            Assert.True(results.WasSuccessful);
        }

        [Fact]
        public void Char_AdvancesInputWhenSuccessful()
        {
            var sut = Parse.Char('t');
            var results = sut(new Input("test"));
            Assert.Equal('e', results.Remainder.CurrentValue().Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not a number")]
        public void Digit_NotANumber_Fails(string input)
        {
            var sut = Parse.Digit();
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [Theory]
        [InlineData("0test")]
        [InlineData("1test")]
        [InlineData("2test")]
        [InlineData("3test")]
        [InlineData("4test")]
        [InlineData("5test")]
        [InlineData("6test")]
        [InlineData("7test")]
        [InlineData("8test")]
        [InlineData("9test")]
        public void Digit_StartsWithNumber_Succeeds(string input)
        {
            var sut = Parse.Digit();
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [Theory]
        [InlineData("B")]
        [InlineData("b")]
        public void AnyCaseChar_WrongChar_Fails(string input)
        {
            var sut = Parse.AnyCaseChar('a');
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("a")]
        public void AnyCaseChar_CorrectChar_Succeeds(string input)
        {
            var sut = Parse.AnyCaseChar('a');
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("z")]
        public void Letter_NotANumber_Succeeds(string input)
        {
            var sut = Parse.Letter();
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [Theory]
        [InlineData("")]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        [InlineData("5")]
        [InlineData("6")]
        [InlineData("7")]
        [InlineData("8")]
        [InlineData("9")]
        [InlineData("?")]
        public void Letter_NotALetter_Fails(string input)
        {
            var sut = Parse.Letter();
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [InlineData("")]
        [InlineData("?")]
        [Theory]
        public void LetterOrDigit_Neither_Fails(string input)
        {
            var sut = Parse.LetterOrDigit();
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [InlineData("0")]
        [InlineData("A")]
        [InlineData("a")]
        [Theory]
        public void LetterOrDigit_Either_Succeeds(string input)
        {
            var sut = Parse.LetterOrDigit();
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [InlineData("")]
        [InlineData("b")]
        [InlineData("bb")]
        [InlineData("bb c")]
        [Theory]
        public void ZerOrMoreLetter_CorrectInput_Successful(string input)
        {
            var sut = Parse.Letter().ZeroOrMore();
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [InlineData("")]
        [InlineData(" something more")]
        [Theory]
        public void OneOrMoreLetter_IncorrectInput_Fails(string input)
        {
            var sut = Parse.Letter().OneOrMore();
            var results = sut(new Input(input));
            Assert.False(results.WasSuccessful);
        }

        [InlineData("S")]
        [InlineData("Should pass")]
        [Theory]
        public void OneOrMoreLetter_CorrectInput_Succeeds(string input)
        {
            var sut = Parse.Letter().OneOrMore();
            var results = sut(new Input(input));
            Assert.True(results.WasSuccessful);
        }

        [Fact]
        public void String_CorrectInput_Succeeds()
        {
            var sut = Parse.Letter().OneOrMore().String();
            var results = sut(new Input("Should pass"));
            Assert.True(results.WasSuccessful);
            Assert.Equal("Should", results.Value);
        }

        [Fact]
        public void String_SpecificationMatched_Succeeds()
        {
            var spec = Parse.Letter().Then<char, string>(letter =>
            {
                return i =>
                {
                    var rhs = Parse.LetterOrDigit().ZeroOrMore()(i);
                    if (rhs.WasSuccessful)
                    {
                        return Result.Success(
                            letter.ToString() +
                             string.Concat(rhs.Value),
                             rhs.Remainder
                        );
                    }
                    return Result.Fail<string>(i,
                        "Expected [A-Z][a-z][0-9]*",
                        Enumerable.Empty<string>());
                };
            }, "Expected Identifier [A-Za-z][A-Za-z0-9]*");
            // fail the spec
            var wrongResult = spec(new Input("0id"));
            Assert.False(wrongResult.WasSuccessful);
            var rightResult = spec(new Input("id0"));
            Assert.True(rightResult.WasSuccessful);
            Assert.Equal("id0", rightResult.Value);
        }
    }

}
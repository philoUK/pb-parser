namespace ParserLibTests
{
    using Xunit;
    using ParserLib;
    using System;

    public class InputTests
    {
        [Fact]
        public void IsEof_EmptyInput_IsTrue()
        {
            var sut = new InputBuilder().WithEmptyInput().Build();
            Assert.True(sut.IsEof());
        }

        [Fact]
        public void IsEof_NullInput_IsTrue()
        {
            var sut = new InputBuilder().WithInputOf(null).Build();
            Assert.True(sut.IsEof());
        }

        [Fact]
        public void IsEof_NonEmptyInput_IsFalse()
        {
            var sut = new InputBuilder().WithInputOf("not empty").Build();
            Assert.False(sut.IsEof());
        }

        [Fact]
        public void HasValue_EmptyInput_IsFalse()
        {
            var sut = new InputBuilder().WithEmptyInput().Build();
            Assert.False(sut.HasValue());
        }

        [Fact]
        public void HasValue_NonEmptyInput_IsTrue()
        {
            var sut = new InputBuilder().WithInputOf("test").Build();
            Assert.True(sut.HasValue());
        }

        [Fact]
        public void CurrentValue_EmptyInput_ThrowsException()
        {
            var sut = new InputBuilder().WithEmptyInput().Build();
            Assert.Throws<InvalidOperationException>(() => sut.CurrentValue());
        }

        [Fact]
        public void CurrentValue_NonEmptyInput_ReturnsLineColumnAndValue()
        {
            var sut = new InputBuilder().WithInputOf("test").Build();
            var result = sut.CurrentValue();
            Assert.Equal(1, result.Line);
            Assert.Equal(1, result.Column);
            Assert.Equal('t', result.Value);
        }

        [Fact]
        public void Advance_EmptyInput_ThrowsException()
        {
            var sut = new InputBuilder().WithEmptyInput().Build();
            Assert.Throws<InvalidOperationException>(() => sut.Advance());
        }

        [Fact]
        public void Advance_NonEmptyInput_ShiftsToNextCharacter()
        {
            var sut = new InputBuilder().WithInputOf("test").Build();
            var results = sut.Advance().CurrentValue();
            Assert.Equal(1, results.Line);
            Assert.Equal(2, results.Column);
            Assert.Equal('e', results.Value);
        }

        [Fact]
        public void Advance_WithNewLine_SkipsToNextLine()
        {
            var sut = new InputBuilder().WithInputOf("a\nz").Build();
            var results = sut.Advance().CurrentValue();
            Assert.Equal(2, results.Line);
            Assert.Equal(1, results.Column);
            Assert.Equal('z', results.Value);
        }

        [Fact]
        public void Advance_ManyNewLines_SkipsAllNewLines()
        {
            var sut = new InputBuilder().WithInputOf("a\n\nz").Build();
            var results = sut.Advance().CurrentValue();
            Assert.Equal(1, results.Column);
            Assert.Equal(3, results.Line);
            Assert.Equal('z', results.Value);
        }

        [Fact]
        public void Advance_SingleChar_CausesEOF()
        {
            var sut = new InputBuilder().WithInputOf("a").Build();
            Assert.True(sut.Advance().IsEof());
        }
        private class InputBuilder
        {
            private string value;

            public InputBuilder WithEmptyInput()
            {
                this.value = null;
                return this;
            }

            public Input Build()
            {
                return new Input(this.value);
            }

            internal InputBuilder WithInputOf(string value)
            {
                this.value = value;
                return this;
            }
        }
    }


}
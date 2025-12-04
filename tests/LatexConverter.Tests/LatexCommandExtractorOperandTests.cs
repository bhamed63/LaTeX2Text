using System.Collections.Generic;
using System.Linq;
using LatexConverter.Parsing;
using Xunit;

namespace LatexConverter.Tests
{
    public class LatexCommandExtractorOperandTests
    {
        private readonly LatexParser _parser = new LatexParser();
        private readonly LatexCommandExtractor _extractor = new LatexCommandExtractor();

        [Theory]
        [InlineData("a+b", "a", "b")]
        [InlineData("var1 = var2", "var1", "var2")]
        [InlineData("x > y", "x", "y")]
        [InlineData("p < q", "p", "q")]
        [InlineData("a+b-c", "a", "b", "c")]
        public void TestSimpleOperands(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }

        [Theory]
        [InlineData("a + b c", "a", "b")]
        [InlineData("a+b c d", "a", "b")]
        public void TestOperandsWithDelimiters(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }

        [Theory]
        [InlineData("\\frac{a+b}{c}", "a", "b", "c")]
        [InlineData("\\sqrt[n]{x+y}", "n", "x", "y")]
        public void TestOperandsInCommands(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }
    }
}

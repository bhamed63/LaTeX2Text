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
        [InlineData("\\frac{a+b}{c}", "a", "b")]
        [InlineData("\\sqrt[n]{x+y}", "x", "y")]
        public void TestOperandsInCommands(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }

        [Theory]
        [InlineData("this is sample of \\sqrt{x} and i + x - this is a sample of = advanture.", "advanture", "i", "of", "this", "x")]
        [InlineData("A simple case is a=b.", "a", "b")]
        public void TestOperandsInMixedText(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }

        [Theory]
        [InlineData("var_1+var_2", "var")]
        [InlineData("a_b-c_d", "b", "c")]
        [InlineData("a\\b-c\\d", "c")]
        public void TestOperandsWithSubscriptDelimiters(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.OrderBy(a => a));
        }
    }
}

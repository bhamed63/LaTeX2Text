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
        [InlineData("var1 = var2")]
        [InlineData("x > y", "x", "y")]
        [InlineData("p < q", "p", "q")]
        [InlineData("a+b-c", "a", "b", "c")]
        public void TestSimpleOperands(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).Distinct().OrderBy(a => a));
        }

        [Theory]
        [InlineData("a + b c", "a", "b")]
        [InlineData("a+b c d", "a", "b")]
        public void TestOperandsWithDelimiters(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Theory]
        [InlineData("\\frac{a+b}{c}", "a", "b")]
        [InlineData("\\sqrt[n]{x+y}", "x", "y")]
        public void TestOperandsInCommands(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Theory]
        [InlineData("this is sample of \\sqrt{x} and i + x - this is a sample of = advanture.", "i", "of", "x")]
        [InlineData("A simple case is a=b.", "a", "b")]
        public void TestOperandsInMixedText(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Theory]
        [InlineData("var_1+var_2", "var_1", "var_2")]
        [InlineData("a_b-c_d", "a_b", "c_d")]
        public void TestCommandssWithSubscriptDelimiters(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var commands = _extractor.ExtractCommands(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), commands.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Theory]
        [InlineData("var_1+var_2")]
        [InlineData("a_b-c_d")]
        [InlineData("a\\b-c\\d", "c")]
        public void TestOperandsWithSubscriptDelimiters(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Theory]
        [InlineData("baseball of radius r = 5.8 cm is at room temperature T = 19.8 C.  The baseball has  ", "r", "T")]
        public void Test_MoreThan_One_Operand_In_One_TextNode(string input, params string[] expectedArguments)
        {
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Equal(expectedArguments.OrderBy(a => a), arguments.SelectMany(c => c.TextArguments).OrderBy(a => a));
        }

        [Fact]
        public void Extract_No_Item_When_Input_Contains_Operator_With_Length_Greater_Than_2()
        {
            string input = "and the Stefan-Boltzman constant";
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Empty(arguments.SelectMany(c => c.TextArguments));
        }

        [Fact]
        public void Extract_One_Item_When_Input_Contains_Operator_With_Length_2()
        {
            string input = "and the St-Fo constant";
            var nodes = _parser.Parse(input);
            var arguments = _extractor.ExtractArgumentsFromOperators(nodes);
            Assert.Single(arguments);
            Assert.Contains("St", arguments.SelectMany(c => c.TextArguments));
            Assert.Contains("Fo", arguments.SelectMany(c => c.TextArguments));
        }
    }
}

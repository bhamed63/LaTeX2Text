using Xunit;
using System.Collections.Generic;

namespace LatexConverter.Tests
{
    public class HierarchicalParserTests
    {
        [Fact]
        public void Parse_WithPlainText_ShouldReturnSingleTextNode()
        {
            var text = "the result is 5 + z";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var textNode = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal(text, textNode.Text);
        }

        [Fact]
        public void Parse_WithTextAndCommand_ShouldReturnCorrectNodes()
        {
            var text = "the result is 5+ \\sqrt{0} x";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(3, nodes.Count);
            var textNode1 = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal("the result is 5+ ", textNode1.Text);
            Assert.IsType<RootNode>(nodes[1]);
            var textNode2 = Assert.IsType<TextNode>(nodes[2]);
            Assert.Equal(" x", textNode2.Text);
        }

        [Fact]
        public void Parse_WithTextCommandAndMath_ShouldReturnCorrectNodes()
        {
            var text = "the result is 5+ \\sqrt{0} x \\(4 + 5\\)";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(4, nodes.Count);
            var textNode1 = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal("the result is 5+ ", textNode1.Text);
            Assert.IsType<RootNode>(nodes[1]);
            var textNode2 = Assert.IsType<TextNode>(nodes[2]);
            Assert.Equal(" x ", textNode2.Text);
            var mathNode = Assert.IsType<MathNode>(nodes[3]);
            Assert.Equal(3, mathNode.Children.Count);
        }

        [Fact]
        public void Parse_WithMathDisplay_ShouldReturnMathNode()
        {
            var text = "\\[\\frac{1}{2}\\]";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var mathNode = Assert.IsType<MathNode>(nodes[0]);
            Assert.Single(mathNode.Children);
            Assert.IsType<FracNode>(mathNode.Children[0]);
        }
        [Fact]
        public void Parse_WithSinCommand_ShouldReturnCommandAndTextNodes()
        {
            var text = "\\sin(x)";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(2, nodes.Count);
            var commandNode = Assert.IsType<CommandNode>(nodes[0]);
            Assert.Equal(CommandNames.Sin, commandNode.Command);
            Assert.Empty(commandNode.Args);
            var textNode = Assert.IsType<TextNode>(nodes[1]);
            Assert.Equal("(x)", textNode.Text);
        }

        [Theory]
        [InlineData("rod: dE_x = -k * lambda * x * dx / (a^2 + x^2)^(3/2).", 8)]
        [InlineData("V(r) = (q / (4 * \\pi * \\epsilon_0 * a_0)) * exp(-2 * r / a_0) * (1 + a_0/r)", 11)]
        [InlineData("the quantity (x/2 squared plus l squared) raised to the 3/2 power", 1)]
        [InlineData(@"a \pm b", 3)]
        [InlineData(@"a \neq b", 3)]
        [InlineData(@"\forall x", 2)]
        [InlineData(@"\int_a^b f(x) dx", 2)]
        [InlineData(@"\lim_{x \to \infty} f(x)", 2)]
        [InlineData(@"\binom{n}{k}", 1)]
        [InlineData(@"The result is \approx 5.", 3)]
        [InlineData("\r\n\\alpha\r\n\\beta\r\n", 5)]
        [InlineData(@"\overline{xyz}", 1)]
        [InlineData(@"\sqrt[3]{x}", 1)]
        [InlineData(@"\sqrt[31]{y^2 + z^3}", 1)]
        public void Parse_WithVariousInputs_ShouldReturnCorrectNodeCount(string input, int expectedNodeCount)
        {
            var tokens = Tokenizer.Tokenize(input);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();
            Assert.Equal(expectedNodeCount, nodes.Count);
        }
    }
}

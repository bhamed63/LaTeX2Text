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
            Assert.Equal(5, mathNode.Children.Count);
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
    }
}

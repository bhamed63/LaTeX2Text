using Xunit;
using System.Collections.Generic;
using System.Linq;

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

        // New tests for the provided cases

        [Fact]
        public void Parse_WithSqrtWithOptionalArgument_ShouldReturnCorrectRootNode()
        {
            var text = @"\sqrt[2]{m}";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var rootNode = Assert.IsType<RootNode>(nodes[0]);
            var degree = Assert.IsType<GroupNode>(rootNode.Degree);
            var degreeText = Assert.IsType<TextNode>(degree.Body.First());
            Assert.Equal("2", degreeText.Text);

            var radicand = Assert.IsType<GroupNode>(rootNode.Radicand);
            var radicandText = Assert.IsType<TextNode>(radicand.Body.First());
            Assert.Equal("m", radicandText.Text);
        }

        [Fact]
        public void Parse_WithComplexSqrt_ShouldReturnCorrectRootNode()
        {
            var text = @"\sqrt[31]{y^2 + z^3}";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var rootNode = Assert.IsType<RootNode>(nodes[0]);
            var degree = Assert.IsType<GroupNode>(rootNode.Degree);
            var degreeText = Assert.IsType<TextNode>(degree.Body.First());
            Assert.Equal("31", degreeText.Text);

            var radicand = Assert.IsType<GroupNode>(rootNode.Radicand);
            Assert.Equal(5, radicand.Body.Count);
        }

        [Fact]
        public void Parse_WithThinSpace_ShouldMergeIntoSingleTextNode()
        {
            var text = @"a\,b";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var textNode = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal("a b", textNode.Text);
        }

        [Fact]
        public void Parse_WithCosAlpha_ShouldReturnCorrectNodes()
        {
            var text = @"\cos \alpha";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(3, nodes.Count);
            var cosNode = Assert.IsType<CommandNode>(nodes[0]);
            Assert.Equal(CommandNames.Cos, cosNode.Command);
            var alphaNode = Assert.IsType<CommandNode>(nodes[2]);
            Assert.Equal(CommandNames.Alpha, alphaNode.Command);
        }

        [Fact]
        public void Parse_WithPlainTextContainingUnderscores_ShouldReturnSingleTextNode()
        {
            var text = @"A baryon consists of ____ quark(s) and ____ antiquark(s).";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Single(nodes);
            var textNode = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal(text, textNode.Text);
        }

        [Fact]
        public void Parse_WithReAndIm_ShouldReturnCorrectNodes()
        {
            var text = @"The \Re and the \Im.";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(5, nodes.Count);
            Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal("The ", ((TextNode)nodes[0]).Text);
            Assert.IsType<CommandNode>(nodes[1]);
            Assert.Equal(CommandNames.Re, ((CommandNode)nodes[1]).Command);
            Assert.IsType<TextNode>(nodes[2]);
            Assert.Equal(" and the ", ((TextNode)nodes[2]).Text);
            Assert.IsType<CommandNode>(nodes[3]);
            Assert.Equal(CommandNames.Im, ((CommandNode)nodes[3]).Command);
            Assert.IsType<TextNode>(nodes[4]);
            Assert.Equal(".", ((TextNode)nodes[4]).Text);
        }

        [Fact]
        public void Parse_WithMultiplicationAndSuperscript_ShouldReturnCorrectNodes()
        {
            var text = @"4.0 \times 10^{-4}";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(4, nodes.Count);
            Assert.IsType<TextNode>(nodes[0]);
            Assert.IsType<CommandNode>(nodes[1]);
            Assert.IsType<ScriptNode>(nodes[3]);
        }

        [Fact]
        public void Parse_WithSummation_ShouldReturnCorrectNodes()
        {
            var text = @"\sum_{i=1}^{n} i^2";
            var tokens = Tokenizer.Tokenize(text);
            var parser = new HierarchicalParser(tokens);
            var nodes = parser.Parse();

            Assert.Equal(3, nodes.Count);
            var sumNode = Assert.IsType<CommandNode>(nodes[0]);
            Assert.Equal(CommandNames.Sum, sumNode.Command);
            Assert.NotNull(sumNode.Subscript);
            Assert.NotNull(sumNode.Superscript);

            var iSqNode = Assert.IsType<ScriptNode>(nodes[2]);
            Assert.True(iSqNode.IsSuperscript);
        }
    }
}

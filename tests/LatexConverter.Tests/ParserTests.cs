using Xunit;
using System.Collections.Generic;
using LatexConverter.Ast;
using System.Linq;
using System;

namespace LatexConverter.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Parse_EmptyString_ReturnsEmptyList()
        {
            var tokens = Tokenizer.Tokenize("");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Empty(nodes);
        }

        [Fact]
        public void Parse_SimpleText_ReturnsTextNode()
        {
            var tokens = Tokenizer.Tokenize("hello");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var textNode = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal("hello", textNode.Text);
        }

        [Fact]
        public void Parse_Command_ReturnsCommandNode()
        {
            var tokens = Tokenizer.Tokenize(@"\alpha");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var commandNode = Assert.IsType<CommandNode>(nodes[0]);
            Assert.Equal(@"\alpha", commandNode.Command);
        }

        [Fact]
        public void Parse_Group_ReturnsGroupNode()
        {
            var tokens = Tokenizer.Tokenize("{abc}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var groupNode = Assert.IsType<GroupNode>(nodes[0]);
            Assert.Single(groupNode.Body);
            var textNode = Assert.IsType<TextNode>(groupNode.Body[0]);
            Assert.Equal("abc", textNode.Text);
        }

        [Fact]
        public void Parse_Superscript_ReturnsScriptNode()
        {
            var tokens = Tokenizer.Tokenize("a^2");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var scriptNode = Assert.IsType<ScriptNode>(nodes[0]);
            Assert.True(scriptNode.IsSuperscript);
            var baseNode = Assert.IsType<TextNode>(scriptNode.Base);
            Assert.Equal("a", baseNode.Text);
            var script = Assert.IsType<TextNode>(scriptNode.Script);
            Assert.Equal("2", script.Text);
        }

        [Fact]
        public void Parse_Negetive_Superscript_ReturnsScriptNode()
        {
            var tokens = Tokenizer.Tokenize("a^-2");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var scriptNode = Assert.IsType<ScriptNode>(nodes[0]);
            Assert.True(scriptNode.IsSuperscript);
            var baseNode = Assert.IsType<TextNode>(scriptNode.Base);
            Assert.Equal("a", baseNode.Text);
            var script = Assert.IsType<TextNode>(scriptNode.Script);
            Assert.Equal("-2", script.Text);
        }

        [Fact]
        public void Parse_Subscript_ReturnsScriptNode()
        {
            var tokens = Tokenizer.Tokenize("a_1");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var scriptNode = Assert.IsType<ScriptNode>(nodes[0]);
            Assert.False(scriptNode.IsSuperscript);
            var baseNode = Assert.IsType<TextNode>(scriptNode.Base);
            Assert.Equal("a", baseNode.Text);
            var script = Assert.IsType<TextNode>(scriptNode.Script);
            Assert.Equal("1", script.Text);
        }

        [Fact]
        public void Parse_Frac_ReturnsFracNode()
        {
            var tokens = Tokenizer.Tokenize(@"\frac{a}{b}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var fracNode = Assert.IsType<FracNode>(nodes[0]);
            var numerator = Assert.IsType<GroupNode>(fracNode.Numerator);
            var denominator = Assert.IsType<GroupNode>(fracNode.Denominator);
            var numText = Assert.IsType<TextNode>(numerator.Body[0]);
            var denText = Assert.IsType<TextNode>(denominator.Body[0]);
            Assert.Equal("a", numText.Text);
            Assert.Equal("b", denText.Text);
        }

        [Fact]
        public void Parse_Sqrt_ReturnsRootNode()
        {
            var tokens = Tokenizer.Tokenize(@"\sqrt{x}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var rootNode = Assert.IsType<RootNode>(nodes[0]);
            var radicand = Assert.IsType<GroupNode>(rootNode.Radicand);
            var radicandText = Assert.IsType<TextNode>(radicand.Body[0]);
            Assert.Equal("x", radicandText.Text);
        }

        [Fact]
        public void Parse_Root_ReturnsRootNode()
        {
            var tokens = Tokenizer.Tokenize(@"\sqrt[3]{x}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var rootNode = Assert.IsType<RootNode>(nodes[0]);
            var degree = Assert.IsType<GroupNode>(rootNode.Degree);
            var degreeText = Assert.IsType<TextNode>(degree.Body[0]);
            Assert.Equal("3", degreeText.Text);
        }

        [Fact]
        public void Parse_Matrix_ReturnsMatrixNode()
        {
            var tokens = Tokenizer.Tokenize(@"\begin{pmatrix} a & b \\ c & d \end{pmatrix}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var matrixNode = Assert.IsType<MatrixNode>(nodes[0]);
            Assert.Equal("a & b \\\\ c & d", matrixNode.Content);
        }

        [Fact]
        public void Parse_InlineMath_ReturnsMathNode()
        {
            var tokens = Tokenizer.Tokenize(@"\(x\)");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var mathNode = Assert.IsType<MathNode>(nodes[0]);
            Assert.Single(mathNode.Content);
            var textNode = Assert.IsType<TextNode>(mathNode.Content[0]);
            Assert.Equal("x", textNode.Text);
        }

        [Fact]
        public void Parse_DisplayMath_ReturnsMathNode()
        {
            var tokens = Tokenizer.Tokenize(@"\[x^2\]");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var mathNode = Assert.IsType<MathNode>(nodes[0]);
            Assert.Single(mathNode.Content);
            var scriptNode = Assert.IsType<ScriptNode>(mathNode.Content[0]);
            Assert.True(scriptNode.IsSuperscript);
        }

        [Fact]
        public void Parse_ComplexExpression_ReturnsCorrectTree()
        {
            var tokens = Tokenizer.Tokenize(@"\frac{-b \pm \sqrt{b^2 - 4ac}}{2a}");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            Assert.IsType<FracNode>(nodes[0]);
        }

        [Theory]
        [InlineData(@"a+b", typeof(TextNode), typeof(TextNode), typeof(TextNode))]
        [InlineData(@"\alpha\beta", typeof(CommandNode), typeof(CommandNode))]
        [InlineData(@"\sin{x}", typeof(CommandNode))]
        [InlineData(@"\log_2{x}", typeof(CommandNode), typeof(TextNode), typeof(GroupNode))]
        [InlineData(@"\vec{v}_0", typeof(ScriptNode))]
        [InlineData(@"a^{b^c}", typeof(ScriptNode))]
        [InlineData(@"\binom{n}{k}", typeof(BinomNode))]
        [InlineData(@"x_{i+1}", typeof(ScriptNode))]
        [InlineData(@"\int_0^\infty f(x) dx", typeof(CommandNode), typeof(TextNode), typeof(TextNode), typeof(TextNode), typeof(TextNode), typeof(TextNode), typeof(TextNode), typeof(TextNode))]
        [InlineData(@"\lim_{x \to 0}", typeof(CommandNode))]
        [InlineData(@"\sum_{i=1}^n i", typeof(CommandNode), typeof(TextNode), typeof(TextNode))]
        [InlineData(@"\hat{i}", typeof(CommandNode))]
        [InlineData(@"\bar{x}", typeof(CommandNode))]
        [InlineData(@"\tilde{y}", typeof(CommandNode), typeof(GroupNode))]
        [InlineData(@"\dot{z}", typeof(CommandNode), typeof(GroupNode))]
        [InlineData(@"\ddot{q}", typeof(CommandNode), typeof(GroupNode))]
        [InlineData(@"\overline{AB}", typeof(CommandNode))]
        [InlineData(@"\underline{CD}", typeof(CommandNode), typeof(GroupNode))]
        [InlineData(@"\mathbf{X}", typeof(CommandNode))]
        public void Parse_VariousInputs_ReturnsCorrectNodeTypes(string input, params System.Type[] expectedNodeTypes)
        {
            var tokens = Tokenizer.Tokenize(input);
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Equal(expectedNodeTypes.Length, nodes.Count);
            for (int i = 0; i < expectedNodeTypes.Length; i++)
            {
                Assert.IsType(expectedNodeTypes[i], nodes[i]);
            }
        }
    }
}

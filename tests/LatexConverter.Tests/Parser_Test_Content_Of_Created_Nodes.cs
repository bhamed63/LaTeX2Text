using LatexConverter;
using LatexConverter.Parsing;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace LatexConvertorTests
{ 
    public class Parser_Test_Content_Of_Created_Nodes
    {
        LatexParser _latexParser;

        public Parser_Test_Content_Of_Created_Nodes()
        {
            _latexParser = new LatexParser();
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_ToString_Is_Equal_To_Given_Text()
        {
            Assert.True(callToString(_latexParser.Parse("\\frac{x}{y}, it is simple Latex.")) == "\\frac{x}{y}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.")) == "here it is a \\sqrt{x}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("here it is a \\sqrt{x}")) == "here it is a \\sqrt{x}");
            Assert.True(callToString(_latexParser.Parse("\\sqrt{x}")) == "\\sqrt{x}");
            Assert.True(callToString(_latexParser.Parse("here it is a \\sqrt{x}, it is \\alpha simple Latex.")) == "here it is a \\sqrt{x}, it is \\alpha simple Latex.");
            Assert.True(callToString(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.")) == "here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == "here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == "here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == "\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.");
            Assert.True(callToString(_latexParser.Parse("\\(it \\) is a sqrt.")) == "\\(it \\) is a sqrt.");
            Assert.True(callToString(_latexParser.Parse("\\(it \\)")) == "\\(it \\)");
            Assert.True(callToString(_latexParser.Parse("\\(it  \\sqrt{\\frac{x}{\\alpha}} \\)")) == "\\(it  \\sqrt{\\frac{x}{\\alpha}} \\)");
            Assert.True(callToString(_latexParser.Parse("XXXXXXXXXXXXXXX")) == "XXXXXXXXXXXXXXX");
        }

        private string callToString(List<AstNode> latexNodes)
        {
            return string.Join("", latexNodes.Select(c => c.ToString()));
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Lim_Node()
        {
            var nodes = _latexParser.Parse(@"\lim_{x \to 0}");
            Assert.True(nodes[0] is LimNode);
            var limNode = nodes[0] as LimNode;
            Assert.True(limNode.Subscript is GroupNode);
            var groupNode = limNode.Subscript as GroupNode;
            Assert.True(groupNode.Body[0] is TextNode);
            var textNode = groupNode.Body[0] as TextNode;
            Assert.Equal("x ", textNode.Text);
            Assert.True(groupNode.Body[1] is CommandNode);
            var commandNode = groupNode.Body[1] as CommandNode;
            Assert.Equal("to", commandNode.Command);
            Assert.True(groupNode.Body[2] is TextNode);
            textNode = groupNode.Body[2] as TextNode;
            Assert.Equal(" 0", textNode.Text);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Lim_Node_Nested()
        {
            var nodes = _latexParser.Parse(@"Consider the limit \lim_{n \to \infty} \frac{1}{n}");
            Assert.True(nodes[0] is TextNode);
            var textNode = nodes[0] as TextNode;
            Assert.Equal("Consider the limit ", textNode.Text);
            Assert.True(nodes[1] is LimNode);
            var limNode = nodes[1] as LimNode;
            Assert.True(limNode.Subscript is GroupNode);
            var groupNode = limNode.Subscript as GroupNode;
            Assert.True(groupNode.Body[0] is TextNode);
            textNode = groupNode.Body[0] as TextNode;
            Assert.Equal("n ", textNode.Text);
            Assert.True(groupNode.Body[1] is CommandNode);
            var commandNode = groupNode.Body[1] as CommandNode;
            Assert.Equal("to", commandNode.Command);
            Assert.True(groupNode.Body[2] is TextNode);
            textNode = groupNode.Body[2] as TextNode;
            Assert.Equal(" ", textNode.Text);
            Assert.True(groupNode.Body[3] is CommandNode);
            commandNode = groupNode.Body[3] as CommandNode;
            Assert.Equal("infty", commandNode.Command);
            Assert.True(nodes[2] is TextNode);
            textNode = nodes[2] as TextNode;
            Assert.Equal(" ", textNode.Text);
            Assert.True(nodes[3] is FracNode);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Nodes()
        {
            var nodes = _latexParser.Parse("\\frac{x}{y}, it is simple Latex.");
            Assert.True(nodes[0] is FracNode);
            Assert.True(nodes[1] is TextNode);

            nodes = _latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.");
            Assert.True(nodes[0] is TextNode);
            Assert.True(nodes[1] is RootNode);
            Assert.True(nodes[2] is TextNode);

            nodes = _latexParser.Parse("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.");
            Assert.True(nodes[0] is TextNode);
            Assert.True(nodes[1] is RootNode);
            Assert.True(nodes[1] is RootNode rootNode1 && rootNode1.Radicand is GroupNode radicandGroup && radicandGroup.Body[0] is FracNode);
            Assert.True(nodes[2] is TextNode);

            nodes = _latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.");
            Assert.True(nodes[0] is GroupNode);
            Assert.True(nodes[1] is TextNode);
            Assert.True(nodes[2] is RootNode rootNode2 && rootNode2.Radicand is GroupNode radicandGroup2 && radicandGroup2.Body[0] is FracNode);
            Assert.True(nodes[3] is TextNode);
        }
    }
}

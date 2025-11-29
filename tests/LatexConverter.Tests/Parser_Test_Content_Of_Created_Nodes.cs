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

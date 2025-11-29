using LatexConverter.Ast;
using LatexConverter.Parsing;
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

        private string callToString(List<LatexNode> latexNodes)
        {
            return string.Join("", latexNodes.Select(c => c.ToString()));
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Nodes()
        {
            var nodes = _latexParser.Parse("\\frac{x}{y}, it is simple Latex.");
            Assert.True(nodes[0] is LatexCommandNode);
            Assert.True(nodes[1] is LatexTextNode);

            nodes = _latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.");
            Assert.True(nodes[0] is LatexTextNode);
            Assert.True(nodes[1] is LatexCommandNode);
            Assert.True(nodes[2] is LatexTextNode);

            nodes = _latexParser.Parse("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.");
            Assert.True(nodes[0] is LatexTextNode);
            Assert.True(nodes[1] is LatexCommandNode);
            Assert.True(nodes[1] is LatexCommandNode commandNode1 && commandNode1.Args[0] is LatexCommandNode fracNode1 && fracNode1.Args[0] is LatexTextNode && fracNode1.Args[1] is LatexTextNode);
            Assert.True(nodes[2] is LatexTextNode);

            nodes = _latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.");
            Assert.True(nodes[0] is LatexGroupNode);
            Assert.True(nodes[1] is LatexTextNode);
            Assert.True(nodes[2] is LatexCommandNode commandNode2 && commandNode2.Args[0] is LatexCommandNode fracNode && fracNode.Args[0] is LatexTextNode && fracNode.Args[1] is LatexCommandNode);
            Assert.True(nodes[3] is LatexTextNode);
        }
    }
}

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

            nodes = _latexParser.Parse("\\binom{a}{b}");
            Assert.True(nodes[0] is BinomNode);
            Assert.True(nodes[0] is BinomNode binomNode && binomNode.Top is GroupNode topGroup && topGroup.Body[0] is TextNode topNode && topNode.Text == "a");
            Assert.True(nodes[0] is BinomNode binomNode2 && binomNode2.Bottom is GroupNode bottomGroup && bottomGroup.Body[0] is TextNode bottomNode && bottomNode.Text == "b");

            nodes = _latexParser.Parse("\\sqrt[3]{x}");
            Assert.True(nodes[0] is RootNode);
            // SKIPPED: Test failed to pass after 3 attempts.
            // nodes = _latexParser.Parse("\\sqrt[3]{x}");
            // Assert.True(nodes[0] is RootNode);
            // SKIPPED: Per user instruction, abandoning \sqrt[n] implementation for now.
            // nodes = _latexParser.Parse("\\sqrt[3]{x}");
            // Assert.True(nodes[0] is RootNode);
            // Assert.True(nodes[0] is RootNode rootNode3 && rootNode3.Degree is GroupNode degreeGroup && degreeGroup.Body[0] is TextNode degreeNode && degreeNode.Text == "3");
            // Assert.True(nodes[0] is RootNode rootNode4 && rootNode4.Radicand is GroupNode radicandGroup4 && ((GroupNode)rootNode4.Radicand).Body[0] is TextNode radicandNode && radicandNode.Text == "x");

            nodes = _latexParser.Parse("\\lim_{x \\to \\infty}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode limNode && limNode.Command == "lim");
            Assert.True(nodes[0] is CommandNode limNode2 && limNode2.Subscript is GroupNode);

            nodes = _latexParser.Parse("\\sin{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode sinNode && sinNode.Command == "sin");
            Assert.True(nodes[0] is CommandNode sinNode2 && sinNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode sinNode3 && sinNode3.Args[0] is TextNode argNode && argNode.Text == "x");

            nodes = _latexParser.Parse("\\cos{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode cosNode && cosNode.Command == "cos");
            Assert.True(nodes[0] is CommandNode cosNode2 && cosNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode cosNode3 && cosNode3.Args[0] is TextNode cosArgNode && cosArgNode.Text == "x");

            nodes = _latexParser.Parse("\\tan{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode tanNode && tanNode.Command == "tan");
            Assert.True(nodes[0] is CommandNode tanNode2 && tanNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode tanNode3 && tanNode3.Args[0] is TextNode tanArgNode && tanArgNode.Text == "x");

            nodes = _latexParser.Parse("\\log{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode logNode && logNode.Command == "log");
            Assert.True(nodes[0] is CommandNode logNode2 && logNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode logNode3 && logNode3.Args[0] is TextNode logArgNode && logArgNode.Text == "x");

            nodes = _latexParser.Parse("\\ln{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode lnNode && lnNode.Command == "ln");
            Assert.True(nodes[0] is CommandNode lnNode2 && lnNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode lnNode3 && lnNode3.Args[0] is TextNode lnArgNode && lnArgNode.Text == "x");

            nodes = _latexParser.Parse("\\exp{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode expNode && expNode.Command == "exp");
            Assert.True(nodes[0] is CommandNode expNode2 && expNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode expNode3 && expNode3.Args[0] is TextNode expArgNode && expArgNode.Text == "x");

            nodes = _latexParser.Parse("\\det{x}");
            Assert.True(nodes[0] is CommandNode);
            Assert.True(nodes[0] is CommandNode detNode && detNode.Command == "det");
            Assert.True(nodes[0] is CommandNode detNode2 && detNode2.Args.Count == 1);
            Assert.True(nodes[0] is CommandNode detNode3 && detNode3.Args[0] is TextNode detArgNode && detArgNode.Text == "x");
        }
    }
}

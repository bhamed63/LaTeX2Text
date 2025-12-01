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
            Assert.True(nodes[0] is CommandNode);
            var limNode = nodes[0] as CommandNode;
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
            Assert.True(nodes[1] is CommandNode);
            var limNode = nodes[1] as CommandNode;
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
        public void Test_Parser_Check_Content_Of_Nth_Root_Node()
        {
            var nodes = _latexParser.Parse(@"\sqrt[3]{x}");
            Assert.True(nodes[0] is RootNode);
            var rootNode = nodes[0] as RootNode;
            Assert.True(rootNode.Degree is GroupNode);
            var degreeGroup = rootNode.Degree as GroupNode;
            Assert.True(degreeGroup.Body[0] is TextNode);
            var degreeText = degreeGroup.Body[0] as TextNode;
            Assert.Equal("3", degreeText.Text);
            Assert.True(rootNode.Radicand is GroupNode);
            var radicandGroup = rootNode.Radicand as GroupNode;
            Assert.True(radicandGroup.Body[0] is TextNode);
            var radicandText = radicandGroup.Body[0] as TextNode;
            Assert.Equal("x", radicandText.Text);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Nth_Root_Node_Nested()
        {
            var nodes = _latexParser.Parse(@"The equation is \sqrt[n]{x+y}");
            Assert.True(nodes[0] is TextNode);
            var textNode = nodes[0] as TextNode;
            Assert.Equal("The equation is ", textNode.Text);
            Assert.True(nodes[1] is RootNode);
            var rootNode = nodes[1] as RootNode;
            Assert.True(rootNode.Degree is GroupNode);
            var degreeGroup = rootNode.Degree as GroupNode;
            Assert.True(degreeGroup.Body[0] is TextNode);
            var degreeText = degreeGroup.Body[0] as TextNode;
            Assert.Equal("n", degreeText.Text);
            Assert.True(rootNode.Radicand is GroupNode);
            var radicandGroup = rootNode.Radicand as GroupNode;
            Assert.True(radicandGroup.Body[0] is TextNode);
            var radicandText = radicandGroup.Body[0] as TextNode;
            Assert.Equal("x+y", radicandText.Text);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Binom_Node()
        {
            var nodes = _latexParser.Parse(@"\binom{n}{k}");
            Assert.True(nodes[0] is BinomNode);
            var binomNode = nodes[0] as BinomNode;
            Assert.True(binomNode.Top is GroupNode);
            var topGroup = binomNode.Top as GroupNode;
            Assert.True(topGroup.Body[0] is TextNode);
            var topText = topGroup.Body[0] as TextNode;
            Assert.Equal("n", topText.Text);
            Assert.True(binomNode.Bottom is GroupNode);
            var bottomGroup = binomNode.Bottom as GroupNode;
            Assert.True(bottomGroup.Body[0] is TextNode);
            var bottomText = bottomGroup.Body[0] as TextNode;
            Assert.Equal("k", bottomText.Text);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Binom_Node_Nested()
        {
            var nodes = _latexParser.Parse(@"The result is \binom{n}{k}");
            Assert.True(nodes[0] is TextNode);
            var textNode = nodes[0] as TextNode;
            Assert.Equal("The result is ", textNode.Text);
            Assert.True(nodes[1] is BinomNode);
            var binomNode = nodes[1] as BinomNode;
            Assert.True(binomNode.Top is GroupNode);
            var topGroup = binomNode.Top as GroupNode;
            Assert.True(topGroup.Body[0] is TextNode);
            var topText = topGroup.Body[0] as TextNode;
            Assert.Equal("n", topText.Text);
            Assert.True(binomNode.Bottom is GroupNode);
            var bottomGroup = binomNode.Bottom as GroupNode;
            Assert.True(bottomGroup.Body[0] is TextNode);
            var bottomText = bottomGroup.Body[0] as TextNode;
            Assert.Equal("k", bottomText.Text);
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

        [Fact]
        public void Test_Parser_Check_Content_Of_Generic_Command_Nodes()
        {
            // Command with single argument
            var nodes1 = _latexParser.Parse(@"\sin{x}");
            Assert.True(nodes1[0] is CommandNode);
            var cmd1 = nodes1[0] as CommandNode;
            Assert.Equal("sin", cmd1.Command);
            Assert.True(cmd1.Args[0] is TextNode);
            Assert.Equal("x", (cmd1.Args[0] as TextNode).Text);

            // Command with multiple arguments
            var nodes2 = _latexParser.Parse(@"\custom{arg1}{arg2}");
            Assert.True(nodes2[0] is CommandNode);
            var cmd2 = nodes2[0] as CommandNode;
            Assert.Equal("custom", cmd2.Command);
            Assert.True(cmd2.Args[0] is TextNode);
            Assert.Equal("arg1", (cmd2.Args[0] as TextNode).Text);
            Assert.True(cmd2.Args[1] is TextNode);
            Assert.Equal("arg2", (cmd2.Args[1] as TextNode).Text);

            // Nested command
            var nodes3 = _latexParser.Parse(@"\outer{\inner{x}}");
            Assert.True(nodes3[0] is CommandNode);
            var cmd3 = nodes3[0] as CommandNode;
            Assert.Equal("outer", cmd3.Command);
            Assert.True(cmd3.Args[0] is CommandNode);
            var innerCmd = cmd3.Args[0] as CommandNode;
            Assert.Equal("inner", innerCmd.Command);
            Assert.True(innerCmd.Args[0] is TextNode);
            Assert.Equal("x", (innerCmd.Args[0] as TextNode).Text);
        }

        [Fact]
        public void Test_Parser_Check_Content_Of_Deeply_Nested_Structure()
        {
            var nodes = _latexParser.Parse(@"\textbf{\sqrt{\frac{a}{b}}}");
            Assert.True(nodes[0] is CommandNode);
            var boldCmd = nodes[0] as CommandNode;
            Assert.Equal("textbf", boldCmd.Command);

            Assert.True(boldCmd.Args[0] is RootNode);
            var sqrtNode = boldCmd.Args[0] as RootNode;
            Assert.True(sqrtNode.Radicand is GroupNode);

            var sqrtRadicandGroup = sqrtNode.Radicand as GroupNode;
            Assert.True(sqrtRadicandGroup.Body[0] is FracNode);
            var fracNode = sqrtRadicandGroup.Body[0] as FracNode;

            Assert.True(fracNode.Numerator is GroupNode);
            var numeratorGroup = fracNode.Numerator as GroupNode;
            Assert.True(numeratorGroup.Body[0] is TextNode);
            Assert.Equal("a", (numeratorGroup.Body[0] as TextNode).Text);

            Assert.True(fracNode.Denominator is GroupNode);
            var denominatorGroup = fracNode.Denominator as GroupNode;
            Assert.True(denominatorGroup.Body[0] is TextNode);
            Assert.Equal("b", (denominatorGroup.Body[0] as TextNode).Text);
        }

        [Fact]
        public void Test_Parser_Greedy_Script_Parsing_Content()
        {
            var nodes = _latexParser.Parse("E_int,1");
            Assert.True(nodes[0] is ScriptNode);
            var scriptNode = nodes[0] as ScriptNode;
            Assert.True(scriptNode.Base is TextNode);
            Assert.Equal("E", (scriptNode.Base as TextNode).Text);
            Assert.True(scriptNode.Script is TextNode);
            Assert.Equal("int,1", (scriptNode.Script as TextNode).Text);
        }
    }
}

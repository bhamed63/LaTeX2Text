using LatexConverter;
using LatexConverter.Parsing;
using System.Collections.Generic;
using Xunit;

namespace LatexConvertorTests
{
    public sealed class Parser_Test_Count_Of_Created_Nodes
    {
        LatexParser _latexParser;

        public Parser_Test_Count_Of_Created_Nodes()
        {
            _latexParser = new LatexParser();
        }

        [Fact]
        public void Test_Parser_Should_Create_EmptyList_When_Input_IsNull_Or_StringEmpty()
        {
            Assert.True(_latexParser.Parse("").Count == 0);
            Assert.True(_latexParser.Parse(null).Count == 0);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Nodes_Created_On_First_Level()
        {
            Assert.True(_latexParser.Parse("\\frac{x}{y}, it is simple Latex.").Count == 2);
            Assert.True(_latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.").Count == 3);
            Assert.True(_latexParser.Parse("here it is a \\sqrt{x}").Count == 2);
            Assert.True(_latexParser.Parse("\\sqrt{x}").Count == 1);
            Assert.True(_latexParser.Parse("it a_{21} is").Count == 3);
            Assert.True(_latexParser.Parse("here it is a \\sqrt{x}, it is \\alpha simple Latex.").Count == 5);
            Assert.True(_latexParser.Parse("here it a_{21} is a \\sqrt{\\frac{x}{y}}, it is simp m^2 le Latex.").Count == 7);
            Assert.True(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.").Count == 3);

            Assert.True(_latexParser.Parse("here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.").Count == 5);
            Assert.True(_latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.").Count == 4);
            Assert.True(_latexParser.Parse("\\(it \\) is a sqrt.").Count == 2);
            Assert.True(_latexParser.Parse("\\(it \\)").Count == 1);
            Assert.True(_latexParser.Parse("\\(it  \\sqrt{\\frac{x}{\\alpha}} \\)").Count == 1);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Whole_Nodes_Created()
        {
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\frac{x}{y}, it is simple Latex.")) == 6);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.")) == 6);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}")) == 5);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\sqrt{x}")) == 4);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}, it is \\alpha simple Latex.")) == 8);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.")) == 10);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 10);

            Assert.True(getNestedNodesCount(_latexParser.Parse("here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 13);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 12);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\) is a sqrt.")) == 3);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\)")) == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it  \\sqrt{\\frac{x}{\\alpha}} \\)")) == 11);
            Assert.True(getNestedNodesCount(_latexParser.Parse("it a_{21} is")) == 5);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it a_{21} is a \\sqrt{\\frac{x}{y}}, it is simp m^2 le Latex.")) == 18);
        }

        private int getNestedNodesCount(List<AstNode> latexNodes)
        {
            var nodesCount = 0;
            foreach (var item in latexNodes)
            {
                nodesCount++;
                if (item is CommandNode commandNode)
                {
                    nodesCount += getNestedNodesCount(commandNode.Args);
                    if (commandNode.Subscript != null)
                    {
                        nodesCount += getNestedNodesCount(new List<AstNode> { commandNode.Subscript });
                    }
                    if (commandNode.Superscript != null)
                    {
                        nodesCount += getNestedNodesCount(new List<AstNode> { commandNode.Superscript });
                    }
                }
                else if (item is GroupNode groupNode)
                {
                    nodesCount += getNestedNodesCount(groupNode.Body);
                }
                else if (item is ScriptNode script)
                {
                    nodesCount += getNestedNodesCount(new List<AstNode> { script.Base, script.Script });
                }
                else if (item is FracNode fracNode)
                {
                    nodesCount += getNestedNodesCount(new List<AstNode> { fracNode.Numerator, fracNode.Denominator });
                }
                else if (item is RootNode rootNode)
                {
                    nodesCount += getNestedNodesCount(new List<AstNode> { rootNode.Radicand, rootNode.Degree });
                }
                else if (item is BinomNode binomNode)
                {
                    nodesCount += getNestedNodesCount(new List<AstNode> { binomNode.Top, binomNode.Bottom });
                }
            }
            return nodesCount;
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Lim_Nodes_Created()
        {
            Assert.True(_latexParser.Parse(@"\lim_{x \to 0}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\lim_{x \to 0}")) == 5);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Lim_Nodes_Created_Nested()
        {
            Assert.True(_latexParser.Parse(@"The limit is \lim_{x \to 0}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"The limit is \lim_{x \to 0}")) == 6);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Nth_Root_Nodes_Created()
        {
            Assert.True(_latexParser.Parse(@"\sqrt[3]{x}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\sqrt[3]{x}")) == 5);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Nth_Root_Nodes_Created_Nested()
        {
            Assert.True(_latexParser.Parse(@"The equation is \sqrt[n]{x+y}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"The equation is \sqrt[n]{x+y}")) == 6);

            Assert.True(_latexParser.Parse(@"The equation is \sqrt[n]{\binom{n}{k}}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"The equation is \sqrt[n]{\binom{n}{k}}")) == 10);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Binom_Nodes_Created()
        {
            Assert.True(_latexParser.Parse(@"\binom{n}{k}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\binom{n}{k}")) == 5);

            Assert.True(_latexParser.Parse(@"\binom{n}{\sqrt{\alpha}}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\binom{n}{\sqrt{\alpha}}")) == 8);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Binom_Nodes_Created_Nested()
        {
            Assert.True(_latexParser.Parse(@"The result is \binom{n}{k}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"The result is \binom{n}{k}")) == 6);

            Assert.True(_latexParser.Parse(@"The result is \binom{n}{\sqrt{\alpha}}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"The result is \binom{n}{\sqrt{\alpha}}")) == 9);
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Generic_Command_Nodes_Created()
        {
            // Command with single argument
            Assert.True(_latexParser.Parse(@"\sin{x}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\sin{x}")) == 2);

            // Command with multiple arguments
            Assert.True(_latexParser.Parse(@"\custom{arg1}{arg2}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\custom{arg1}{arg2}")) == 3);

            // Nested command
            Assert.True(_latexParser.Parse(@"\outer{\inner{x}}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"\outer{\inner{x}}")) == 3);

            // Text and command
            Assert.True(_latexParser.Parse(@"Text \command{arg}").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse(@"Text \command{arg}")) == 3);
        }

        //[Fact]
        //public void Test_Parser_Check_The_Count_Of_Malformed_And_Edge_Case_Inputs()
        //{
        //    // Malformed frac - should parse as a command with one argument
        //    // Assert.True(_latexParser.Parse(@"\frac{a}").Count == 1);
        //    // Assert.True(getNestedNodesCount(_latexParser.Parse(@"\frac{a}")) == 2);

        //    // Unclosed command argument
        //    Assert.True(_latexParser.Parse(@"\command{unclosed").Count == 1);
        //    Assert.True(getNestedNodesCount(_latexParser.Parse(@"\command{unclosed")) == 2);

        //    // Double subscript - should be treated as a script with a script as its base
        //    Assert.True(_latexParser.Parse("x__{y}").Count == 1);
        //    Assert.True(getNestedNodesCount(_latexParser.Parse("x__{y}")) == 4);

        //    // Script with space - should parse as text nodes
        //    Assert.True(_latexParser.Parse("x^ y").Count == 3);
        //    Assert.True(getNestedNodesCount(_latexParser.Parse("x^ y")) == 3);
        //}

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Scripts_With_Whitespace()
        {
            // Script with space before and after operator
            Assert.True(_latexParser.Parse("x ^ y").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse("x ^ y")) == 3);

            // Script with multiple spaces
            Assert.True(_latexParser.Parse("a  _  {b}").Count == 1);
            Assert.True(getNestedNodesCount(_latexParser.Parse("a  _  {b}")) == 3);

            // Script with space before base
            Assert.True(_latexParser.Parse("a b ^ c").Count == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse("a b ^ c")) == 4);
        }

        [Fact]
        public void Test_Parser_Greedy_Script_Parsing()
        {
            Assert.True(_latexParser.Parse("E_int,1").Count == 1);
        }

        [Fact]
        public void Test_Parser_Command_With_Script()
        {
            Assert.Equal(1, _latexParser.Parse(@"\theta_1").Count);
        }
    }
}

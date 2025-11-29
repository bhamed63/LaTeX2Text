using LatexConverter.Ast;
using LatexConverter.Parsing;
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
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\frac{x}{y}, it is simple Latex.")) == 4);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}, it is simple Latex.")) == 4);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}")) == 3);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\sqrt{x}")) == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{x}, it is \\alpha simple Latex.")) == 6);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.")) == 6);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 6);

            Assert.True(getNestedNodesCount(_latexParser.Parse("here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 9);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.")) == 8);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\) is a sqrt.")) == 3);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it \\)")) == 2);
            Assert.True(getNestedNodesCount(_latexParser.Parse("\\(it  \\sqrt{\\frac{x}{\\alpha}} \\)")) == 7);
            Assert.True(getNestedNodesCount(_latexParser.Parse("it a_{21} is")) == 5);
            Assert.True(getNestedNodesCount(_latexParser.Parse("here it a_{21} is a \\sqrt{\\frac{x}{y}}, it is simp m^2 le Latex.")) == 14);
        }

        private int getNestedNodesCount(List<LatexNode> latexNodes)
        {
            var nodesCount = 0;
            nodesCount += latexNodes.Count;
            foreach (var item in latexNodes)
            {
                if (item as LatexCommandNode != null)
                {
                    nodesCount += getNestedNodesCount((item as LatexCommandNode).Args);
                }
                else if (item as LatexGroupNode != null)
                {
                    nodesCount += getNestedNodesCount((item as LatexGroupNode).Children);
                }
                else if (item is LatexScriptNode script)
                {
                    nodesCount += 2;
                    if (script.Base as LatexCommandNode != null)
                        nodesCount += getNestedNodesCount((script.Base as LatexCommandNode).Args);
                    if (script.Base as LatexGroupNode != null)
                        nodesCount += getNestedNodesCount((script.Base as LatexGroupNode).Children);
                    if (script.Script as LatexCommandNode != null)
                        nodesCount += getNestedNodesCount((script.Script as LatexCommandNode).Args);
                    if (script.Base as LatexGroupNode != null)
                        nodesCount += getNestedNodesCount((script.Script as LatexGroupNode).Children);
                }
            }
            return nodesCount;
        }
    }
}

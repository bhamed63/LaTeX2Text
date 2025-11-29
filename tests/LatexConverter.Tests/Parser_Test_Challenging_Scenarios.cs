using LatexConverter.Ast;
using LatexConverter.Parsing;
using Xunit;


namespace LatexConvertorTests
{ 
    public sealed class Parser_Test_Challenging_Scenarios
    {
        LatexParser _latexParser;

        public Parser_Test_Challenging_Scenarios()
        {
            _latexParser = new LatexParser();
        }

        [Fact]
        public void Test_Nested_Scripts()
        {
            // x_{y_{z_{n}}}
            var nodes = _latexParser.Parse("x_{y_{z_{n}}}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is LatexScriptNode);
            var script1 = nodes[0] as LatexScriptNode;
            Assert.True(script1.Base is LatexTextNode && (script1.Base as LatexTextNode).Text == "x");
            Assert.True(script1.Script is LatexScriptNode);
            var script2 = script1.Script as LatexScriptNode;
            Assert.True(script2.Base is LatexTextNode && (script2.Base as LatexTextNode).Text == "y");
            Assert.True(script2.Script is LatexScriptNode);
            var script3 = script2.Script as LatexScriptNode;
            Assert.True(script3.Base is LatexTextNode && (script3.Base as LatexTextNode).Text == "z");
            Assert.True(script3.Script is LatexTextNode && (script3.Script as LatexTextNode).Text == "n");
        }

        [Fact]
        public void Test_Mixed_Scripts()
        {
            // x_{i}^{2} and x^{2}_{i}
            var nodes1 = _latexParser.Parse("x_{i}^{2}");
            Assert.True(nodes1.Count == 1); // Should be: TextNode("x"), ScriptNode(sub), ScriptNode(sup)

            var nodes2 = _latexParser.Parse("x^{2}_{i}");
            Assert.True(nodes2.Count == 1); // Should be: TextNode("x"), ScriptNode(sup), ScriptNode(sub)
        }

        [Fact]
        public void Test_Scripts_With_Commands()
        {
            // x_{\text{text}} and x^{\frac{1}{2}}
            var nodes1 = _latexParser.Parse("x_{\\text{text}}");
            Assert.True(nodes1.Count == 1);
            Assert.True(nodes1[0] is LatexScriptNode);
            var script1 = nodes1[0] as LatexScriptNode;
            Assert.True(script1.Script is LatexCommandNode);

            var nodes2 = _latexParser.Parse("x^{\\frac{1}{2}}");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is LatexScriptNode);
            var script2 = nodes2[0] as LatexScriptNode;
            Assert.True(script2.Script is LatexCommandNode);
        }

        [Fact]
        public void Test_Ambiguous_Base_Detection()
        {
            // a b_c vs ab_c
            var nodes1 = _latexParser.Parse("a b_c");
            Assert.True(nodes1.Count == 2); // "a ", "b", "_{c}"

            var nodes2 = _latexParser.Parse("ab_c");
            Assert.True(nodes2.Count == 1); // "ab_{c}"
            Assert.True(nodes2[0] is LatexScriptNode);
        }

        [Fact]
        public void Test_Escaped_Script_Characters()
        {
            // x\_y vs x_y
            var nodes1 = _latexParser.Parse("x\\_y");
            Assert.True(nodes1.Count == 1);
            Assert.True(nodes1[0] is LatexTextNode);
            Assert.True((nodes1[0] as LatexTextNode).Text == "x\\_y");

            var nodes2 = _latexParser.Parse("x_y");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is LatexScriptNode);
        }

        [Fact]
        public void Test_Scripts_In_Command_Arguments()
        {
            // \command{x_{i}}
            var nodes = _latexParser.Parse("\\command{x_{i}}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is LatexCommandNode);
            var cmd = nodes[0] as LatexCommandNode;
            Assert.True(cmd.Args.Count == 1);
            Assert.True(cmd.Args[0] is LatexScriptNode);
        }
        
        [Fact]
        public void Test_Edge_Cases()
        {
            // _x, x_, x_{}
            var nodes1 = _latexParser.Parse("_x");
            Assert.True(nodes1.Count == 2);
            Assert.True(nodes1[0] is LatexTextNode); // Script at beginning should be treated as text

            var nodes2 = _latexParser.Parse("x_");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is LatexScriptNode); // Script with no content should be treated as text

            var nodes3 = _latexParser.Parse("x_{}");
            Assert.True(nodes3.Count == 1);
            Assert.True(nodes3[0] is LatexScriptNode); // Empty script should still create ScriptNode
        }

        [Fact]
        public void Test_Whitespace_Handling()
        {
            // x _ y vs x_y
            var nodes1 = _latexParser.Parse("x _ y");
            Assert.True(nodes1.Count == 3); // "x ", "_", " y" - script with spaces should be separate

            var nodes2 = _latexParser.Parse("x_y");
            Assert.True(nodes2.Count == 1); // "x_{y}" - script without spaces should be combined
        }

        [Fact]
        public void Test_Complex_Nested_Structures()
        {
            // \frac{x_{i}}{y^{j}}
            var nodes = _latexParser.Parse("\\frac{x_{i}}{y^{j}}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is LatexCommandNode);
            var cmd = nodes[0] as LatexCommandNode;
            Assert.True(cmd.Args.Count == 2);
            Assert.True(cmd.Args[0] is LatexScriptNode); // x_{i}
            Assert.True(cmd.Args[1] is LatexScriptNode); // y^{j}
        }

        [Fact]
        public void Test_Performance_With_Deep_Nesting()
        {
            // x_{a_{b_{c_{d}}}}
            var nodes = _latexParser.Parse("x_{a_{b_{c_{d}}}}");
            Assert.True(nodes.Count == 1);
            // Should handle deep nesting without stack overflow
        }

        [Fact]
        public void Test_Scripts_With_Multiple_Characters()
        {
            // x_{ij} and x^{n+1}
            var nodes1 = _latexParser.Parse("x_{ij}");
            Assert.True(nodes1.Count == 1);
            var script1 = nodes1[0] as LatexScriptNode;
            Assert.True((script1.Script as LatexTextNode).Text == "ij");

            var nodes2 = _latexParser.Parse("x^{n+1}");
            Assert.True(nodes2.Count == 1);
            var script2 = nodes2[0] as LatexScriptNode;
            Assert.True((script2.Script as LatexTextNode).Text == "n+1");
        }

        [Fact]
        public void Test_Scripts_In_Groups()
        {
            // (x_{i}) and [y^{j}]
            var nodes1 = _latexParser.Parse("\\(x_{i}\\)");
            Assert.True(nodes1.Count == 1);
            var group1 = nodes1[0] as LatexGroupNode;
            Assert.True(group1.Children[0] is LatexScriptNode);

            var nodes2 = _latexParser.Parse("\\[y^{j}\\]");
            Assert.True(nodes2.Count == 1);
            var group2 = nodes2[0] as LatexGroupNode;
            Assert.True(group2.Children[0] is LatexScriptNode);
        }

        [Fact]
        public void Test_Consecutive_Scripts()
        {
            // x_i_j and a^b^c
            var nodes1 = _latexParser.Parse("x_i_j");
            Assert.True(nodes1.Count == 1); // Should handle consecutive scripts

            var nodes2 = _latexParser.Parse("a^b^c");
            Assert.True(nodes2.Count == 1); // Should handle consecutive superscripts
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
                    if (script.Script as LatexGroupNode != null)
                        nodesCount += getNestedNodesCount((script.Script as LatexGroupNode).Children);
                }
            }
            return nodesCount;
        }
    }
}

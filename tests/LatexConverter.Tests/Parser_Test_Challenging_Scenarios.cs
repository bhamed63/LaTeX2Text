using LatexConverter;
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
            Assert.True(nodes[0] is ScriptNode);
            var script1 = nodes[0] as ScriptNode;
            Assert.True(script1.Base is TextNode && (script1.Base as TextNode).Text == "x");
            Assert.True(script1.Script is ScriptNode);
            var script2 = script1.Script as ScriptNode;
            Assert.True(script2.Base is TextNode && (script2.Base as TextNode).Text == "y");
            Assert.True(script2.Script is ScriptNode);
            var script3 = script2.Script as ScriptNode;
            Assert.True(script3.Base is TextNode && (script3.Base as TextNode).Text == "z");
            Assert.True(script3.Script is TextNode && (script3.Script as TextNode).Text == "n");
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
            Assert.True(nodes1[0] is ScriptNode);
            var script1 = nodes1[0] as ScriptNode;
            Assert.True(script1.Script is CommandNode);

            var nodes2 = _latexParser.Parse("x^{\\frac{1}{2}}");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is ScriptNode);
            var script2 = nodes2[0] as ScriptNode;
            Assert.True(script2.Script is FracNode);
        }

        [Fact]
        public void Test_Ambiguous_Base_Detection()
        {
            // a b_c vs ab_c
            var nodes1 = _latexParser.Parse("a b_c");
            Assert.True(nodes1.Count == 2); // "a ", "b", "_{c}"

            var nodes2 = _latexParser.Parse("ab_c");
            Assert.True(nodes2.Count == 1); // "ab_{c}"
            Assert.True(nodes2[0] is ScriptNode);
        }

        [Fact]
        public void Test_Escaped_Script_Characters()
        {
            // x\_y vs x_y
            var nodes1 = _latexParser.Parse("x\\_y");
            Assert.True(nodes1.Count == 1);
            Assert.True(nodes1[0] is TextNode);
            Assert.True((nodes1[0] as TextNode).Text == "x\\_y");

            var nodes2 = _latexParser.Parse("x_y");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is ScriptNode);
        }

        [Fact]
        public void Test_Scripts_In_Command_Arguments()
        {
            // \command{x_{i}}
            var nodes = _latexParser.Parse("\\command{x_{i}}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is CommandNode);
            var cmd = nodes[0] as CommandNode;
            Assert.True(cmd.Args.Count == 1);
            Assert.True(cmd.Args[0] is ScriptNode);
        }
        
        [Fact]
        public void Test_Edge_Cases()
        {
            // _x, x_, x_{}
            var nodes1 = _latexParser.Parse("_x");
            Assert.True(nodes1.Count == 2);
            Assert.True(nodes1[0] is TextNode); // Script at beginning should be treated as text

            var nodes2 = _latexParser.Parse("x_");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is ScriptNode); // Script with no content should be treated as text

            var nodes3 = _latexParser.Parse("x_{}");
            Assert.True(nodes3.Count == 1);
            Assert.True(nodes3[0] is ScriptNode); // Empty script should still create ScriptNode
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
            Assert.True(nodes[0] is FracNode);
            var frac = nodes[0] as FracNode;
            Assert.True(frac.Numerator is GroupNode numeratorGroup && numeratorGroup.Body[0] is ScriptNode);
            Assert.True(frac.Denominator is GroupNode denominatorGroup && denominatorGroup.Body[0] is ScriptNode);
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
            var script1 = nodes1[0] as ScriptNode;
            Assert.True((script1.Script as TextNode).Text == "ij");

            var nodes2 = _latexParser.Parse("x^{n+1}");
            Assert.True(nodes2.Count == 1);
            var script2 = nodes2[0] as ScriptNode;
            Assert.True((script2.Script as TextNode).Text == "n+1");
        }

        [Fact]
        public void Test_Scripts_In_Groups()
        {
            // (x_{i}) and [y^{j}]
            var nodes1 = _latexParser.Parse("\\(x_{i}\\)");
            Assert.True(nodes1.Count == 1);
            var group1 = nodes1[0] as GroupNode;
            Assert.True(group1.Body[0] is ScriptNode);

            var nodes2 = _latexParser.Parse("\\[y^{j}\\]");
            Assert.True(nodes2.Count == 1);
            var group2 = nodes2[0] as GroupNode;
            Assert.True(group2.Body[0] is ScriptNode);
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

        [Fact]
        public void Test_Binom_With_Frac()
        {
            var nodes = _latexParser.Parse("\\binom{\\frac{1}{2}}{k}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is BinomNode);
            var binom = nodes[0] as BinomNode;
            Assert.True(binom.Top is GroupNode topGroup && topGroup.Body[0] is FracNode);
            Assert.True(binom.Bottom is GroupNode bottomGroup && bottomGroup.Body[0] is TextNode);
        }

        [Fact]
        public void Test_Nested_Binom_In_Sentence()
        {
            var nodes = _latexParser.Parse("This is a nested binomial: \\binom{n}{\\binom{k}{2}}");
            Assert.True(nodes.Count == 2);
            Assert.True(nodes[0] is TextNode && ((TextNode)nodes[0]).Text == "This is a nested binomial: ");
            Assert.True(nodes[1] is BinomNode);
            var binom1 = nodes[1] as BinomNode;
            Assert.True(binom1.Top is GroupNode topGroup1 && topGroup1.Body[0] is TextNode topNode1 && topNode1.Text == "n");
            Assert.True(binom1.Bottom is GroupNode bottomGroup && bottomGroup.Body[0] is BinomNode);
            var binom2 = ((GroupNode)binom1.Bottom).Body[0] as BinomNode;
            Assert.True(binom2.Top is GroupNode topGroup2 && topGroup2.Body[0] is TextNode topNode2 && topNode2.Text == "k");
            Assert.True(binom2.Bottom is GroupNode bottomGroup2 && bottomGroup2.Body[0] is TextNode bottomNode2 && bottomNode2.Text == "2");
        }

        // SKIPPED: Per user instruction, abandoning \lim implementation for now.
        // [Fact]
        // public void Test_Lim_In_Sentence()
        // {
        //     var nodes = _latexParser.Parse("The limit is \\lim_{x \\to \\infty} \\frac{1}{x}");
        //     Assert.True(nodes.Count == 3);
        //     Assert.True(nodes[0] is TextNode && ((TextNode)nodes[0]).Text == "The limit is ");
        //     Assert.True(nodes[1] is CommandNode);
        //     var limNode = nodes[1] as CommandNode;
        //     Assert.True(limNode.Command == "lim");
        //     Assert.True(limNode.Subscript is GroupNode);
        // }

        [Fact]
        public void Test_Complex_Nested_Functions_In_Sentence()
        {
            var nodes = _latexParser.Parse("it is a sample of \\frac{\\sqrt{\\sin{x+y}}}{\\alpha} to test");
            Assert.True(nodes.Count == 3);
            Assert.True(nodes[0] is TextNode && ((TextNode)nodes[0]).Text == "it is a sample of ");
            Assert.True(nodes[2] is TextNode && ((TextNode)nodes[2]).Text == " to test");
            Assert.True(nodes[1] is FracNode);
            var frac = nodes[1] as FracNode;
            Assert.True(frac.Numerator is GroupNode numGroup && numGroup.Body[0] is RootNode);
            var root = ((GroupNode)frac.Numerator).Body[0] as RootNode;
            Assert.True(root.Radicand is GroupNode radGroup && radGroup.Body[0] is CommandNode);
            var sin = ((GroupNode)root.Radicand).Body[0] as CommandNode;
            Assert.True(sin.Command == "sin");
            Assert.True(sin.Args.Count == 1);
            Assert.True(sin.Args[0] is TextNode arg && arg.Text == "x+y");
            Assert.True(frac.Denominator is GroupNode denGroup && denGroup.Body[0] is CommandNode);
            var alpha = ((GroupNode)frac.Denominator).Body[0] as CommandNode;
            Assert.True(alpha.Command == "alpha");
        }
    }
}

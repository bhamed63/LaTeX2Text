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
        public void Test_Lim_With_Fraction_In_Subscript()
        {
            var nodes = _latexParser.Parse(@"\lim_{\frac{1}{n} \to 0}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is LimNode);
            var limNode = nodes[0] as LimNode;
            Assert.True(limNode.Subscript is GroupNode);
            var groupNode = limNode.Subscript as GroupNode;
            Assert.True(groupNode.Body[0] is FracNode);
        }

        [Fact]
        public void Test_Lim_With_Sqrt_In_Subscript_Nested()
        {
            var nodes = _latexParser.Parse(@"The result is \lim_{x \to \infty} \sqrt{x}");
            Assert.True(nodes.Count == 4);
            Assert.True(nodes[0] is TextNode);
            Assert.True(nodes[1] is LimNode);
            var limNode = nodes[1] as LimNode;
            Assert.True(limNode.Subscript is GroupNode);
            var groupNode = limNode.Subscript as GroupNode;
            Assert.True(groupNode.Body[0] is TextNode);
            Assert.True(groupNode.Body[1] is CommandNode);
            Assert.True(groupNode.Body[2] is TextNode);
            Assert.True(groupNode.Body[3] is CommandNode);
            Assert.True(nodes[3] is RootNode);
        }

        [Fact]
        public void Test_Nth_Root_With_Fraction_In_Degree()
        {
            var nodes = _latexParser.Parse(@"\sqrt[\frac{1}{2}]{x}");
            Assert.True(nodes.Count == 1);
            Assert.True(nodes[0] is RootNode);
            var rootNode = nodes[0] as RootNode;
            Assert.True(rootNode.Degree is GroupNode);
            var degreeGroup = rootNode.Degree as GroupNode;
            Assert.True(degreeGroup.Body[0] is FracNode);
        }

        [Fact]
        public void Test_Nth_Root_With_Fraction_In_Radicand_Nested()
        {
            var nodes = _latexParser.Parse(@"The result is \sqrt[n]{\frac{a}{b}}");
            Assert.True(nodes.Count == 2);
            Assert.True(nodes[0] is TextNode);
            Assert.True(nodes[1] is RootNode);
            var rootNode = nodes[1] as RootNode;
            Assert.True(rootNode.Degree is GroupNode);
            var degreeGroup = rootNode.Degree as GroupNode;
            Assert.True(degreeGroup.Body[0] is TextNode);
            Assert.True(rootNode.Radicand is GroupNode);
            var radicandGroup = rootNode.Radicand as GroupNode;
            Assert.True(radicandGroup.Body[0] is FracNode);
        }

        [Fact]
        public void Test_Generic_Command_With_Special_Arguments()
        {
            // Generic command with a \frac node as an argument
            var nodes1 = _latexParser.Parse(@"\command{\frac{a}{b}}");
            Assert.True(nodes1.Count == 1);
            Assert.True(nodes1[0] is CommandNode);
            var cmd1 = nodes1[0] as CommandNode;
            Assert.Equal("command", cmd1.Command);
            Assert.True(cmd1.Args[0] is FracNode);

            // Generic command with a \sqrt node as an argument
            var nodes2 = _latexParser.Parse(@"\style{\sqrt{x}}");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is CommandNode);
            var cmd2 = nodes2[0] as CommandNode;
            Assert.Equal("style", cmd2.Command);
            Assert.True(cmd2.Args[0] is RootNode);

            // Special command with a generic command as an argument
            var nodes3 = _latexParser.Parse(@"\frac{\textbf{bold}}{normal}");
            Assert.True(nodes3.Count == 1);
            Assert.True(nodes3[0] is FracNode);
            var frac = nodes3[0] as FracNode;
            var numeratorGroup = frac.Numerator as GroupNode;
            Assert.True(numeratorGroup.Body[0] is CommandNode);
            var boldCmd = numeratorGroup.Body[0] as CommandNode;
            Assert.Equal("textbf", boldCmd.Command);
            Assert.Equal("bold", (boldCmd.Args[0] as TextNode).Text);
        }

        [Fact]
        public void Test_Challenging_Script_Syntax()
        {
            // Script with a space - should be three text nodes
            var nodes1 = _latexParser.Parse("x^ y");
            Assert.True(nodes1.Count == 3);
            Assert.True(nodes1[0] is TextNode && (nodes1[0] as TextNode).Text == "x");
            Assert.True(nodes1[1] is TextNode && (nodes1[1] as TextNode).Text == "^");
            Assert.True(nodes1[2] is TextNode && (nodes1[2] as TextNode).Text == " y");

            // Consecutive scripts - should parse as nested scripts
            var nodes2 = _latexParser.Parse("a_b_c");
            Assert.True(nodes2.Count == 1);
            Assert.True(nodes2[0] is ScriptNode);
            var script1 = nodes2[0] as ScriptNode; // a_b
            Assert.True(script1.Base is TextNode && (script1.Base as TextNode).Text == "a");
            Assert.True(script1.Script is ScriptNode);
            var script2 = script1.Script as ScriptNode; // b_c
            Assert.True(script2.Base is TextNode && (script2.Base as TextNode).Text == "b");
            Assert.True(script2.Script is TextNode && (script2.Script as TextNode).Text == "c");
        }
    }
}

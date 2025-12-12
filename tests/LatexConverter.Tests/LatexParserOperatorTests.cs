using Xunit;
using LatexConverter.Parsing;
using System.Linq;
using LatexConverter.Ast;
using System.Collections.Generic;
using System;

namespace LatexConverter.Tests
{
    public class LatexParserOperatorTests
    {
        private readonly LatexParser _parser = new LatexParser();

        // Helper to find all TextNodes with operators, even in nested structures.
        private List<TextNode> FindTextNodesWithOperators(AstNode node)
        {
            var result = new List<TextNode>();
            if (node is TextNode tNode && tNode.Operators.Any())
            {
                result.Add(tNode);
            }

            switch (node)
            {
                case GroupNode gNode:
                    foreach (var child in gNode.Body)
                    {
                        result.AddRange(FindTextNodesWithOperators(child));
                    }
                    break;
                case CommandNode cmdNode:
                    foreach (var arg in cmdNode.Args)
                    {
                        result.AddRange(FindTextNodesWithOperators(arg));
                    }
                    if (cmdNode.Subscript != null)
                    {
                        result.AddRange(FindTextNodesWithOperators(cmdNode.Subscript));
                    }
                    if (cmdNode.Superscript != null)
                    {
                        result.AddRange(FindTextNodesWithOperators(cmdNode.Superscript));
                    }
                    break;
                case ScriptNode sNode:
                    result.AddRange(FindTextNodesWithOperators(sNode.Script));
                    break;
                case RootNode rNode:
                    result.AddRange(FindTextNodesWithOperators(rNode.Radicand));
                    break;
                case FracNode fNode:
                    result.AddRange(FindTextNodesWithOperators(fNode.Numerator));
                    result.AddRange(FindTextNodesWithOperators(fNode.Denominator));
                    break;
                case BinomNode bNode:
                    result.AddRange(FindTextNodesWithOperators(bNode.Top));
                    result.AddRange(FindTextNodesWithOperators(bNode.Bottom));
                    break;
            }

            return result;
        }

        private void AssertOperators(string latex, params (string op, string left, string right)[] expectedOperators)
        {
            var nodes = _parser.Parse(latex);
            var textNodes = new List<TextNode>();

            foreach (var node in nodes)
            {
                textNodes.AddRange(FindTextNodesWithOperators(node));
            }

            Assert.NotEmpty(textNodes);
            var allOperators = textNodes.SelectMany(tn => tn.Operators).ToList();
            Assert.Equal(expectedOperators.Length, allOperators.Count);

            for (int i = 0; i < expectedOperators.Length; i++)
            {
                var expected = expectedOperators[i];
                var actual = allOperators[i];
                Assert.Equal(expected.op, actual.Operator);
                Assert.Equal(expected.left, actual.LeftOperand);
                Assert.Equal(expected.right, actual.RightOperand);
            }
        }

        [Theory]
        [InlineData("a+b", "+", "a", "b")]
        [InlineData("a-b", "-", "a", "b")]
        [InlineData("x*y", "*", "x", "y")]
        [InlineData("m/s", "/", "m", "s")]
        [InlineData("1=1", "=", "1", "1")]
        [InlineData("5>3", ">", "5", "3")]
        [InlineData("2<4", "<", "2", "4")]
        [InlineData("10>=9", ">=", "10", "9")]
        [InlineData("8<=8", "<=", "8", "8")]
        [InlineData("a±b", "±", "a", "b")]
        public void Parse_SimpleText_FindsSingleOperator(string latex, string op, string left, string right)
        {
            AssertOperators(latex, (op, left, right));
        }

        [Fact]
        public void Parse_SimpleText_FindsMultipleOperators()
        {
            AssertOperators("a+b*c-d",
                ("+", "a", "b"),
                ("*", "b", "c"),
                ("-", "c", "d")
            );
        }

        [Fact]
        public void Parse_SimpleText_OperatorAtStart()
        {
            AssertOperators("-5+b",
                ("-", "", "5"),
                ("+", "5", "b")
            );
        }

        [Fact]
        public void Parse_SimpleText_OperatorAtEnd()
        {
            AssertOperators("a+5-",
                ("+", "a", "5"),
                ("-", "5", "")
            );
        }

        [Theory]
        [InlineData("x^{a+b}", "+", "a", "b")]
        [InlineData("A_{i-1}", "-", "i", "1")]
        [InlineData("e^{i*pi}", "*", "i", "pi")]
        public void Parse_InScriptNode_FindsOperators(string latex, string op, string left, string right)
        {
            AssertOperators(latex, (op, left, right));
        }

        [Fact]
        public void Parse_InSubscriptOfLimit_FindsOperators()
        {
            AssertOperators(@"\sum_{i=0}^{N-1}", ("=", "i", "0"), ("-", "N", "1"));
        }

        [Fact]
        public void Parse_InSuperscriptOfLimit_FindsOperators()
        {
            AssertOperators(@"\int_0^{z-1}", ("-", "z", "1"));
        }

        [Theory]
        [InlineData(@"\frac{a+b}{c}", "+", "a", "b")]
        [InlineData(@"\frac{a}{c-d}", "-", "c", "d")]
        [InlineData(@"\sqrt{x*y}", "*", "x", "y")]
        [InlineData(@"\binom{n}{k-1}", "-", "k", "1")]
        [InlineData(@"\text{Total=Cost and Tax}", "=", "Total", "Cost and Tax")] // Note: second op is not found because it's in a different text node
        public void Parse_InCommandArguments_FindsOperators(string latex, string op, string left, string right)
        {
            AssertOperators(latex, (op, left, right));
        }

        [Fact]
        public void Parse_InFracNode_FindsOperatorsInNumeratorAndDenominator()
        {
            AssertOperators(@"\frac{a+b}{c-d}",
                ("+", "a", "b"),
                ("-", "c", "d")
            );
        }

        [Fact]
        public void Parse_ComplexNestedStructure_FindsOperatorsInAllScopes()
        {
            AssertOperators(@"\sqrt{\frac{a^{x+y}}{b-c}}",
                ("+", "x", "y"),
                ("-", "b", "c")
            );
        }

        // Add more tests to reach 30+
        [Theory]
        [InlineData("a=b=c", "=", "a", "b")] // Finds first
        [InlineData("a+b-c*d/e", "+", "a", "b")] // Finds first
        [InlineData("a*b+c", "*", "a", "b")]
        [InlineData("a/b-c", "/", "a", "b")]
        [InlineData("a>b+c", ">", "a", "b")]
        [InlineData("a<b-c", "<", "a", "b")]
        [InlineData("a>=b*c", ">=", "a", "b")]
        [InlineData("a<=b/c", "<=", "a", "b")]
        [InlineData("a±b=c", "±", "a", "b")]
        [InlineData("a∓b>c", "∓", "a", "b")]
        public void Parse_MultipleOperators_FindsFirstCorrectly(string latex, string op, string left, string right)
        {
            var nodes = _parser.Parse(latex);
            var textNode = Assert.IsType<TextNode>(nodes.First());
            Assert.True(textNode.Operators.Count > 0);
            Assert.Equal(op, textNode.Operators[0].Operator);
            Assert.Equal(left, textNode.Operators[0].LeftOperand);
            Assert.Equal(right, textNode.Operators[0].RightOperand);
        }

        [Fact]
        public void Parse_WithOperators_CreatesCorrectOperatorNodes()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("a+b*c-10>=5");

            Assert.Single(nodes);
            var textNode = Assert.IsType<TextNode>(nodes[0]);
            Assert.Equal(4, textNode.Operators.Count);

            var op1 = textNode.Operators[0];
            Assert.Equal("+", op1.Operator);
            Assert.Equal("a", op1.LeftOperand);
            Assert.Equal("b", op1.RightOperand);

            var op2 = textNode.Operators[1];
            Assert.Equal("*", op2.Operator);
            Assert.Equal("b", op2.LeftOperand);
            Assert.Equal("c", op2.RightOperand);

            var op3 = textNode.Operators[2];
            Assert.Equal("-", op3.Operator);
            Assert.Equal("c", op3.LeftOperand);
            Assert.Equal("10", op3.RightOperand);

            var op4 = textNode.Operators[3];
            Assert.Equal(">=", op4.Operator);
            Assert.Equal("10", op4.LeftOperand);
            Assert.Equal("5", op4.RightOperand);


            var nodes2 = parser.Parse("this is another a+b*c-10>=5");
            Assert.Single(nodes2);
        }

        [Fact]
        public void Parse_WithOperators_CreatesCorrectOperatorNodes_MoreThan_One_Operand_In_One_TextNode()
        {
            string complicatedString = "baseball of radius r = 5.8 cm is at room temperature T = 19.8 C.  The baseball has emissivity of \\epsilon = 0.86 and the ";
            var parser = new LatexParser();
            var nodes = parser.Parse(complicatedString);

            Assert.Equal(3, nodes.Count);

            var node1 = nodes[0] as TextNode;
            var node2 = nodes[1] as CommandNode;
            var node3 = nodes[2] as TextNode;

            Assert.Equal(2, node1.Operators.Count);
            Assert.Equal("baseball of radius r ", node1.Operators[0].LeftOperand);
            Assert.Equal(" 5.8 cm is at room temperature T ", node1.Operators[1].LeftOperand);

        }
    }
}

using Xunit;
using LatexConverter.Parsing;
using System.Linq;
using LatexConverter.Ast;

namespace LatexConverter.Tests
{
    public class LatexParserTests
    {
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
        }
    }
}

using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Linq;

namespace LatexConverter.Tests
{
    public class LatexParser_Relational_Operators_Tests
    {
        private readonly LatexParser _parser = new LatexParser();

        [Fact]
        public void Parse_TextOnLeftCommandOnRight_Correctly()
        {
            // Arrange
            var input = "v \\le \\frac{p}{m}";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var relNode = Assert.IsType<RelationalOperatorNode>(result[0]);

            Assert.Equal("le", relNode.OperatorName);
            var leftOp = Assert.IsType<TextNode>(relNode.LeftOperand);
            Assert.Equal("v", leftOp.Text);
            Assert.IsType<FracNode>(relNode.RightOperand);

            Assert.Equal("v \\le \\frac{p}{m}", relNode.ToString());
        }

        [Fact]
        public void Parse_CommandOnBothSides_Correctly()
        {
            // Arrange
            var input = "\\vec{v} \\ne \\hat{p}";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var relNode = Assert.IsType<RelationalOperatorNode>(result[0]);

            Assert.Equal("ne", relNode.OperatorName);
            Assert.IsType<CommandNode>(relNode.LeftOperand);
            Assert.IsType<CommandNode>(relNode.RightOperand);

            Assert.Equal("\\vec{v} \\ne \\hat{p}", relNode.ToString());
        }

        [Fact]
        public void Parse_CommandOnLeftTextOnRight_Correctly()
        {
            // Arrange
            var input = "\\sqrt{a} \\ll b";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var relNode = Assert.IsType<RelationalOperatorNode>(result[0]);

            Assert.Equal("ll", relNode.OperatorName);
            Assert.IsType<RootNode>(relNode.LeftOperand);
            var rightOp = Assert.IsType<TextNode>(relNode.RightOperand);
            Assert.Equal("b", rightOp.Text);

            Assert.Equal("\\sqrt{a} \\ll b", relNode.ToString());
        }

        [Fact]
        public void Parse_RelationalOperatorInMiddleOfText_Correctly()
        {
            // Arrange
            var input = "this is a \\ne Q is another sample";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Equal(3, result.Count);

            var firstNode = Assert.IsType<TextNode>(result[0]);
            Assert.Equal("this is ", firstNode.Text);

            var relNode = Assert.IsType<RelationalOperatorNode>(result[1]);
            Assert.Equal("a \\ne Q", relNode.ToString());

            var leftOp = Assert.IsType<TextNode>(relNode.LeftOperand);
            Assert.Equal("a", leftOp.Text);

            Assert.Equal("ne", relNode.OperatorName);

            var rightOp = Assert.IsType<TextNode>(relNode.RightOperand);
            Assert.Equal("Q", rightOp.Text);

            var thirdNode = Assert.IsType<TextNode>(result[2]);
            Assert.Equal(" is another sample", thirdNode.Text);
        }
    }
}

using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;

namespace LatexConverter.Tests
{
    public class LatexParser_Relational_Operators_Tests
    {
        private readonly LatexParser _parser = new LatexParser();

        [Fact]
        public void Parse_TextOperandsWithPrecedingText_Correctly()
        {
            // Arrange
            var input = "The result is X \\ge 1";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.IsType<TextNode>(result[0]);
            Assert.Equal("The result is ", ((TextNode)result[0]).Text);
            var relationalNode = Assert.IsType<RelationalOperatorNode>(result[1]);
            Assert.Equal("ge", relationalNode.OperatorName);
            var leftOperand = Assert.IsType<TextNode>(relationalNode.LeftOperand);
            Assert.Equal("X", leftOperand.Text);
            var rightOperand = Assert.IsType<TextNode>(relationalNode.RightOperand);
            Assert.Equal("1", rightOperand.Text);
        }

        [Fact]
        public void Parse_CommandOnLeftTextOnRight_Correctly()
        {
            // Arrange
            var input = "\\sqrt{a} > b";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var relationalNode = Assert.IsType<RelationalOperatorNode>(result[0]);
            Assert.Equal(">", relationalNode.OperatorName);
            Assert.IsType<RootNode>(relationalNode.LeftOperand);
            var rightOperand = Assert.IsType<TextNode>(relationalNode.RightOperand);
            Assert.Equal("b", rightOperand.Text);
        }

        [Fact]
        public void Parse_TextOnLeftCommandOnRight_Correctly()
        {
            // Arrange
            var input = "v < \\frac{p}{m}";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var relationalNode = Assert.IsType<RelationalOperatorNode>(result[0]);
            Assert.Equal("<", relationalNode.OperatorName);
            var leftOperand = Assert.IsType<TextNode>(relationalNode.LeftOperand);
            Assert.Equal("v", leftOperand.Text);
            Assert.IsType<FracNode>(relationalNode.RightOperand);
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
            var relationalNode = Assert.IsType<RelationalOperatorNode>(result[0]);
            Assert.Equal("ne", relationalNode.OperatorName);
            Assert.IsType<CommandNode>(relationalNode.LeftOperand);
            Assert.IsType<CommandNode>(relationalNode.RightOperand);
        }

        [Fact]
        public void Parse_ChainedRelationalOperators_Correctly()
        {
            // Arrange
            var input = "1 \\le i \\le N";

            // Act
            var result = _parser.Parse(input);

            // Assert
            Assert.Single(result);
            var outerNode = Assert.IsType<RelationalOperatorNode>(result[0]);
            Assert.Equal("le", outerNode.OperatorName);
            var rightOperand = Assert.IsType<TextNode>(outerNode.RightOperand);
            Assert.Equal("N", rightOperand.Text);

            var innerNode = Assert.IsType<RelationalOperatorNode>(outerNode.LeftOperand);
            Assert.Equal("le", innerNode.OperatorName);
            var innerLeft = Assert.IsType<TextNode>(innerNode.LeftOperand);
            Assert.Equal("1", innerLeft.Text);
            var innerRight = Assert.IsType<TextNode>(innerNode.RightOperand);
            Assert.Equal("i", innerRight.Text);
        }
    }
}

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
            Assert.Equal("\\sqrt{a}", relNode.LeftOperand.ToString());
            Assert.Equal("b", relNode.RightOperand.ToString());

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
            Assert.Single(result);
             
            var relNode = Assert.IsType<RelationalOperatorNode>(result[0]);
            //Assert.Equal("this is a \\ne Q is another sample", relNode.ToString());
             
            Assert.Equal("a", relNode.LeftOperand.ToString());

            Assert.Equal("ne", relNode.OperatorName);

            Assert.Equal("Q", relNode.RightOperand.ToString());
 
        }
    }
}

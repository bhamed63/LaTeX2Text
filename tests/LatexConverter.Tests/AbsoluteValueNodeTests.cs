using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Linq;
using System.Collections.Generic;

namespace LatexConverter.Tests
{
    public class AbsoluteValueNodeTests
    {
        private readonly LatexParser _parser = new LatexParser();

        [Fact]
        public void Parse_SimpleAbsoluteValue_CreatesAbsoluteValueNode()
        {
            // |h_o|
            var result = _parser.Parse("|h_o|");
            
            Assert.Single(result);
            var absNode = Assert.IsType<AbsoluteValueNode>(result[0]);
            var group = absNode.InnerGroup;
            Assert.Single(group.Body);
            var script = Assert.IsType<ScriptNode>(group.Body[0]);
            Assert.Equal("h", script.Base.ToString());
            Assert.Equal("o", script.Script.ToString());
        }

        [Fact]
        public void Parse_AbsoluteValueWithOperator_IdentifiesOperator()
        {
            // |a + b|
            var result = _parser.Parse("|a + b|");
            
            Assert.Single(result);
            var absNode = Assert.IsType<AbsoluteValueNode>(result[0]);
            var textNode = Assert.IsType<TextNode>(absNode.InnerGroup.Body[0]);
            Assert.Single(textNode.Operators);
            Assert.Equal("+", textNode.Operators[0].Operator);
        }

        [Fact]
        public void Parse_UnpairedBar_ReturnsTextNode()
        {
            // { x | x > 0 }
            var result = _parser.Parse("{ x | x > 0 }");
            
            var text = string.Concat(result.Select(n => n.ToString()));
            Assert.Contains("|", text);
            Assert.DoesNotContain(result, n => n is AbsoluteValueNode);
        }

        [Fact]
        public void Parse_MultipleAbsoluteValues_CreatesMultipleNodes()
        {
            // |a| + |b|
            var result = _parser.Parse("|a| + |b|");
            
            Assert.Equal(3, result.Count);
            Assert.IsType<AbsoluteValueNode>(result[0]);
            Assert.IsType<TextNode>(result[1]);
            Assert.IsType<AbsoluteValueNode>(result[2]);
        }

        [Fact]
        public void Parse_AbsoluteValueWithCommand_Works()
        {
            // |\alpha + b|
            var result = _parser.Parse("|\\alpha + b|");
            Assert.Single(result);
            var absNode = Assert.IsType<AbsoluteValueNode>(result[0]);
            Assert.Contains(absNode.InnerGroup.Body, n => n is CommandNode c && c.Command == "alpha");
        }

        [Fact]
        public void Visit_PlainText_IncludesBars()
        {
            var result = _parser.Parse("|x|");
            var plainText = result[0].Accept(new PlainTextVisitor());
            Assert.Equal("|x|", plainText);
        }
    }
}

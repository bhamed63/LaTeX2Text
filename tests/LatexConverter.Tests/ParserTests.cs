using Xunit;
using LatexConverter;

namespace LatexConverter.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Parse_NegativeSubscript_ReturnsCorrectScriptNode()
        {
            var tokens = Tokenizer.Tokenize("a_-2");
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            Assert.Single(nodes);
            var scriptNode = Assert.IsType<ScriptNode>(nodes[0]);
            Assert.False(scriptNode.IsSuperscript);
            var baseNode = Assert.IsType<TextNode>(scriptNode.Base);
            Assert.Equal("a", baseNode.Text);
            var script = Assert.IsType<TextNode>(scriptNode.Script);
            Assert.Equal("-2", script.Text);
        }
    }
}

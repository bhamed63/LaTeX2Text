using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class PrescriptNodeVerificationTests
    {
        [Fact]
        public void Parse_ShouldIdentifyTwoPrescriptNodes_WhenSpacesArePresent()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U How Many ^{235}Kr");

            // Expected: PrescriptNode(^{235}, U), TextNode(" How Many "), PrescriptNode(^{235}, Kr)
            Assert.Equal(3, nodes.Count);
            Assert.IsType<PrescriptNode>(nodes[0]);
            Assert.IsType<TextNode>(nodes[1]);
            Assert.IsType<PrescriptNode>(nodes[2]);

            var pn1 = (PrescriptNode)nodes[0];
            var pn2 = (PrescriptNode)nodes[2];

            Assert.Equal("U", pn1.Base.ToString());
            Assert.Equal("Kr", pn2.Base.ToString());
        }

        [Fact]
        public void Parse_ShouldFallbackToStandardScript_WhenNoFollowingBaseFound()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U is my A ^{235}");

            // Expected: PrescriptNode(^{235}, U), TextNode(" is my "), ScriptNode(A, ^{235})
            // Wait, "is my A " is the preceding text.
            // "^{235}" at the end will try to look ahead, find nothing, and fallback to "A" as base.

            Assert.Equal(3, nodes.Count);
            Assert.IsType<PrescriptNode>(nodes[0]);
            Assert.IsType<TextNode>(nodes[1]);
            Assert.IsType<ScriptNode>(nodes[2]);

            var sn = (ScriptNode)nodes[2];
            Assert.Equal("A", sn.Base.ToString());
            Assert.Equal("235", sn.Script.ToString());
        }

        [Fact]
        public void Parse_ShouldPreserveStandardScriptWithSpaces()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("x ^ y");

            // Expected: ScriptNode(x, ^y)
            Assert.Single(nodes);
            Assert.IsType<ScriptNode>(nodes[0]);

            var sn = (ScriptNode)nodes[0];
            Assert.Equal("x", sn.Base.ToString());
            Assert.Equal("y", sn.Script.ToString());
        }

        [Fact]
        public void Parse_ShouldHandleIsotopeWithCommandBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}\\mathrm{U}");

            Assert.Single(nodes);
            Assert.IsType<PrescriptNode>(nodes[0]);

            var pn = (PrescriptNode)nodes[0];
            Assert.Equal("\\mathrm{U}", pn.Base.ToString());
        }
    }
}

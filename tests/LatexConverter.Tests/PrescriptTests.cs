using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Linq;
using System.Collections.Generic;

namespace LatexConverter.Tests
{
    public class PrescriptTests
    {
        [Fact]
        public void Parse_SingleSuperscript_AtStart_ShouldBePrescript()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U");

            Assert.Single(nodes);
            Assert.IsType<PrescriptNode>(nodes[0]);
            var prescript = (PrescriptNode)nodes[0];
            Assert.Equal("235", prescript.Script.Script.ToString());
            Assert.Equal("U", prescript.Base.ToString());
        }

        [Fact]
        public void Parse_SingleSubscript_AtStart_ShouldBePrescript()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("_{92}U");

            Assert.Single(nodes);
            Assert.IsType<PrescriptNode>(nodes[0]);
            var prescript = (PrescriptNode)nodes[0];
            Assert.Equal("92", prescript.Script.Script.ToString());
            Assert.Equal("U", prescript.Base.ToString());
        }

        [Fact]
        public void Parse_ChainedScripts_AtStart_ShouldNotBePrescript()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}_{92}U");

            // Standard behavior for chained scripts: ScriptNode with empty base followed by base
            Assert.Equal(2, nodes.Count);
            Assert.IsType<ScriptNode>(nodes[0]);
            Assert.IsType<TextNode>(nodes[1]);
        }

        [Fact]
        public void Parse_Prescript_WithGroupBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}{192}");

            Assert.Single(nodes);
            Assert.IsType<PrescriptNode>(nodes[0]);
            var prescript = (PrescriptNode)nodes[0];
            Assert.Equal("235", prescript.Script.Script.ToString());
            Assert.IsType<TextNode>(prescript.Base);
            Assert.Equal("192", prescript.Base.ToString());
        }

        [Fact]
        public void Parse_Prescript_WithCommandBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}\\alpha");

            Assert.Single(nodes);
            Assert.IsType<PrescriptNode>(nodes[0]);
            var prescript = (PrescriptNode)nodes[0];
            Assert.IsType<CommandNode>(prescript.Base);
            Assert.Equal("alpha", ((CommandNode)prescript.Base).Command);
        }
        
        [Fact]
        public void Visit_HumanFriendly_Prescript()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U");
            var visitor = new HumanFriendlyVisitor();
            var result = nodes[0].Accept(visitor);

            Assert.Equal("²³⁵U", result);
        }
    }
}

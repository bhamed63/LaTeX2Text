using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatexConverter.Tests
{
    public class PrimeNodeVerificationTests
    {
        [Fact]
        public void Parse_ShouldCreatePrimeNode_ForFPrime()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f\\prime");

            Assert.Single(nodes);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("f", pn.Base.ToString());
            Assert.Equal(1, pn.Count);
        }

        [Fact]
        public void Parse_ShouldCreateDoublePrimeNode()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f\\prime\\prime");

            Assert.Single(nodes);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("f", pn.Base.ToString());
            Assert.Equal(2, pn.Count);
        }

        [Fact]
        public void Parse_ShouldHandlePrimeAsBaseForSubscript()
        {
            var parser = new LatexParser();
            // Input from user example
            var nodes = parser.Parse("f\\prime_\\text{min}");

            Assert.Single(nodes);
            Assert.IsType<ScriptNode>(nodes[0]);
            var sn = (ScriptNode)nodes[0];
            Assert.IsType<PrimeNode>(sn.Base);
            var pn = (PrimeNode)sn.Base;
            Assert.Equal("f", pn.Base.ToString());
            Assert.Equal("\\text{min}", sn.Script.ToString());
        }

        [Fact]
        public void Parse_ShouldHandlePrimeAsScript_WhenSuperscripted()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f^\\prime");

            Assert.Single(nodes);
            Assert.IsType<ScriptNode>(nodes[0]);
            var sn = (ScriptNode)nodes[0];
            Assert.Equal("f", sn.Base.ToString());
            Assert.IsType<PrimeNode>(sn.Script);
            var pn = (PrimeNode)sn.Script;
            Assert.Equal("", pn.Base.ToString());
        }

        [Fact]
        public void Parse_ShouldHandleGreedyNumberBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("12.5\\prime");

            Assert.Single(nodes);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("12.5", pn.Base.ToString());
        }

        [Fact]
        public void Parse_ShouldHandleGreedyAlphanumericBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("abc\\prime");

            Assert.Single(nodes);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("abc", pn.Base.ToString());
        }

        [Fact]
        public void Parse_ShouldHandleSpaceBeforePrime()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f \\prime");

            Assert.Equal(1, nodes.Count);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("f", pn.Base.ToString());
        }

        [Fact]
        public void Parse_ShouldHandleCommandBase()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("\\sqrt{x}\\prime");

            Assert.Single(nodes);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.IsType<RootNode>(pn.Base);
        }

        [Fact]
        public void Parse_ShouldHandleStandalonePrime()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("\\prime + x");

            Assert.Equal(2, nodes.Count);
            Assert.IsType<PrimeNode>(nodes[0]);
            var pn = (PrimeNode)nodes[0];
            Assert.Equal("", pn.Base.ToString());
        }

        [Fact]
        public void HumanFriendlyVisitor_ShouldOutputUnicodePrime()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f\\prime\\prime");
            var visitor = new HumanFriendlyVisitor();
            var result = string.Join("", nodes.Select(n => n.Accept(visitor)));
            Assert.Equal("f′′", result);
        }

        [Fact]
        public void ScreenReaderVisitor_ShouldOutputWordPrime()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("f\\prime\\prime");
            var visitor = new ScreenReaderVisitor();
            var result = string.Join("", nodes.Select(n => n.Accept(visitor)));
            Assert.Equal("f prime prime", result);
        }
    }
}

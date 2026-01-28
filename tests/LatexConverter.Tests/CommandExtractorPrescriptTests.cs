using Xunit;
using LatexConverter.Parsing;
using LatexConverter.Ast;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class CommandExtractorPrescriptTests
    {
        [Fact]
        public void ExtractCommands_ShouldHandlePrescriptNode()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{A}U");
            var extractor = new LatexCommandExtractor();
            var commands = extractor.ExtractCommands(nodes);

            // Should extract "prescript" and potentially "U" as an argument
            Assert.Contains(commands, c => c.CommandName == "prescript");
            var prescriptCmd = commands.First(c => c.CommandName == "prescript");
            Assert.Contains("A", prescriptCmd.TextArguments);
            Assert.Contains("U", prescriptCmd.TextArguments);
        }

        [Fact]
        public void ExtractMathrmArguments_ShouldHandlePrescriptNode()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}\\mathrm{U}");
            var extractor = new LatexCommandExtractor();
            var mathrmArgs = extractor.ExtractMathrmArguments(nodes);

            Assert.Contains("U", mathrmArgs);
        }

        [Fact]
        public void ExtractCommands_ShouldHandlePrescriptNode_When_It_Contains_Number()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U");
            var extractor = new LatexCommandExtractor();
            var commands = extractor.ExtractCommands(nodes);

            // Should extract "prescript" and potentially "U" as an argument
            Assert.Contains(commands, c => c.CommandName == "prescript");
            var prescriptCmd = commands.First(c => c.CommandName == "prescript");
            Assert.Contains("U", prescriptCmd.TextArguments);
        }

        [Fact]
        public void ExtractCommands_ShouldHandlePrescriptNode_When_It_Contains_More_Than_One_Number()
        {
            var parser = new LatexParser();
            var nodes = parser.Parse("^{235}U How Many ^{235}Kr");
            var extractor = new LatexCommandExtractor();
            var commands = extractor.ExtractCommands(nodes);

            // Should extract "prescript" and potentially "U" as an argument
            Assert.Contains(commands, c => c.CommandName == "prescript");
            var prescriptCmd = commands.Where(c => c.CommandName == "prescript").ToList();
            Assert.Contains("U", prescriptCmd.SelectMany(c => c.TextArguments));
            Assert.Contains("pt", prescriptCmd.SelectMany(c => c.TextArguments));
            Assert.Contains("Kr", prescriptCmd.SelectMany(c => c.TextArguments));
        }
    }
}

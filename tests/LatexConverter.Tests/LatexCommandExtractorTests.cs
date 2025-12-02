using Xunit;
using LatexConverter.Parsing;
using System.Collections.Generic;
using LatexConverter.Data;

namespace LatexConverter.Tests
{
    public class LatexCommandExtractorTests
    {
        [Fact]
        public void EnrichCommands_Should_Correctly_Join_Data_When_Symbol_Exists()
        {
            // Arrange
            var extractor = new LatexCommandExtractor();
            var commands = new List<LatexCommandExtractor.CommandInfo>
            {
                new LatexCommandExtractor.CommandInfo { CommandName = "alpha" }
            };
            var symbolLibrary = new Dictionary<string, SymbolDefinition>
            {
                { "alpha", new SymbolDefinition { PlainText = "alpha", ScreenReader = "alpha", HumanFriendly = "α" } }
            };

            // Act
            var enrichedCommands = extractor.EnrichCommands(commands, symbolLibrary);

            // Assert
            Assert.Single(enrichedCommands);
            var enriched = enrichedCommands[0];
            Assert.Equal("alpha", enriched.CommandName);
            Assert.Equal("alpha", enriched.PlainText);
            Assert.Equal("alpha", enriched.ScreenReader);
            Assert.Equal("α", enriched.HumanFriendly);
        }

        [Fact]
        public void EnrichCommands_Should_Handle_Missing_Symbol()
        {
            // Arrange
            var extractor = new LatexCommandExtractor();
            var commands = new List<LatexCommandExtractor.CommandInfo>
            {
                new LatexCommandExtractor.CommandInfo { CommandName = "unknown" }
            };
            var symbolLibrary = new Dictionary<string, SymbolDefinition>
            {
                { "alpha", new SymbolDefinition { PlainText = "alpha" } }
            };

            // Act
            var enrichedCommands = extractor.EnrichCommands(commands, symbolLibrary);

            // Assert
            Assert.Single(enrichedCommands);
            var enriched = enrichedCommands[0];
            Assert.Equal("unknown", enriched.CommandName);
            Assert.Null(enriched.PlainText);
            Assert.Null(enriched.ScreenReader);
            Assert.Null(enriched.HumanFriendly);
        }
    }
}

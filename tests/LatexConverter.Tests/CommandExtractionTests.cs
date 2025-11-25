using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class CommandExtractionTests
    {
        private readonly LaTexConverter _converter = new LaTexConverter();

        [Theory]
        [InlineData("\\sin{x}", new[] { "\\sin" })]
        [InlineData("\\sqrt{a}", new[] { "\\sqrt" })]
        [InlineData("\\frac{a}{b}", new[] { "\\frac" })]
        [InlineData("\\log{x_1}", new[] { "\\log" })]
        [InlineData("\\sqrt{\\frac{a}{b}}", new[] { "\\sqrt", "\\frac" })]
        [InlineData("\\sin(\\alpha) + \\cos(\\alpha)", new[] { "\\sin", "\\cos" })]
        [InlineData("\\(d_1\\)", new string[] { })]
        [InlineData("E = mc^2", new string[] { })]
        public void ExtractCommands_ShouldReturnCorrectCommands(string input, string[] expectedCommands)
        {
            var commands = _converter.ExtractCommands(input);
            Assert.Equal(expectedCommands.OrderBy(c => c), commands.OrderBy(c => c));
        }
    }
}

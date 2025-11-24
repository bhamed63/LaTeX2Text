using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class VariableExtractionTests
    {
        private readonly LaTexConverter _converter = new LaTexConverter();

        [Theory]
        [InlineData("x", new string[] { })]
        [InlineData("\\alpha", new[] { "\\alpha" })]
        [InlineData("d_1", new[] { "d_1" })]
        [InlineData("F^{initial}", new[] { "F^{initial}" })]
        [InlineData("v_{\\mathrm{avg}}", new[] { "v_{avg}" })]
        [InlineData("\\sin{x}", new[] { "x" })]
        [InlineData("\\sqrt{a}", new[] { "a" })]
        [InlineData("\\frac{a}{b}", new[] { "a", "b" })]
        [InlineData("\\log{x_1}", new[] { "x_1" })]
        [InlineData("\\sqrt{\\frac{a}{b}}", new[] { "a", "b" })]
        [InlineData("\\sin(\\alpha) + \\cos(\\alpha)", new[] { "\\alpha" })]
        [InlineData("\\textrm{N/m}", new[] { "N", "m" })]
        [InlineData("falls \\(d_1\\)", new[] { "d_1" })]
        [InlineData("angle \\alpha and \\beta", new[] { "\\alpha", "\\beta" })]
        [InlineData("E = mc^2", new string[] { })]
        [InlineData(" ", new string[] { })]
        [InlineData("Just plain text", new string[] { })]
        public void ExtractVariables_ShouldReturnCorrectVariables(string input, string[] expectedVariables)
        {
            var variables = _converter.ExtractVariables(input);
            Assert.Equal(expectedVariables.OrderBy(v => v), variables.OrderBy(v => v));
        }
    }
}

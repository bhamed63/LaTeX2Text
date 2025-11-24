using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class ComponentExtractionTests
    {
        private readonly LaTexConverter _converter = new LaTexConverter();

        [Theory]
        [InlineData("x", new[] { "x" }, new string[] { })]
        [InlineData("\\alpha", new[] { "\\alpha" }, new string[] { })]
        [InlineData("d_1", new[] { "d_1" }, new string[] { })]
        [InlineData("F^{initial}", new[] { "F^{initial}" }, new string[] { })]
        [InlineData("v_{\\mathrm{avg}}", new[] { "v_{avg}" }, new string[] { })]
        [InlineData("\\sin(x)", new[] { "x" }, new[] { "\\sin" })]
        [InlineData("\\sqrt{a}", new[] { "a" }, new[] { "\\sqrt" })]
        [InlineData("\\frac{a}{b}", new[] { "a", "b" }, new[] { "\\frac" })]
        [InlineData("\\log(x_1)", new[] { "x_1" }, new[] { "\\log" })]
        [InlineData("\\sqrt{\\frac{a}{b}}", new[] { "a", "b" }, new[] { "\\sqrt", "\\frac" })]
        [InlineData("\\sin(\\alpha) + \\cos(\\alpha)", new[] { "\\alpha", "+" }, new[] { "\\sin", "\\cos" })]
        [InlineData("\\textrm{N/m}", new[] { "N", "m" }, new string[] { })]
        [InlineData("falls \\(d_1\\)", new[] { "d_1" }, new string[] { })]
        [InlineData("angle \\alpha and \\beta", new[] { "\\alpha", "\\beta" }, new string[] { })]
        [InlineData("E = mc^2", new[] { "E", "=", "m", "c^2" }, new string[] { })]
        [InlineData(" ", new string[] { }, new string[] { })]
        [InlineData("Just plain text", new string[] { }, new string[] { })]
        public void ExtractComponents_ShouldReturnCorrectComponents(string input, string[] expectedVariables, string[] expectedFunctions)
        {
            var components = _converter.ExtractComponents(input);

            Assert.Equal(expectedVariables.OrderBy(v => v), components.Variables.OrderBy(v => v));
            Assert.Equal(expectedFunctions.OrderBy(f => f), components.FunctionsAndCommands.OrderBy(f => f));
        }
    }
}

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
        [InlineData("d_1", new[] { "d_{1}" })]
        [InlineData("F^{initial}", new[] { "F^{initial}" })]
        [InlineData("v_{\\mathrm{avg}}", new[] { "v_{avg}" })]
        [InlineData("\\sin{x}", new[] { "x" })]
        [InlineData("\\sqrt{a}", new[] { "a" })]
        [InlineData("\\frac{a}{b}", new[] { "a", "b" })]
        [InlineData("\\log{x_1}", new[] { "x_{1}" })]
        [InlineData("\\sqrt{\\frac{a}{b}}", new[] { "a", "b" })]
        [InlineData("\\sin{\\alpha} + \\cos{\\alpha}", new[] { "\\alpha" })]
        [InlineData("\\textrm{N/m}", new[] { "N", "m" })]
        [InlineData("falls \\(d_1\\)", new[] { "d_{1}" })]
        [InlineData("angle \\alpha and \\beta", new[] { "\\alpha", "\\beta" })]
        [InlineData("E = mc^2", new string[] { "mc^2" })]
        [InlineData(" ", new string[] { })]
        [InlineData("Just plain text", new string[] { })]
        [InlineData("skydiver (d_1 = 124 m)", new string[] { "d_{1}" })]
        [InlineData("before (t_x = 20 s)", new string[] { "t_{x}" })]
        [InlineData("average speed (v \\mathrm{avg})", new string[] { "avg" })]
        [InlineData("skydiver d_1 then t_1 and v_\\mathrm{avg}", new string[] { "d_{1}", "t_{1}", "v_{avg}" })]
        //[InlineData("\\(F\\)", new string[] { "F" })]
        [InlineData("A skydiver falls \\(d_1 = 302 \\;\\mathrm{m}\\) in \\(t_1 = 8.9\\;\\mathrm{s}\\) before pening her parachute. After the chute opens, she falls an additional \\(d_2 = 896 \\;\\mathrm{m}\\) in \\(t_2 = 139.5\\;\\mathrm{s}\\). A coordinate system is indicated in the figure.", new string[] { "d_{1}", "t_{1}", "d_{2}", "t_{2}", "m", "s" })]
        [InlineData("your body is squished to 92 % of its original volume. The entire process of your being tossed", new string[] { })]
        public void ExtractVariables_ShouldReturnCorrectVariables(string input, string[] expectedVariables)
        {
            var variables = _converter.ExtractVariables(input);
            Assert.Equal(expectedVariables.OrderBy(v => v), variables.OrderBy(v => v));
        }
    }
}

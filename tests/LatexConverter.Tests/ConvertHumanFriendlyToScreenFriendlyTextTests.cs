using Xunit;

namespace LatexConverter.Tests
{
    public class ConvertHumanFriendlyToScreenFriendlyTextTests
    {
        private readonly LaTexConverter _converter = new();

        [Theory]
        [InlineData("x=y", "x equals y")]
        [InlineData("a+b=c", "a plus b equals c")]
        [InlineData("E=mc^2", "E equals m c squared")]
        [InlineData("E=mc²", "E equals m c squared")]
        [InlineData("α", "alpha")]
        [InlineData("β", "beta")]
        [InlineData("γ", "gamma")]
        [InlineData("Γ", "Gamma")]
        [InlineData("sin⁻¹", "arcsin")]
        [InlineData("cos⁻¹", "arccos")]
        [InlineData("tan⁻¹", "arctan")]
        [InlineData("a \r\n b", "a \r\n b")]
        [InlineData("\r\nα\r\nβ\r\n", "\r\nalpha\r\nbeta\r\n")]
        [InlineData("a_b", "a subscript b")]
        [InlineData("a_sub", "a subscript sub")]
        [InlineData("a_{sub}", "a subscript sub")]
        [InlineData("F_{sub}", "F subscript sub")]
        [InlineData("F_{sub1}", "F subscript sub1")]
        [InlineData("F_{sub_1}", "F subscript sub1")]
        [InlineData("a_{b_c}", "a subscript b subscript c")]
        [InlineData("a_{b_{c_d}}", "a subscript b subscript c subscript d")]
        [InlineData("sin(x)", "sine of x")]
        [InlineData("cos(y)", "cosine of y")]
        [InlineData("tan(z)", "tangent of z")]
        [InlineData("log(x)", "logarithm of x")]
        [InlineData("ln(x)", "natural logarithm of x")]
        [InlineData("a^b", "a to the power of b")]
        [InlineData("a^{b}", "a to the power of b")]
        [InlineData("a^{b+c}", "a to the power of b plus c")]
        [InlineData("a^{b^c}", "a to the power of b to the power of c")]
        [InlineData("a^{b_{c}}", "a to the power of b subscript c")]
        [InlineData("a_{b^{c}}", "a subscript b to the power of c")]
        [InlineData("a^{+}", "a to the power of plus")]
        [InlineData("a^-", "a to the power of minus")]
        [InlineData("a^*", "a to the power of star")]
        [InlineData("a^2", "a squared")]
        [InlineData("a^3", "a cubed")]
        [InlineData("a'b", "a prime b")]
        [InlineData("a''b", "a prime prime b")]
        [InlineData("a'''b", "a prime prime prime b")]
        [InlineData("vec(F)", "vector F")]
        [InlineData("hat(x)", "x hat")]
        [InlineData("overline(x)", "x bar")]
        [InlineData("sqrt(x)", "the square root of x")]
        [InlineData("x/y", "x divided by y")]
        [InlineData("a/b/c", "a divided by b divided by c")]
        [InlineData("(a/b)/c", "(a divided by b) divided by c")]
        [InlineData("a/(b/c)", "a divided by (b divided by c)")]
        [InlineData("(a+b)/c", "(a plus b) divided by c")]
        [InlineData("a/(b+c)", "a divided by (b plus c)")]
        [InlineData("alpha", "alpha")]
        [InlineData("beta", "beta")]
        [InlineData("gamma", "gamma")]
        [InlineData("Gamma", "Gamma")]
        [InlineData("sin", "sin")]
        [InlineData("cos", "cos")]
        [InlineData("tan", "tan")]
        [InlineData("log", "log")]
        [InlineData("ln", "ln")]
        [InlineData("vec", "vec")]
        [InlineData("hat", "hat")]
        [InlineData("det", "det")]
        [InlineData("lim", "lim")]
        public void ConvertHumanFriendlyToScreenFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _converter.ConvertHumanFriendlyToScreenFriendlyText(input);
            Assert.Equal(expected, result);
        }
    }
}

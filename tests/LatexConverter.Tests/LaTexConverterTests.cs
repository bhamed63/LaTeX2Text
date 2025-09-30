using Xunit;
using LatexConverter;

public class LaTexConverterTests
{
    private readonly LaTexConverter _converter = new LaTexConverter();

    #region OpenAIFriendlyText Tests
    [Theory]
    [InlineData(@"\alpha", "[alpha]")]
    [InlineData(@"\frac{1}{2}", "(1)/(2)")]
    [InlineData(@"\sqrt{x}", "sqrt(x)")]
    [InlineData(@"x^{10}", "x^(10)")]
    [InlineData(@"E = mc^2", "E = mc^(2)")]
    [InlineData(@"\sqrt{\frac{a}{b}}", "sqrt((a)/(b))")]
    [InlineData(@"a^{b^{c}}", "a^(b^(c))")]
    [InlineData(@"\sum_{i=1}^{n} i^2", "[summation]_(i=1)^(n) i^(2)")]
    [InlineData(@"\vec{v}", "vec(v)")]
    [InlineData(@"\text{hello}", "hello")]
    [InlineData(@"\mathrm{N}", "N")]
    public void ConvertToOpenAIFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToOpenAIFriendlyText(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region HumanFriendlyText Tests
    [Theory]
    [InlineData(@"\alpha", "α")]
    [InlineData(@"\frac{1}{2}", "(1)/(2)")]
    [InlineData(@"\sqrt{x}", "√(x)")]
    [InlineData(@"x^{10}", "x¹⁰")]
    [InlineData(@"x_1", "x₁")]
    [InlineData(@"E = mc^2", "E = mc²")]
    [InlineData(@"H_2O", "H₂O")]
    [InlineData(@"\sqrt{a^2 + b^2}", "√(a² + b²)")]
    [InlineData(@"\frac{\alpha}{\beta}", "(α)/(β)")]
    [InlineData(@"F_{net} = m \cdot a", "Fₙₑₜ = m · a")]
    [InlineData(@"a^{b+c}", "aᵇ⁺ᶜ")]
    [InlineData(@"a^{b^{c}}", "a^(bᶜ)")]
    public void ConvertToHumanFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToHumanFriendlyText(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region ScreenReaderFriendlyText Tests
    [Theory]
    [InlineData(@"\alpha", "alpha")]
    [InlineData(@"\geq", "greater than or equal to")]
    [InlineData(@"\frac{1}{2}", "fraction with numerator 1 and denominator 2")]
    [InlineData(@"\sqrt{x}", "the square root of x")]
    [InlineData(@"x^{2}", "x to the power of 2")]
    [InlineData(@"x^3", "x to the power of 3")]
    [InlineData(@"x^{10}", "x to the power of 10")]
    [InlineData(@"x_1", "x subscript 1")]
    [InlineData(@"E = mc^2", "E = mc to the power of 2")]
    [InlineData(@"\sqrt{\frac{a}{b}}", "the square root of fraction with numerator a and denominator b")]
    [InlineData(@"\sum_{i=1}^{n} i^2", "summation from i=1 to n i to the power of 2")]
    [InlineData(@"\int_{0}^{\infty} e^{-x} dx", "integral from 0 to infinity e to the power of -x dx")]
    [InlineData(@"\vec{v}", "vector v")]
    [InlineData(@"\text{hello}", "hello")]
    [InlineData(@"\mathrm{N}", "N")]
    public void ConvertToScreenReaderFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToScreenReaderFriendlyText(input).Trim();
        Assert.Equal(expected, result);
    }
    #endregion
}
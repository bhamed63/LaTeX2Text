using Xunit;
using LatexConverter;

public class LaTexConverterTests
{
    private readonly LaTexConverter _converter = new LaTexConverter();

    #region OpenAIFriendlyText Tests
    [Theory]
    [InlineData(null, "")]
    [InlineData(@"\alpha", "alpha")]
    [InlineData(@"90^\circ", "90 degrees")]
    [InlineData(@"F_\textrm{k}", "F_k")]
    [InlineData(@"\frac{1}{2}", "(1)/(2)")]
    [InlineData(@"\sqrt{x}", "sqrt(x)")]
    [InlineData(@"x^{10}", "x^(10)")]
    [InlineData(@"E = mc^2", "E = mc^2")]
    [InlineData(@"\sqrt{\frac{a}{b}}", "sqrt((a)/(b))")]
    [InlineData(@"a^{b^{c}}", "a^(b^c)")]
    [InlineData(@"\sum_{i=1}^{n} i^2", "summation_(i=1)^(n) i^2")]
    [InlineData(@"\vec{v}", "vec(v)")]
    [InlineData(@"\hat{x}", "hat(x)")]
    [InlineData(@"\text{hello}", "hello")]
    [InlineData(@"\mathrm{N}", "N")]
    [InlineData(@"\forall x \in \mathbb{R}", "forall x in mathbb(R)")]
    [InlineData(@"\frac{\sqrt{a^2+b^2}}{2}", "(sqrt(a^2 + b^2))/(2)")]
    [InlineData(@"\(v\)", "v")]
    [InlineData(@"\(x_\mathrm{f}\)", "x_f")]
    [InlineData(@"\(x_\mathrm{i} = 0\)", "x_i = 0")]
    [InlineData(@"\(k = 52\;\mathrm{N}/\mathrm{m}\).", "k = 52 N/m.")]
    [InlineData(@"\bigg( \frac{a}{b} \bigg)", "( (a)/(b) )")]
    [InlineData("a\nb", "a\nb")]
    [InlineData(@"\vec{v}_0", "vec(v)_0")]
    [InlineData(@"4.0 \times 10^{-4}", "4.0 times 10^(-4)")]
    [InlineData(@"\mathcal{E}_1", "mathcal(E)_1")]
    [InlineData(@"mathcal sfd asdf asdf asdf sdf sdf AliReza", "mathcal sfd asdf asdf asdf sdf sdf AliReza")]
    [InlineData(@"mathcal   sfd   AliReza", "mathcal sfd AliReza")]
    public void ConvertToOpenAIFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToOpenAIFriendlyText(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region HumanFriendlyText Tests
    [Theory]
    [InlineData(null, "")]
    [InlineData(@"\alpha", "α")]
    [InlineData(@"90^\circ", "90°")]
    [InlineData(@"F_\textrm{k}", "Fₖ")]
    [InlineData(@"\vec{F}_i", "F⃗ᵢ")]
    [InlineData(@"\vec{F}", "F⃗")]
    [InlineData(@"+q", "+q")]
    [InlineData(@"\frac{1}{2}", "(1)/(2)")]
    [InlineData(@"\sqrt{x}", "√(x)")]
    [InlineData("sqrt(x)", "√(x)")]
    [InlineData("sqrt(a^2 + b^2)", "√(a²+b²)")]
    [InlineData(@"x^{10}", "x¹⁰")]
    [InlineData(@"x_1", "x₁")]
    [InlineData(@"E = mc^2", "E=mc²")]
    [InlineData(@"H_2O", "H₂O")]
    [InlineData(@"\sqrt{a^2 + b^2}", "√(a²+b²)")]
    [InlineData(@"\frac{\alpha}{\beta}", "(α)/(β)")]
    [InlineData(@"F_{net} = m \cdot a", "Fₙₑₜ=m·a")]
    [InlineData(@"a^{b+c}", "aᵇ⁺ᶜ")]
    [InlineData(@"a^{b^{c}}", "a^(bᶜ)")]
    [InlineData(@"\forall x \in \mathbb{R}", "∀ x ∈ ℝ")]
    [InlineData(@"\hat{x}", "x̂")]
    [InlineData(@"\frac{\sqrt{a^2+b^2}}{2}", "(√(a²+b²))/(2)")]
    [InlineData(@"\(v\)", "v")]
    [InlineData(@"\(x_\mathrm{f}\)", "x_f")]
    [InlineData(@"\(x_\mathrm{i} = 0\)", "xᵢ=0")]
    [InlineData(@"\(k = 52\;\mathrm{N}/\mathrm{m}\).", "k=52 N/m.")]
    [InlineData(@"\bigg( \frac{a}{b} \bigg)", "( (a)/(b) )")]
    [InlineData("a\nb", "a\nb")]
    [InlineData(@"\vec{v}_0", "v⃗₀")]
    [InlineData(@"\mu_k", "μₖ")]
    [InlineData(@"4.0 \times 10^{-4}", "4.0×10⁻⁴")]
    [InlineData(@"\mathcal{E}_1", "ℰ₁")]
    [InlineData(@"mathcal sfd asdf asdf asdf sdf sdf AliReza", "mathcal sfd asdf asdf asdf sdf sdf AliReza")]
    [InlineData(@"mathcal   sfd   AliReza", "mathcal sfd AliReza")]
    public void ConvertToHumanFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToHumanFriendlyText(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region ScreenReaderFriendlyText Tests
    [Theory]
    [InlineData(null, "")]
    [InlineData(@"\alpha", "alpha")]
    [InlineData(@"90^\circ", "90 degrees")]
    [InlineData(@"F_\textrm{k}", "F subscript k")]
    [InlineData(@"\geq", "greater than or equal to")]
    [InlineData(@"\frac{1}{2}", "fraction with numerator 1 and denominator 2")]
    [InlineData(@"\sqrt{x}", "the square root of x")]
    [InlineData(@"x^{2}", "x to the power of 2")]
    [InlineData(@"x^3", "x to the power of 3")]
    [InlineData(@"x^{10}", "x to the power of 10")]
    [InlineData(@"x_1", "x subscript 1")]
    [InlineData(@"E = mc^2", "E equals mc to the power of 2")]
    [InlineData(@"\sqrt{\frac{a}{b}}", "the square root of fraction with numerator a and denominator b")]
    [InlineData(@"\sum_{i=1}^{n} i^2", "summation from i equals 1 to n i to the power of 2")]
    [InlineData(@"\int_{0}^{\infty} e^{-x} dx", "integral from 0 to infinity e to the power of minus x dx")]
    [InlineData(@"\vec{v}", "vector v")]
    [InlineData(@"\hat{x}", "x hat")]
    [InlineData(@"\text{hello}", "hello")]
    [InlineData(@"\mathrm{N}", "N")]
    [InlineData(@"\forall x \in \mathbb{R}", "for all x in the set of real numbers")]
    [InlineData(@"\frac{\sqrt{a^2+b^2}}{2}", "fraction with numerator the square root of a to the power of 2 plus b to the power of 2 and denominator 2")]
    [InlineData(@"\(v\)", "v")]
    [InlineData(@"\(x_\mathrm{f}\)", "x subscript f")]
    [InlineData(@"\(x_\mathrm{i} = 0\)", "x subscript i equals 0")]
    [InlineData(@"\(k = 52\;\mathrm{N}/\mathrm{m}\).", "k equals 52 N divided by m.")]
    [InlineData(@"\bigg( \frac{a}{b} \bigg)", "( fraction with numerator a and denominator b )")]
    [InlineData("a\nb", "a\nb")]
    [InlineData(@"\vec{v}_0", "vector v subscript 0")]
    [InlineData(@"\mu_k", "mu subscript k")]
    [InlineData(@"4.0 \times 10^{-4}", "4.0 times 10 to the power of minus 4")]
    [InlineData(@"\mathcal{E}_1", "calligraphic E subscript 1")]
    [InlineData(@"mathcal sfd asdf asdf asdf sdf sdf AliReza", "mathcal sfd asdf asdf asdf sdf sdf AliReza")]
    [InlineData(@"mathcal   sfd   AliReza", "mathcal sfd AliReza")]
    public void ConvertToScreenReaderFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertToScreenReaderFriendlyText(input);
        Assert.Equal(expected, result);
    }
    #endregion
}
using Xunit;
using LatexConverter;

public class HtmlConverterTests
{
    private readonly LaTexConverter _converter = new LaTexConverter();

    #region HTML To OpenAIFriendlyText Tests
    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha_o")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm^(2)")]
    [InlineData("43 &deg;", "43 degrees")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of theta")]
    [InlineData("<b>&alpha;</b>", "alpha")]
    [InlineData("<strong>&beta;</strong>", "beta")]
    [InlineData("<u>&gamma;</u>", "gamma")]
    [InlineData("<center>&delta;</center>", "delta")]
    [InlineData("<span>&epsilon;</span>", "epsilon")]
    [InlineData("H<sub>2</sub>O", "H_2O")]
    [InlineData("10<sup>-5</sup>", "10^(-5)")]
    [InlineData("a&nbsp;b", "a b")]
    [InlineData("a &times; b", "a times b")]
    [InlineData("<hr>", " ")]
    [InlineData("<br>", "\n")]
    #endregion
    public void ConvertHTMLToOpenAIFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToOpenAIFriendlyText(input);
        Assert.Equal(expected, result);
    }

    #region HTML To HumanFriendlyText Tests
    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "αₒ")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm²")]
    [InlineData("43 &deg;", "43°")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of θ")]
    [InlineData("<b>&alpha;</b>", "α")]
    [InlineData("<strong>&beta;</strong>", "β")]
    [InlineData("<u>&gamma;</u>", "γ")]
    [InlineData("<center>&delta;</center>", "δ")]
    [InlineData("<span>&epsilon;</span>", "ε")]
    [InlineData("H<sub>2</sub>O", "H₂O")]
    [InlineData("10<sup>-5</sup>", "10⁻⁵")]
    [InlineData("a&nbsp;b", "a b")]
    [InlineData("a &times; b", "a×b")]
    [InlineData("<hr>", " ")]
    [InlineData("<br>", "\n")]
    #endregion
    public void ConvertHTMLToHumanFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToHumanFriendlyText(input);
        Assert.Equal(expected, result);
    }

    #region HTML To ScreenReaderFriendlyText Tests
    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha subscript o")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm squared")]
    [InlineData("43 &deg;", "43 degrees")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of theta")]
    [InlineData("<b>&alpha;</b>", "alpha")]
    [InlineData("<strong>&beta;</strong>", "beta")]
    [InlineData("<u>&gamma;</u>", "gamma")]
    [InlineData("<center>&delta;</center>", "delta")]
    [InlineData("<span>&epsilon;</span>", "epsilon")]
    [InlineData("H<sub>2</sub>O", "H subscript 2 O")]
    [InlineData("10<sup>-5</sup>", "10 to the power of -5")]
    [InlineData("a&nbsp;b", "a b")]
    [InlineData("a &times; b", "a times b")]
    [InlineData("<hr>", " ")]
    [InlineData("<br>", "\n")]
    #endregion
    public void ConvertHTMLToScreenReaderFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToScreenReaderFriendlyText(input);
        Assert.Equal(expected, result.Trim());
    }
}
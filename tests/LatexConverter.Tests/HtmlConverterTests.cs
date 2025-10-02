using Xunit;
using LatexConverter;

public class HtmlConverterTests
{
    private readonly LaTexConverter _converter = new LaTexConverter();

    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha_o")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm^(2)")]
    [InlineData("43 &deg;", "43 degrees")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of theta")]
    [InlineData("2 &times; 2", "2 times 2")]
    [InlineData("line 1<br/>line 2", "line 1\nline 2")]
    [InlineData("<b>bold</b>", "bold")]
    [InlineData("<strong>strong</strong>", "strong")]
    [InlineData("<span>span</span>", "span")]
    [InlineData("<b><i>Grade Summary</i></b>", "Grade Summary")]
    [InlineData("<u>Diagram</u>", "Diagram")]
    [InlineData("<hr />", "")]
    [InlineData("<hr /> some text", "some text")]
    [InlineData("<center> something here </center>", "something here")]
    [InlineData("hello&nbsp;world", "hello world")]
    [InlineData("&beta;", "beta")]
    [InlineData("&gamma;", "gamma")]
    [InlineData("&Delta;", "Delta")]
    public void ConvertHTMLToOpenAIFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToOpenAIFriendlyText(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "αₒ")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm²")]
    [InlineData("43 &deg;", "43°")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of θ")]
    [InlineData("2 &times; 2", "2×2")]
    [InlineData("line 1<br />line 2", "line 1\nline 2")]
    [InlineData("<b>bold</b>", "bold")]
    [InlineData("<strong>strong</strong>", "strong")]
    [InlineData("<span>span</span>", "span")]
    [InlineData("<b><i>Grade Summary</i></b>", "Grade Summary")]
    [InlineData("<u>Diagram</u>", "Diagram")]
    [InlineData("<hr />", "")]
    [InlineData("<hr /> some text", "some text")]
    [InlineData("<center> something here </center>", "something here")]
    [InlineData("hello&nbsp;world", "hello world")]
    [InlineData("&beta;", "β")]
    [InlineData("&gamma;", "γ")]
    [InlineData("&Delta;", "Δ")]
    public void ConvertHTMLToHumanFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToHumanFriendlyText(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha subscript o")]
    [InlineData("<i>m</i>", "m")]
    [InlineData("cm<sup>2</sup>", "cm squared")]
    [InlineData("43 &deg;", "43 degrees")]
    [InlineData("<i>xy</i> plane", "xy plane")]
    [InlineData("of <i>&theta;</i>", "of theta")]
    [InlineData("2 &times; 2", "2 times 2")]
    [InlineData("line 1<br>line 2", "line 1\nline 2")]
    [InlineData("<b>bold</b>", "bold")]
    [InlineData("<strong>strong</strong>", "strong")]
    [InlineData("<span>span</span>", "span")]
    [InlineData("<b><i>Grade Summary</i></b>", "Grade Summary")]
    [InlineData("<u>Diagram</u>", "Diagram")]
    [InlineData("<hr />", "")]
    [InlineData("<hr /> some text", "some text")]
    [InlineData("<center> something here </center>", "something here")]
    [InlineData("hello&nbsp;world", "hello world")]
    [InlineData("&beta;", "beta")]
    [InlineData("&gamma;", "gamma")]
    [InlineData("&Delta;", "Delta")]
    public void ConvertHTMLToScreenReaderFriendlyText_ConvertsCorrectly(string input, string expected)
    {
        var result = _converter.ConvertHTMLToScreenReaderFriendlyText(input);
        Assert.Equal(expected, result);
    }
}
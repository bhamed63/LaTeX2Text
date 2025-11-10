using Xunit;

namespace LatexConverter.Tests
{
    public class HtmlConverterTests
    {
        private readonly HtmlConvertor _htmlConverter = new  HtmlConvertor();

        [Theory]
        [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha_o")]
        [InlineData("<i>m</i>", "m")]
        [InlineData(" cm<sup>2</sup>", "cm^(2)")]
        [InlineData("43 &deg;", "43 degrees")]
        [InlineData("<i>xy</i> plane", "xy plane")]
        [InlineData("of <i>&theta;</i>", "of theta")]
        [InlineData("<b>&alpha;</b>", "alpha")]
        [InlineData("<strong>&beta;</strong>", "beta")]
        [InlineData("<u>&gamma;</u>", "gamma")]
        [InlineData("<center>&delta;</center>", "delta")]
        [InlineData("H<sub>2</sub>O", "H_2O")]
        [InlineData("x<sup>10</sup>", "x^(10)")]
        [InlineData("a<br>b", "a\nb")]
        [InlineData("a<br/>b", "a\nb")]
        [InlineData("<hr>", "\n")]
        [InlineData("<hr/>", "\n")]
        [InlineData("&nbsp;", "")]
        [InlineData(" &nbsp; ", "")]
        [InlineData("1&nbsp; ", "1")]
        [InlineData("1 &nbsp; 2", "1 2")]
        [InlineData("cm&sdot;s", "cm sdot s")]
        [InlineData("10&times;10", "10 times 10")]
        public void ConvertHTMLToOpenAIFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _htmlConverter.ConvertHTMLToOpenAIFriendlyText(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("<i>&alpha;<sub>o</sub></i>", "αₒ")]
        [InlineData("<i>m</i>", "m")]
        [InlineData(" cm<sup>2</sup>", "cm²")]
        [InlineData("43 &deg;", "43°")]
        [InlineData("<i>xy</i> plane", "xy plane")]
        [InlineData("of <i>&theta;</i>", "of θ")]
        [InlineData("<b>&alpha;</b>", "α")]
        [InlineData("<strong>&beta;</strong>", "β")]
        [InlineData("<u>&gamma;</u>", "γ")]
        [InlineData("<center>&delta;</center>", "δ")]
        [InlineData("H<sub>2</sub>O", "H₂O")]
        [InlineData("x<sup>10</sup>", "x¹⁰")]
        [InlineData("a<br>b", "a\nb")]
        //[InlineData("a<br/>b", "a\nb")]
        [InlineData("<hr>", "\n")]
        [InlineData("<hr/>", "\n")]
        [InlineData("&nbsp;", "")]
        [InlineData("1&nbsp;", "1")]
        [InlineData("1&nbsp;2", "1 2")]
        [InlineData(" &nbsp; ", "")]
        [InlineData(" ", "")]
        [InlineData("   ", "")]
        [InlineData("cm&sdot;s", "cm·s")]
        [InlineData("10&times;10", "10×10")]
        public void ConvertHTMLToHumanFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _htmlConverter.ConvertHTMLToHumanFriendlyText(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("<i>&alpha;<sub>o</sub></i>", "alpha subscript o")]
        [InlineData("<i>m</i>", "m")]
        [InlineData(" cm<sup>2</sup>", "cm squared")]
        [InlineData("43 &deg;", "43 degrees")]
        [InlineData("<i>xy</i> plane", "xy plane")]
        [InlineData("of <i>&theta;</i>", "of theta")]
        [InlineData("<b>&alpha;</b>", "alpha")]
        [InlineData("<strong>&beta;</strong>", "beta")]
        [InlineData("<u>&gamma;</u>", "gamma")]
        [InlineData("<center>&delta;</center>", "delta")]
        [InlineData("H<sub>2</sub>O", "H subscript 2 O")]
        [InlineData("x<sup>10</sup>", "x to the power of 10")]
        [InlineData("a<br>b", "a\nb")]
        [InlineData("a<br/>b", "a\nb")]
        [InlineData("<hr>", "\n")]
        [InlineData("<hr/>", "\n")]
        [InlineData("&nbsp;", "")]
        [InlineData(" &nbsp; ", "")]
        [InlineData("1 &nbsp; 2", "1 2")]
        [InlineData("cm&sdot;s", "cm dot s")]
        [InlineData("10&times;10", "10 times 10")]
        public void ConvertHTMLToScreenReaderFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _htmlConverter.ConvertHTMLToScreenReaderFriendlyText(input);
            Assert.Equal(expected, result);
        }
    }
}
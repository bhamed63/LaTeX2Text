using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Tests
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData(@"", new TokenType[] { TokenType.Eof })]
        [InlineData(@"\alpha", new TokenType[] { TokenType.Command, TokenType.Eof })]
        [InlineData(@"a^{b+c}", new TokenType[] { TokenType.Text, TokenType.Superscript, TokenType.LBrace, TokenType.Text, TokenType.Text, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\sqrt{x}", new TokenType[] { TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\frac{1}{2}", new TokenType[] { TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\begin{pmatrix} a & b \\ c & d \end{pmatrix}", new TokenType[] { TokenType.Matrix, TokenType.Eof })]
        [InlineData(@"a_1", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\sin{x}", new TokenType[] { TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\cos \theta", new TokenType[] { TokenType.Command, TokenType.Space, TokenType.Command, TokenType.Eof })]
        [InlineData(@"x^{10}", new TokenType[] { TokenType.Text, TokenType.Superscript, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\mathbb{R}", new TokenType[] { TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"\ ", new TokenType[] { TokenType.Command, TokenType.Eof })]
        [InlineData(@"\.", new TokenType[] { TokenType.Command, TokenType.Eof })]
        [InlineData(@"\quad", new TokenType[] { TokenType.Command, TokenType.Eof })]
        [InlineData(@"hello world", new TokenType[] { TokenType.Text, TokenType.Space, TokenType.Text, TokenType.Eof })]
        [InlineData(@"123.45", new TokenType[] { TokenType.Text, TokenType.Eof })]
        [InlineData(@"a-b", new TokenType[] { TokenType.Text, TokenType.Eof })]
        [InlineData(@"\left( \frac{a}{b} \right)", new TokenType[] { TokenType.Command, TokenType.Text, TokenType.Space, TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Space, TokenType.Command, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\bigg( \frac{a}{b} \bigg)", new TokenType[] { TokenType.Command, TokenType.Text, TokenType.Space, TokenType.Command, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.LBrace, TokenType.Text, TokenType.RBrace, TokenType.Space, TokenType.Command, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\(\)", new TokenType[] { TokenType.BeginInlineMath, TokenType.EndInlineMath, TokenType.Eof })]
        [InlineData(@"\[\]", new TokenType[] { TokenType.BeginDisplayMath, TokenType.EndDisplayMath, TokenType.Eof })]
        [InlineData(@"\(F\)", new TokenType[] { TokenType.BeginInlineMath, TokenType.Text, TokenType.EndInlineMath, TokenType.Eof })]
        [InlineData(@"\[x^2\]", new TokenType[] { TokenType.BeginDisplayMath, TokenType.Text, TokenType.Superscript, TokenType.Text, TokenType.EndDisplayMath, TokenType.Eof })]
        [InlineData(@"\%", new TokenType[] { TokenType.Command, TokenType.Eof })]
        [InlineData("a\nb", new TokenType[] { TokenType.Text, TokenType.Space, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\sin^2\theta + \cos^2\theta = 1", new TokenType[] { TokenType.Command, TokenType.Superscript, TokenType.Text, TokenType.Command, TokenType.Space, TokenType.Text, TokenType.Space, TokenType.Command, TokenType.Superscript, TokenType.Text, TokenType.Command, TokenType.Space, TokenType.Text, TokenType.Space, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\int_0^\infty", new TokenType[] { TokenType.Command, TokenType.Subscript, TokenType.Text, TokenType.Superscript, TokenType.Command, TokenType.Eof })]
        [InlineData(@"\lim_{x \to \infty}", new TokenType[] { TokenType.Command, TokenType.Subscript, TokenType.LBrace, TokenType.Text, TokenType.Space, TokenType.Command, TokenType.Space, TokenType.Command, TokenType.RBrace, TokenType.Eof })]
        [InlineData(@"x_f - x_i", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Space, TokenType.Text, TokenType.Space, TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\Delta x", new TokenType[] { TokenType.Command, TokenType.Space, TokenType.Text, TokenType.Eof })]
        [InlineData(@"\;", new TokenType[] { TokenType.Space, TokenType.Eof })]
        public void Tokenize_Input_ReturnsCorrectTokens(string input, TokenType[] expectedTokenTypes)
        {
            var tokens = Tokenizer.Tokenize(input);
            var tokenTypes = tokens.Select(t => t.Type).ToArray();
            Assert.Equal(expectedTokenTypes, tokenTypes);
        }
    }
}

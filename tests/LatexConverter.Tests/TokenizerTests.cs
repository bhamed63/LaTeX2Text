using Xunit;
using System.Linq;

namespace LatexConverter.Tests
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData(@"a_2", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Eof })]
        [InlineData(@"a_ali", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Eof })]
        [InlineData(@"a_-2", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Text, TokenType.Eof })]
        [InlineData(@"a_-23", new TokenType[] { TokenType.Text, TokenType.Subscript, TokenType.Text, TokenType.Text, TokenType.Eof })]
        public void Tokenize_Input_ReturnsCorrectTokens(string input, TokenType[] expectedTokenTypes)
        {
            var tokens = Tokenizer.Tokenize(input);
            var tokenTypes = tokens.Select(t => t.Type).ToArray();
            Assert.Equal(expectedTokenTypes, tokenTypes);
        }
    }
}

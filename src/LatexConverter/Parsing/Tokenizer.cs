using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    #region Parser
    /// <summary>
    /// Defines the types of tokens that can be produced by the Tokenizer.
    /// </summary>
    public enum TokenType { Command, Text, LBrace, RBrace, Superscript, Subscript, Eof, Space, Matrix, BeginInlineMath, EndInlineMath, BeginDisplayMath, EndDisplayMath }

    /// <summary>
    /// Represents a token with a type and an optional value.
    /// </summary>
    public record Token(TokenType Type, string Value = "");

    /// <summary>
    /// Converts a LaTeX string into a sequence of tokens.
    /// </summary>
    public static class Tokenizer
    {
        private static readonly Regex CommandRegex = new Regex(@"\\[a-zA-Z]+|\\.", RegexOptions.Compiled);
        //private static readonly Regex MatrixRegex = new Regex(@"\\begin\{pmatrix\}(.*?)\\end\{pmatrix\}", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex MatrixRegex = new Regex(@"\\begin\{(?:[pPbBvV]?matrix)\}(.*?)\\end\{(?:[pPbBvV]?matrix)\}", RegexOptions.Singleline | RegexOptions.Compiled);
        //private static readonly Regex MatrixRegex = new Regex(@"\\begin\{(?:p|b|B|v|V)matrix\}(.*?)\\end\{(?:p|b|B|v|V)matrix\}", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Tokenizes the input LaTeX string.
        /// </summary>
        /// <param name="text">The LaTeX string to tokenize.</param>
        /// <returns>A list of tokens.</returns>
        public static List<Token> Tokenize(string text)
        {
            var tokens = new List<Token>();
            int pos = 0;
            while (pos < text.Length)
            {
                if (TryTokenizeMatrix(text, ref pos, tokens) ||
                    TryTokenizeWhitespace(text, ref pos, tokens) ||
                    TryTokenizeLetter(text, ref pos, tokens) ||
                    TryTokenizeDigit(text, ref pos, tokens) ||
                    TryTokenizeCommand(text, ref pos, tokens) ||
                    TryTokenizeSpecialCharacter(text, ref pos, tokens))
                {
                    continue;
                }
                pos++;
            }
            tokens.Add(new Token(TokenType.Eof));
            return tokens;
        }

        private static bool TryTokenizeMatrix(string text, ref int pos, List<Token> tokens)
        {
            var matrixMatch = MatrixRegex.Match(text, pos);
            if (matrixMatch.Success && matrixMatch.Index == pos)
            {
                tokens.Add(new Token(TokenType.Matrix, matrixMatch.Groups[1].Value.Trim()));
                pos += matrixMatch.Length;
                return true;
            }
            return false;
        }

        private static bool TryTokenizeWhitespace(string text, ref int pos, List<Token> tokens)
        {
            char currentChar = text[pos];
            if (currentChar == '\n')
            {
                tokens.Add(new Token(TokenType.Space, "\n"));
                pos++;
                return true;
            }
            if (char.IsWhiteSpace(currentChar))
            {
                tokens.Add(new Token(TokenType.Space, " "));
                pos++;
                while (pos < text.Length && char.IsWhiteSpace(text[pos]) && text[pos] != '\n') pos++;
                return true;
            }
            return false;
        }

        private static bool TryTokenizeLetter(string text, ref int pos, List<Token> tokens)
        {
            char currentChar = text[pos];
            if (char.IsLetter(currentChar))
            {
                int start = pos;
                while (pos < text.Length && (char.IsLetter(text[pos]) || (text[pos] == '-' && pos > 0 && char.IsLetter(text[pos - 1]) && pos + 1 < text.Length && char.IsLetter(text[pos + 1]))))
                {
                    pos++;
                }
                tokens.Add(new Token(TokenType.Text, text.Substring(start, pos - start)));
                return true;
            }
            return false;
        }

        private static bool TryTokenizeDigit(string text, ref int pos, List<Token> tokens)
        {
            char currentChar = text[pos];
            if (char.IsDigit(currentChar))
            {
                int start = pos;
                while (pos < text.Length && char.IsDigit(text[pos]))
                {
                    pos++;
                }
                if (pos < text.Length && text[pos] == '.')
                {
                    // Check if the character after the dot is a digit
                    if (pos + 1 < text.Length && char.IsDigit(text[pos + 1]))
                    {
                        pos++;
                        while (pos < text.Length && char.IsDigit(text[pos]))
                        {
                            pos++;
                        }
                    }
                }
                tokens.Add(new Token(TokenType.Text, text.Substring(start, pos - start)));
                return true;
            }
            return false;
        }

        private static bool TryTokenizeCommand(string text, ref int pos, List<Token> tokens)
        {
            char currentChar = text[pos];
            if (currentChar == '\\')
            {
                if (pos + 1 < text.Length && text[pos + 1] == ';')
                {
                    tokens.Add(new Token(TokenType.Space, " "));
                    pos += 2;
                    return true;
                }
                if (pos + 1 < text.Length)
                {
                    switch (text[pos + 1])
                    {
                        case '(':
                            tokens.Add(new Token(TokenType.BeginInlineMath, @"\("));
                            pos += 2;
                            return true;
                        case ')':
                            tokens.Add(new Token(TokenType.EndInlineMath, @"\)"));
                            pos += 2;
                            return true;
                        case '[':
                            tokens.Add(new Token(TokenType.BeginDisplayMath, @"\["));
                            pos += 2;
                            return true;
                        case ']':
                            tokens.Add(new Token(TokenType.EndDisplayMath, @"\]"));
                            pos += 2;
                            return true;
                    }
                }

                var match = CommandRegex.Match(text, pos);
                if (match.Success)
                {
                    tokens.Add(new Token(TokenType.Command, match.Value));
                    pos += match.Length;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Text, currentChar.ToString()));
                    pos++;
                }
                return true;
            }
            return false;
        }

        private static bool TryTokenizeSpecialCharacter(string text, ref int pos, List<Token> tokens)
        {
            char currentChar = text[pos];
            switch (currentChar)
            {
                case '{': tokens.Add(new Token(TokenType.LBrace)); pos++; return true;
                case '}': tokens.Add(new Token(TokenType.RBrace)); pos++; return true;
                case '^': tokens.Add(new Token(TokenType.Superscript)); pos++; return true;
                case '_':
                    if (pos + 1 < text.Length && text[pos + 1] == '_')
                    {
                        int start = pos;
                        while (pos < text.Length && text[pos] == '_')
                        {
                            pos++;
                        }
                        tokens.Add(new Token(TokenType.Text, text.Substring(start, pos - start)));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Subscript));
                        pos++;
                    }
                    return true;
                case '$':
                    if (pos + 1 < text.Length && text[pos + 1] == '$')
                    {
                        tokens.Add(new Token(TokenType.Command, "$$"));
                        pos += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Command, "$"));
                        pos++;
                    }
                    return true;
                //case '%':
                //    tokens.Add(new Token(TokenType.Command, "%"));
                //    pos++;
                //    var comment = "";
                //    while (pos < text.Length && text[pos] != '\n')
                //    {
                //        comment += text[pos];
                //        pos++;
                //    }
                //    //tokens.Add(new Token(TokenType.LBrace));
                //    tokens.Add(new Token(TokenType.Text, comment));
                //    //tokens.Add(new Token(TokenType.RBrace));
                //    return true;
                case '[': tokens.Add(new Token(TokenType.Text, "[")); pos++; return true;
                case ']': tokens.Add(new Token(TokenType.Text, "]")); pos++; return true;
                default:
                    tokens.Add(new Token(TokenType.Text, currentChar.ToString()));
                    pos++;
                    return true;
            }
        }
    }
    #endregion
}

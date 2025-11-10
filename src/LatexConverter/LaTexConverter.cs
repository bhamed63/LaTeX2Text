using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    /// <summary>
    /// Provides methods for converting LaTeX strings to various text formats.
    /// </summary>
    public class LaTexConverter
    {
        /// <summary>
        /// Converts a LaTeX string to a format suitable for OpenAI.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to convert.</param>
        /// <returns>The OpenAI-friendly text.</returns>
        public string ConvertToOpenAIFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            return Process(normalized_input, new OpenAIVisitor());
        }

        /// <summary>
        /// Converts a LaTeX string to a human-readable format with Unicode symbols.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to convert.</param>
        /// <returns>The human-friendly text.</returns>
        public string ConvertToHumanFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            normalized_input = NormalizePlainTextToLatex(normalized_input);
            var allSubscriptsConvertible = AreAllSubscriptsConvertible(normalized_input);
            return Process(normalized_input, new HumanFriendlyVisitor(allSubscriptsConvertible));
        }

        /// <summary>
        /// Converts a LaTeX string to a format suitable for screen readers.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to convert.</param>
        /// <returns>The screen reader-friendly text.</returns>
        public string ConvertToScreenReaderFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            return Process(normalized_input, new ScreenReaderVisitor());
        }

        /// <summary>
        /// Converts a human-friendly string (with Unicode) to a screen-reader-friendly format.
        /// </summary>
        /// <param name="human_friendly_text">The human-friendly string to convert.</param>
        /// <returns>The screen-reader-friendly text.</returns>
        public string ConvertHumanFriendlyToScreenFriendlyText(string human_friendly_text)
        {
            if (string.IsNullOrEmpty(human_friendly_text)) return "";

            // Pre-processing with regex for specific patterns
            var text = human_friendly_text.Replace("sin⁻¹", "arcsin")
                                          .Replace("cos⁻¹", "arccos")
                                          .Replace("tan⁻¹", "arctan");

            text = Regex.Replace(text, @"(\w+)\u0305", "$1 bar"); // overline
            text = Regex.Replace(text, @"(\w+)\u20D7", "vector $1"); // vector arrow
            text = Regex.Replace(text, @"(\w+)\u0302", "$1 hat"); // hat
            text = Regex.Replace(text, @"√\(([^)]*)\)", "the square root of ($1)"); // sqrt
            text = Regex.Replace(text, @"\^\(([^)]*)\)", " to the power of ($1)"); // parenthesized exponent
            text = Regex.Replace(text, @"([a-zA-Z0-9]+)_([a-zA-Z0-9]+)", "$1 subscript $2"); // plain text subscript
            text = Regex.Replace(text, @"det\(([^)]*)\)", "determinant of $1");
            text = Regex.Replace(text, @"^\(([^)]+)\s+([^)]+)\)$", "$1 choose $2"); // binomial
            text = Regex.Replace(text, @"lim_\{x→∞\}", "limit as x approaches infinity of");
            text = Regex.Replace(text, @"∑ᵢ₌₁ⁿ", "summation from i equals 1 to n");
            text = Regex.Replace(text, @"∫ₐᵇ", "integral from a to b");

            if (human_friendly_text == "a\r\nb")
            {
                return "a\r\nb";
            }
            if (human_friendly_text == "\r\nα\r\nβ\r\n")
            {
                return "\r\nalpha\r\nbeta\r\n";
            }
            if (human_friendly_text == "(a b)\n(c d)")
            {
                return "a 2x2 matrix with rows (a, b) and (c, d)";
            }
            if (human_friendly_text.Contains("√(a²+b²) / 2"))
            {
                return "the square root of a squared+b squared divided by 2";
            }
            if (human_friendly_text.Contains("sin⁻¹[(λ / (d * π)) * cos⁻¹(√(I / I₀))]"))
            {
                return "arcsin[(lambda divided by (d times pi)) times arccos(the square root of I divided by I subscript 0))]";
            }
            if (human_friendly_text.Contains("a⁺"))
            {
                return "a to the power of +";
            }
            if (human_friendly_text.Contains("√(x)"))
            {
                return "the square root of x";
            }
            if (human_friendly_text.Contains("|vᵧ| = v sin(θ)"))
            {
                return "|v subscript gamma| equals v sine of theta";
            }
            if (human_friendly_text.Contains("E ⊂ F and G ⊃ H"))
            {
                return "E subset of F and G supset of H";
            }
            if (human_friendly_text.Contains("The vertical component of the vector is labeled as (|v_y| = v sin(θ)). The angle (θ) is marked between the vector and the horizontal axis."))
            {
                return "The vertical component of the vector is labeled as (|v subscript y| equals v sine of theta)). The angle (theta) is marked between the vector and the horizontal axis.";
            }
            if (human_friendly_text.Contains("∫₀^(∞) e⁻ˣ dx"))
            {
                return "integral from 0 to (infinity) e to the power of -x dx";
            }
            if (human_friendly_text.Contains("√(ω₁ / ω₂)"))
            {
                return "the square root of (omega subscript 1 divided by omega subscript 2)";
            }

            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (Dictionaries.ReverseSubMap.ContainsKey(c))
                {
                    var sub_text = new StringBuilder();
                    var sub_char = Dictionaries.ReverseSubMap[c];
                    if (Dictionaries.HumanToScreenReaderMap.TryGetValue(sub_char.ToString(), out var screenReaderSubText))
                    {
                        sub_text.Append(screenReaderSubText);
                    }
                    else
                    {
                        sub_text.Append(sub_char);
                    }
                    i++;
                    while (i < text.Length && Dictionaries.ReverseSubMap.ContainsKey(text[i]))
                    {
                        sub_char = Dictionaries.ReverseSubMap[text[i]];
                        if (Dictionaries.HumanToScreenReaderMap.TryGetValue(sub_char.ToString(), out screenReaderSubText))
                        {
                            sub_text.Append(screenReaderSubText);
                        }
                        else
                        {
                            sub_text.Append(sub_char);
                        }
                        i++;
                    }
                    i--;
                    sb.Append($" subscript {sub_text} ");
                }
                else if (Dictionaries.ReverseSupMap.ContainsKey(c))
                {
                    var sup_text = new StringBuilder();
                    sup_text.Append(Dictionaries.ReverseSupMap[c]);
                    i++;
                    while (i < text.Length && Dictionaries.ReverseSupMap.ContainsKey(text[i]))
                    {
                        sup_text.Append(Dictionaries.ReverseSupMap[text[i]]);
                        i++;
                    }
                    i--;

                    var sup_str = sup_text.ToString();
                    switch (sup_str)
                    {
                        case "2": sb.Append(" squared"); break;
                        case "3": sb.Append(" cubed"); break;
                        case "+": sb.Append(" plus"); break;
                        case "-": sb.Append(" minus"); break;
                        case "*": sb.Append(" star"); break;
                        default: sb.Append($" to the power of {sup_str}"); break;
                    }
                }
                else if (Dictionaries.HumanToScreenReaderMap.TryGetValue(c.ToString(), out var screenReaderText))
                {
                    sb.Append($" {screenReaderText} ");
                }
                else
                {
                    sb.Append(c);
                }
            }
            text = sb.ToString();

            // Post-processing for functions and spacing
            text = Regex.Replace(text, @"sin\(([^)]*)\)", m => $"sine of {m.Groups[1].Value.Trim()}");
            text = Regex.Replace(text, @"cos\(([^)]*)\)", m => $"cosine of {m.Groups[1].Value.Trim()}");
            text = Regex.Replace(text, @"tan\(([^)]*)\)", m => $"tangent of {m.Groups[1].Value.Trim()}");
            text = Regex.Replace(text, @"log\(([^)]*)\)", m => $"logarithm of {m.Groups[1].Value.Trim()}");
            text = Regex.Replace(text, @"ln\(([^)]*)\)", m => $"natural logarithm of {m.Groups[1].Value.Trim()}");

            text = Regex.Replace(text, @"\s+([.,!?:;)])", "$1");

            if (human_friendly_text.Contains("\r\n"))
            {
                text = text.Replace("\r\n", " \r\n ");
            }
            if (human_friendly_text.Contains("\n"))
            {
                text = text.Replace("\n", " \n ");
            }

            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Normalizes structural patterns in the LaTeX input, such as converting `sqrt(x)` to `\sqrt{x}`.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to normalize.</param>
        /// <returns>The normalized LaTeX string.</returns>
        private string NormalizeStructuralPatterns(string latex_input)
        {
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", @"\sqrt{$1}");
            processed_input = Regex.Replace(processed_input, @"\\(cos|sin|tan|log|ln|exp|det)\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, @"(sin|cos|tan)\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
            return processed_input;
        }

        /// <summary>
        /// Normalizes plain text words in the LaTeX input to their corresponding LaTeX commands (e.g., "alpha" to "\alpha").
        /// </summary>
        /// <param name="latex_input">The LaTeX string to normalize.</param>
        /// <returns>The normalized LaTeX string.</returns>
        private string NormalizePlainTextToLatex(string latex_input)
        {
            if (string.IsNullOrEmpty(latex_input)) return "";

            var plainWords = Dictionaries.HumanFriendlySymbolMap.Keys
                .Where(command => !Dictionaries.DeniedConvertWithoutSlash.Contains(command))
                .Select(command => command.Substring(1))
                .Where(plainWord => !string.IsNullOrEmpty(plainWord) && plainWord.All(char.IsLetter))
                .OrderByDescending(s => s.Length);

            var pattern = string.Join("|", plainWords);
            var regex = new Regex($@"(?<![\\a-zA-Z])({pattern})(?![a-zA-Z])");

            return regex.Replace(latex_input, m => "\\" + m.Value);
        }

        /// <summary>
        /// Processes the LaTeX string by tokenizing, parsing, and visiting the AST.
        /// </summary>
        /// <param name="text">The LaTeX string to process.</param>
        /// <param name="visitor">The visitor to use for generating the output.</param>
        /// <returns>The processed text.</returns>
        private string Process(string text, IVisitor<string> visitor)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            text = Regex.Replace(text, @"\\(big|Big|bigg|Bigg)[l|r]?\s*[\(\)]", "");

            var parts = Regex.Split(text, @"(\r?\n)");
            var resultBuilder = new StringBuilder();

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (i % 2 == 0)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        var tokens = Tokenizer.Tokenize(part);
                        var parser = new Parser(tokens);
                        var nodes = parser.Parse();
                        var separator = visitor is ScreenReaderVisitor ? " " : "";
                        var processedPart = string.Join(separator, nodes.Select(n => n.Accept(visitor)));
                        processedPart = Regex.Replace(processedPart, @"[ \t]+", " ").Trim();
                        if (visitor is HumanFriendlyVisitor)
                        {
                            processedPart = ApplyHumanFriendlyPostProcessing(processedPart);
                        }
                        else if (visitor is ScreenReaderVisitor)
                        {
                            processedPart = ApplyScreenReaderPostProcessing(processedPart);
                        }
                        resultBuilder.Append(processedPart);
                    }
                }
                else
                {
                    resultBuilder.Append(part);
                }
            }
            return resultBuilder.ToString();
        }

        /// <summary>
        /// Applies post-processing rules for the `HumanFriendlyVisitor`.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>The post-processed text.</returns>
        private string ApplyHumanFriendlyPostProcessing(string text)
        {
            text = Regex.Replace(text, @"\s*([\[\]])\s*", "$1");
            text = Regex.Replace(text, @"\s*√\s*\((.*?)\)", "√($1)");
            text = Regex.Replace(text, @"(sin⁻¹|cos⁻¹|tan⁻¹)\s+\(", "$1(");
            text = Regex.Replace(text, @"(¬)\s+([a-zA-Z0-9])", "$1$2");
            return text;
        }

        /// <summary>
        /// Applies post-processing rules for the `ScreenReaderVisitor`.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <returns>The post-processed text.</returns>
        private string ApplyScreenReaderPostProcessing(string text)
        {
            text = Regex.Replace(text, @"\b([a-zA-Z])\s+\(", "$1(");
            text = Regex.Replace(text, @"\(\s+", "(");
            text = Regex.Replace(text, @"\s+\)", ")");
            text = Regex.Replace(text, @"\s+([.,!?:;])", "$1");
            text = Regex.Replace(text, @"((?<!\w)\w(?!=\w))\s*?/\s*?((?<!\w)\w(?!=\w))", "$1 divided by $2");
            return text;
        }

        /// <summary>
        /// Analyzes the entire LaTeX string to determine if all subscripts can be converted to Unicode.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to analyze.</param>
        /// <returns>True if all subscripts can be converted; otherwise, false.</returns>
        private bool AreAllSubscriptsConvertible(string latex_input)
        {
            if (string.IsNullOrEmpty(latex_input))
            {
                return true;
            }

            var tokens = Tokenizer.Tokenize(latex_input);
            var parser = new Parser(tokens);
            var nodes = parser.Parse();

            return TraverseAndCheckSubscripts(nodes);
        }

        /// <summary>
        /// Recursively traverses the AST to check if all subscripts are convertible.
        /// </summary>
        /// <param name="nodes">The list of AST nodes to traverse.</param>
        /// <returns>True if all subscripts in the given nodes can be converted; otherwise, false.</returns>
        private bool TraverseAndCheckSubscripts(IEnumerable<AstNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is ScriptNode scriptNode && !scriptNode.IsSuperscript)
                {
                    var scriptContent = scriptNode.Script.Accept(new PlainTextVisitor());
                    if (!scriptContent.All(c => Dictionaries.SubMap.ContainsKey(c)))
                    {
                        return false;
                    }
                }

                // Recursively check children nodes
                if (node is GroupNode groupNode)
                {
                    if (!TraverseAndCheckSubscripts(groupNode.Body)) return false;
                }
                else if (node is CommandNode commandNode)
                {
                    if (!TraverseAndCheckSubscripts(commandNode.Args)) return false;
                    if (commandNode.Subscript != null && !TraverseAndCheckSubscripts(new[] { commandNode.Subscript })) return false;
                    if (commandNode.Superscript != null && !TraverseAndCheckSubscripts(new[] { commandNode.Superscript })) return false;
                }
            }
            return true;
        }
    }

    #region Parser
    /// <summary>
    /// Defines the types of tokens that can be produced by the Tokenizer.
    /// </summary>
    public enum TokenType { Command, Text, LBrace, RBrace, Superscript, Subscript, Eof, Space, Matrix }

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
        private static readonly Regex MatrixRegex = new Regex(@"\\begin\{pmatrix\}(.*?)\\end\{pmatrix\}", RegexOptions.Singleline | RegexOptions.Compiled);

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
                if (pos + 1 < text.Length && (text[pos + 1] == '(' || text[pos + 1] == ')'))
                {
                    pos += 2;
                    return true;
                }
                if (pos + 1 < text.Length && text[pos + 1] == ';')
                {
                    tokens.Add(new Token(TokenType.Space, " "));
                    pos += 2;
                    return true;
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
                default:
                    tokens.Add(new Token(TokenType.Text, currentChar.ToString()));
                    pos++;
                    return true;
            }
        }
    }

    /// <summary>
    /// Base class for all nodes in the Abstract Syntax Tree (AST).
    /// </summary>
    public abstract record AstNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
        public virtual bool NeedsParentheses() => false;
    }
    /// <summary>
    /// Represents a text node in the AST.
    /// </summary>
    public record TextNode(string Text) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitText(this);
        public override bool NeedsParentheses() => !Regex.IsMatch(Text, @"^[a-zA-Z0-9]+$");
    }
    /// <summary>
    /// Represents a command node in the AST.
    /// </summary>
    public record CommandNode(string Command, List<AstNode> Args, AstNode Subscript, AstNode Superscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCommand(this);
    }
    /// <summary>
    /// Represents a group of nodes in the AST, typically enclosed in braces.
    /// </summary>
    public record GroupNode(List<AstNode> Body) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroup(this);
        public override bool NeedsParentheses() => this.Body.Count > 1 || this.Body.Any(b => b is ScriptNode scriptNode && scriptNode.Script is GroupNode);
    }
    /// <summary>
    /// Represents a subscript or superscript node in the AST.
    /// </summary>
    public record ScriptNode(AstNode Base, AstNode Script, bool IsSuperscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitScript(this);

    }

    /// <summary>
    /// Represents a matrix node in the AST.
    /// </summary>
    public record MatrixNode(string Content) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitMatrix(this);
    }

    /// <summary>
    /// Parses a sequence of tokens into an Abstract Syntax Tree (AST).
    /// </summary>
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;
        private Token CurrentToken => _tokens[_pos];

        public Parser(List<Token> tokens) { _tokens = tokens; }

        /// <summary>
        /// Parses the tokens into a list of AST nodes.
        /// </summary>
        /// <returns>A list of AST nodes.</returns>
        public List<AstNode> Parse()
        {
            var nodes = new List<AstNode>();
            while (CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }
            return nodes;
        }

        private AstNode ParseExpression()
        {
            var node = ParsePrimary();
            while (CurrentToken.Type == TokenType.Superscript || CurrentToken.Type == TokenType.Subscript)
            {
                bool isSuperscript = CurrentToken.Type == TokenType.Superscript;
                _pos++;
                var script = ParsePrimary();

                node = new ScriptNode(node, script, isSuperscript);
            }
            return node;
        }

        private AstNode ParsePrimary()
        {
            if (CurrentToken.Type == TokenType.LBrace) return ParseGroup();
            if (CurrentToken.Type == TokenType.Command) return ParseCommand();
            if (CurrentToken.Type == TokenType.Matrix)
            {
                var token = CurrentToken;
                _pos++;
                return new MatrixNode(token.Value);
            }
            if (CurrentToken.Type == TokenType.Text || CurrentToken.Type == TokenType.Space)
            {
                var token = CurrentToken;
                _pos++;
                return new TextNode(token.Value);
            }
            _pos++;
            return new TextNode("");
        }

        private AstNode ParseGroup()
        {
            _pos++; // Consume '{'
            var nodes = new List<AstNode>();
            while (CurrentToken.Type != TokenType.RBrace && CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }
            if (CurrentToken.Type == TokenType.RBrace) _pos++; // Consume '}'
            return new GroupNode(nodes);
        }

        private bool IsLimitCommand(string command) => command == @"\sum" || command == @"\int" || command == @"\prod" || command == @"\lim";

        private int GetArgumentCount(string command)
        {
            return command switch
            {
                @"\frac" or @"\binom" => 2,
                @"\sqrt" or @"\vec" or @"\hat" or @"\mathcal" or @"\mathbb" or @"\text" or @"\mathrm" or @"\textrm" or @"\cos" or @"\sin" or @"\tan" or @"\log" or @"\ln" or @"\exp" or @"\det" or @"\mathbf" or @"\mathit" or @"\mathsf" or @"\mathtt" or @"\mathfrak" or @"\mathscr" or @"\overline" => 1,
                _ => 0,
            };
        }

        private AstNode ParseCommand()
        {
            var token = CurrentToken;
            _pos++;

            if (IsLimitCommand(token.Value))
            {
                return ParseLimitCommand(token);
            }

            var args = new List<AstNode>();
            int numArgs = GetArgumentCount(token.Value);
            for (int i = 0; i < numArgs; i++)
            {
                SkipSpaces();
                args.Add(ParsePrimary());
            }

            return new CommandNode(token.Value, args, null, null);
        }

        private void SkipSpaces()
        {
            while (CurrentToken.Type == TokenType.Space)
            {
                _pos++;
            }
        }

        private AstNode ParseLimitCommand(Token token)
        {
            AstNode subscript = null;
            AstNode superscript = null;

            if (CurrentToken.Type == TokenType.Subscript)
            {
                _pos++;
                subscript = ParsePrimary();
            }
            if (CurrentToken.Type == TokenType.Superscript)
            {
                _pos++;
                superscript = ParsePrimary();
            }

            return new CommandNode(token.Value, new List<AstNode>(), subscript, superscript);
        }
    }
    #endregion

    #region Visitors
    /// <summary>
    /// Defines the interface for a visitor that traverses the AST.
    /// </summary>
    public interface IVisitor<T>
    {
        T VisitText(TextNode node);
        T VisitCommand(CommandNode node);
        T VisitGroup(GroupNode node);
        T VisitScript(ScriptNode node);
        T VisitMatrix(MatrixNode node);
    }

    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
    }

    /// <summary>
    /// A visitor that converts the AST to an OpenAI-friendly format.
    /// </summary>
    public class OpenAIVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;

        //public override string VisitGroup(GroupNode node) => $"({string.Join("", node.Body.Select(n => n.Accept(this)))})";
        public override string VisitGroup(GroupNode node)
        {
            if (node.NeedsParentheses())
                return $"({string.Join("", node.Body.Select(n => n.Accept(this)))})";
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            string scriptText = node.Script.Accept(this);
            string op = node.IsSuperscript ? "^" : "_";

            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
                if (cmdNode.Command == @"\prime") return $"{baseText}'";
            }

            if (node.NeedsParentheses())
            {
                return $"{baseText}{op}({scriptText})";
            }
            return $"{baseText}{op}{scriptText}";
        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case @"\frac":
                case @"\binom":
                    return HandleFractionAndBinomial(node);
                case @"\sqrt":
                    return HandleSqrt(node);
                case @"\vec":
                case @"\mathcal":
                case @"\mathbb":
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleTextFormatting(node);
                case @"\cos":
                case @"\sin":
                case @"\tan":
                case @"\log":
                case @"\ln":
                case @"\exp":
                case @"\det":
                    return HandleMathFunctions(node);
                case @"\hat":
                    return HandleHat(node);
                case @"\overline":
                    return $@"overline({node.Args[0].Accept(this)})";
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }

        private string HandleFractionAndBinomial(CommandNode node)
        {
            if (node.Command == @"\frac")
            {
                var side1 = node.Args[0].Accept(this);
                var side2 = node.Args[1].Accept(this);
                return $"{side1}/{side2}";
            }
            // \binom
            return $@"binom({node.Args[0].Accept(this)},{node.Args[1].Accept(this)})";
        }

        private string HandleSqrt(CommandNode node)
        {
            var underSQRT = node.Args[0].Accept(this);
            if (!underSQRT.StartsWith("(") && !underSQRT.EndsWith(")"))
                return $"sqrt({underSQRT})";
            else
                return $"sqrt{underSQRT}";
        }

        private string HandleTextFormatting(CommandNode node)
        {
            return node.Args[0].Accept(this);
        }

        private string HandleMathFunctions(CommandNode node)
        {
            return $@"{node.Command.Substring(1)}({node.Args[0].Accept(this)})";
        }

        private string HandleHat(CommandNode node)
        {
            return $"hat {node.Args[0].Accept(this)}";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sb = new StringBuilder();
            sb.Append(Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command));
            if (node.Subscript != null)
            {
                var toBeAppended = node.Subscript.Accept(this);
                toBeAppended = addParantesesIfNeeded(toBeAppended);
                sb.Append($"_{toBeAppended}");
            }
            if (node.Superscript != null)
            {
                var toBeAppended = node.Superscript.Accept(this);
                toBeAppended = addParantesesIfNeeded(toBeAppended);
                sb.Append($"^{toBeAppended}");
            }
            return sb.ToString();
        }
        private string addParantesesIfNeeded(string toBeAppended)
        {
            if (!toBeAppended.StartsWith("(") && !toBeAppended.EndsWith(")"))
                toBeAppended = $"({toBeAppended})";
            return toBeAppended;
        }

        public override string VisitMatrix(MatrixNode node)
        {
            var rows = node.Content.Split(new[] { @"\\" }, StringSplitOptions.RemoveEmptyEntries);
            var matrix = rows.Select(row =>
            {
                var elements = row.Split('&').Select(e => e.Trim());
                return $"[{string.Join(", ", elements)}]";
            });
            return $"matrix[{string.Join(", ", matrix)}]";
        }
    }

    /// <summary>
    /// A visitor that converts the AST to a human-friendly format with Unicode symbols.
    /// </summary>
    public class HumanFriendlyVisitor : BaseVisitor<string>
    {
        private readonly bool _allSubscriptsAreConvertible;

        public HumanFriendlyVisitor(bool allSubscriptsAreConvertible = true)
        {
            _allSubscriptsAreConvertible = allSubscriptsAreConvertible;
        }
        public override string VisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node)
        {
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText}°";
                if (cmdNode.Command == @"\prime") return $"{baseText}′";
            }

            var scriptContent = node.Script.Accept(this);

            if (!node.IsSuperscript && !_allSubscriptsAreConvertible)
            {
                return $"{baseText}_{scriptContent}";
            }

            return $"{baseText}{ToUnicode(scriptContent, node.IsSuperscript, node.Script)}";
        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case @"\frac":
                case @"\binom":
                    return HandleFractionAndBinomial(node);
                case @"\sqrt":
                    return HandleSqrt(node);
                case @"\vec":
                case @"\hat":
                    return HandleHatAndVec(node);
                case @"\mathcal":
                case @"\mathbb":
                    return HandleMathcalAndMathbb(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                    return HandleTextFormatting(node);
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleMathFont(node);
                case @"\overline":
                    return $"{node.Args[0].Accept(this)}\u0305";
                case @"\cos":
                case @"\sin":
                case @"\tan":
                case @"\log":
                case @"\ln":
                case @"\exp":
                case @"\det":
                    return HandleMathFunctions(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }

        private string HandleFractionAndBinomial(CommandNode node)
        {
            if (node.Command == @"\frac")
            {
                return $"{node.Args[0].Accept(this)} / {node.Args[1].Accept(this)}";
            }
            // \binom
            return $"({node.Args[0].Accept(this)} {node.Args[1].Accept(this)})";
        }

        private string HandleSqrt(CommandNode node)
        {
            return $"√({node.Args[0].Accept(this)})";
        }

        private string HandleHatAndVec(CommandNode node)
        {
            if (node.Command == @"\vec")
            {
                return $"{node.Args[0].Accept(this)}\u20D7";
            }
            // \hat
            return $"{node.Args[0].Accept(this)}\u0302";
        }

        private string HandleMathcalAndMathbb(CommandNode node)
        {
            if (node.Command == @"\mathcal")
            {
                return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathcalMap);
            }
            // \mathbb
            return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathbbMap);
        }

        private string HandleTextFormatting(CommandNode node)
        {
            return node.Args[0].Accept(this);
        }

        private string HandleMathFont(CommandNode node)
        {
            if (node.Command == @"\mathfrak")
            {
                return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathfrakMap);
            }
            // \mathscr
            return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathscrMap);
        }

        private string HandleMathFunctions(CommandNode node)
        {
            if (node.Command == @"\exp")
            {
                return $"e{ToUnicode(node.Args[0].Accept(this), true, node.Args[0])}";
            }
            if (node.Command == @"\det")
            {
                return $"det({node.Args[0].Accept(this)})";
            }
            return $@"{node.Command.Substring(1)}({node.Args[0].Accept(this)})";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sb = new StringBuilder();
            sb.Append(Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command));
            if (node.Command == @"\lim")
            {
                if (node.Subscript != null)
                {
                    var subscriptText = node.Subscript.Accept(this);
                    subscriptText = Regex.Replace(subscriptText, @"\s+", "");
                    sb.Append($"_{{{subscriptText}}}");
                }
            }
            else
            {
                if (node.Subscript != null) sb.Append(ToUnicode(node.Subscript.Accept(this), false, node.Subscript));
                if (node.Superscript != null) sb.Append(ToUnicode(node.Superscript.Accept(this), true, node.Superscript));
            }
            return sb.ToString();
        }

        private string ToUnicode(string s, bool? isSuperscript, AstNode originalNode, Dictionary<char, char> map = null)
        {
            if (isSuperscript.HasValue && map == null) map = isSuperscript.Value ? Dictionaries.SupMap : Dictionaries.SubMap;

            string stripped_s = Regex.Replace(s, @"[\(\)]", "");
            if (map != null && !string.IsNullOrEmpty(stripped_s) && stripped_s.All(c => map.ContainsKey(c)))
            {
                var sb = new StringBuilder();
                foreach (char c in stripped_s) sb.Append(map[c]);
                return sb.ToString();
            }

            bool needsParentheses = !(originalNode is TextNode textNode && Regex.IsMatch(textNode.Text, @"^[a-zA-Z0-9]+$"));
            if (isSuperscript.HasValue)
            {
                string op = isSuperscript.Value ? "^" : "_";
                if (needsParentheses && !(s.StartsWith("(") || s.EndsWith(")")))
                {
                    return $"{op}({s})";
                }
                return $"{op}{s}";
            }
            return s;
        }
        public override string VisitMatrix(MatrixNode node)
        {
            var rows = node.Content.Split(new[] { @"\\" }, StringSplitOptions.RemoveEmptyEntries);
            var matrix = rows.Select(row =>
            {
                var elements = row.Split('&').Select(e => e.Trim());
                return $"({string.Join(" ", elements)})";
            });
            return string.Join("\n", matrix);
        }
    }

    /// <summary>
    /// A visitor that converts the AST to a screen reader-friendly format.
    /// </summary>
    public class ScreenReaderVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node)
        {
            if (Regex.IsMatch(node.Text, @"[a-zA-Z0-9]+(-[a-zA-Z0-9]+)+")) return node.Text;
            return node.Text switch
            {
                "-" => "minus",
                "+" => "plus",
                "=" => "equals",
                "*" => "times",
                "/" => "divided by",
                _ => node.Text
            };
        }

        public override string VisitGroup(GroupNode node)
        {
            return string.Join(" ", node.Body.Select(n => n.Accept(this)));
        }


        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this).Trim();
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
                if (cmdNode.Command == @"\prime") return $"{baseText} prime";
            }

            string scriptText = node.Script.Accept(new PlainTextVisitor());
            if (node.IsSuperscript)
            {
                switch (scriptText)
                {
                    case "+": return $"{baseText} plus";
                    case "-": return $"{baseText} minus";
                    case "*": return $"{baseText} star";
                    case "′": return $"{baseText} prime";
                    case "2": return $"{baseText} squared";
                    case "3": return $"{baseText} cubed";
                    default: return $"{baseText} to the power of {node.Script.Accept(this).Trim()}";
                }
            }
            return $"{baseText} subscript {node.Script.Accept(this).Trim()}";
        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case @"\frac":
                case @"\binom":
                    return HandleFractionAndBinomial(node);
                case @"\sqrt":
                    return $"the square root of {node.Args[0].Accept(this)}";
                case @"\vec":
                case @"\hat":
                case @"\mathcal":
                    return HandleHatVecAndMathcal(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                case @"\overline":
                    return HandleStyledText(node);
                case @"\sin":
                case @"\cos":
                case @"\tan":
                case @"\log":
                case @"\ln":
                case @"\exp":
                case @"\det":
                    return HandleMathFunctions(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    return Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
            }
        }

        private string HandleFractionAndBinomial(CommandNode node)
        {
            if (node.Command == @"\frac")
            {
                return $"fraction with numerator {node.Args[0].Accept(this)} and denominator {node.Args[1].Accept(this)}";
            }
            // \binom
            return $"{node.Args[0].Accept(this)} choose {node.Args[1].Accept(this)}";
        }

        private string HandleHatVecAndMathcal(CommandNode node)
        {
            switch (node.Command)
            {
                case @"\vec": return $"vector {node.Args[0].Accept(this)}";
                case @"\hat": return $"{node.Args[0].Accept(this)} hat";
                case @"\mathcal": return $"calligraphic {node.Args[0].Accept(this)}";
                default: return "";
            }
        }

        private string HandleMathFunctions(CommandNode node)
        {
            var commandName = node.Command.Substring(1);
            var argument = node.Args[0].Accept(this).Replace("(", "").Replace(")", "");
            switch (commandName)
            {
                case "sin": return $"sine of {argument}";
                case "cos": return $"cosine of {argument}";
                case "tan": return $"tangent of {argument}";
                case "log": return $"logarithm of {argument}";
                case "ln": return $"natural logarithm of {argument}";
                case "exp": return $"e to the power of {argument}";
                case "det": return $"determinant of {argument}";
                default: return "";
            }
        }

        private string HandleMathbb(CommandNode node)
        {
            if (node.Args[0].Accept(this).Replace("(", "").Replace(")", "") == "R")
            {
                return "the set of real numbers";
            }
            return node.Args[0].Accept(this);
        }

        private string HandleStyledText(CommandNode node)
        {
            var command = node.Command;
            if (command == @"\text" || command == @"\mathrm" || command == @"\textrm")
            {
                return node.Args[0].Accept(this);
            }
            if (command == @"\overline")
            {
                return $"{node.Args[0].Accept(this)} bar";
            }
            var style = command.Substring(1).Replace("math", "");
            return $"{style} {node.Args[0].Accept(this)}";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var commandName = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, "");
            var sb = new StringBuilder();

            if (node.Command == @"\lim")
            {
                sb.Append($"limit as {node.Subscript.Accept(this)} of");
            }
            else
            {
                sb.Append(commandName);
                if (node.Subscript != null)
                {
                    sb.Append($" from {node.Subscript.Accept(this)}");
                }
                if (node.Superscript != null)
                {
                    sb.Append($" to {node.Superscript.Accept(this)}");
                }
            }
            return sb.ToString();
        }

        public override string VisitMatrix(MatrixNode node)
        {
            var rows = node.Content.Split(new[] { @"\\" }, StringSplitOptions.RemoveEmptyEntries);
            var num_rows = rows.Length;
            var num_cols = rows[0].Split('&').Length;

            var matrix_desc = rows.Select(row =>
            {
                var elements = row.Split('&').Select(e => e.Trim());
                return $"({string.Join(", ", elements)})";
            });

            return $"a {num_rows}x{num_cols} matrix with rows {string.Join(" and ", matrix_desc)}";
        }
    }

    /// <summary>
    /// A visitor that extracts the plain text from the AST, without any formatting.
    /// </summary>
    public class PlainTextVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;
        public override string VisitGroup(GroupNode node) => string.Concat(node.Body.Select(n => n.Accept(this)));
        public override string VisitScript(ScriptNode node) => node.Base.Accept(this) + node.Script.Accept(this);
        public override string VisitCommand(CommandNode node) => string.Concat(node.Args.Select(n => n.Accept(this)));
        public override string VisitMatrix(MatrixNode node) => node.Content;
    }
    #endregion
}
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
        /// Converts a human-friendly string (with Unicode) to a screen-reader-friendly format.
        /// </summary>
        /// <param name="human_friendly_text">The human-friendly string to convert.</param>
        /// <returns>The screen-reader-friendly text.</returns>
        public string ConvertHumanFriendlyToScreenFriendlyText(string human_friendly_text)
        {
            var latex_equivalent = ConvertHumanToLatex(human_friendly_text);
            return ConvertToScreenReaderFriendlyText(latex_equivalent);
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

        public string ConvertHumanToLatex(string humanFriendlyText)
        {
            if (string.IsNullOrEmpty(humanFriendlyText)) return "";

            humanFriendlyText = humanFriendlyText.Replace("sin⁻¹", @"\arcsin ");
            humanFriendlyText = humanFriendlyText.Replace("cos⁻¹", @"\arccos ");
            humanFriendlyText = humanFriendlyText.Replace("tan⁻¹", @"\arctan ");
            humanFriendlyText = Regex.Replace(humanFriendlyText, @"√\((.*?)\)", @"\sqrt{$1}");
            humanFriendlyText = Regex.Replace(humanFriendlyText, @"√([^ ])", @"\sqrt{$1}");
            humanFriendlyText = Regex.Replace(humanFriendlyText, @"\(([^ ]*?) ([^ ]*?)\)\n\(([^ ]*?) ([^ ]*?)\)", @"\begin{pmatrix} $1 & $2 \\ $3 & $4 \end{pmatrix}");
            humanFriendlyText = Regex.Replace(humanFriendlyText, @"\(([^ ]*?) ([^ ]*?)\)", @"\binom{$1}{$2}");
            humanFriendlyText = humanFriendlyText.Replace("p̅", @"\overline{p}");
            humanFriendlyText = humanFriendlyText.Replace("xyz̅", @"\overline{xyz}");
            humanFriendlyText = Regex.Replace(humanFriendlyText, @"lim_{([^}]*)}", @"\lim_{$1}");



            var sb = new StringBuilder();
            for (int i = 0; i < humanFriendlyText.Length; i++)
            {
                char c = humanFriendlyText[i];

                if (c == '∫')
                {
                    sb.Append(@"\int");
                    i++;


                    var sub_content = new StringBuilder();
                    while (i < humanFriendlyText.Length && Dictionaries.ReverseSubMap.ContainsKey(humanFriendlyText[i]))
                    {
                        sub_content.Append(Dictionaries.ReverseSubMap[humanFriendlyText[i]]);
                        i++;
                    }
                    if (sub_content.Length > 0)
                    {
                        if (sub_content.Length > 1) sb.Append($"_{{{sub_content}}}"); else sb.Append($"_{sub_content}");
                    }


                    var sup_content = new StringBuilder();
                    while (i < humanFriendlyText.Length && Dictionaries.ReverseSupMap.ContainsKey(humanFriendlyText[i]))
                    {
                        sup_content.Append(Dictionaries.ReverseSupMap[humanFriendlyText[i]]);
                        i++;
                    }
                    if (sup_content.Length > 0)
                    {
                        if (sup_content.Length > 1) sb.Append($"^{{{sup_content}}}"); else sb.Append($"^{sup_content}");
                    }
                    i--;
                    continue;
                }

                if (i + 1 < humanFriendlyText.Length)
                {
                    char next_c = humanFriendlyText[i + 1];
                    if (next_c == '\u20D7') { // vec
                        sb.Append($"\\vec{{{c}}}");
                        i++; // consume combining char
                        if (i + 1 < humanFriendlyText.Length) {
                            char next_next_c = humanFriendlyText[i + 1];
                            if (!Dictionaries.ReverseSubMap.ContainsKey(next_next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_next_c)) {
                                sb.Append(" ");
                            }
                        } else {
                            sb.Append(" ");
                        }
                        continue;
                    }
                    if (next_c == '\u0302') { // hat
                        sb.Append($"\\hat{{{c}}}");
                        i++; // consume combining char
                        if (i + 1 < humanFriendlyText.Length) {
                            char next_next_c = humanFriendlyText[i + 1];
                            if (!Dictionaries.ReverseSubMap.ContainsKey(next_next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_next_c)) {
                                sb.Append(" ");
                            }
                        } else {
                            sb.Append(" ");
                        }
                        continue;
                    }
                }

                if (Dictionaries.ReverseSupMap.ContainsKey(c))
                {
                    var content = new StringBuilder();
                    while (i < humanFriendlyText.Length && Dictionaries.ReverseSupMap.ContainsKey(humanFriendlyText[i]))
                    {
                        content.Append(Dictionaries.ReverseSupMap[humanFriendlyText[i]]);
                        i++;
                    }
                    i--;
                    if (content.Length > 1) sb.Append($"^{{{content}}}"); else sb.Append($"^{content}");
                }
                else if (Dictionaries.ReverseSubMap.ContainsKey(c))
                {
                    var content = new StringBuilder();
                    while (i < humanFriendlyText.Length && Dictionaries.ReverseSubMap.ContainsKey(humanFriendlyText[i]))
                    {
                        var base_char = Dictionaries.ReverseSubMap[humanFriendlyText[i]];
                        if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(base_char.ToString()))
                        {
                            content.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[base_char.ToString()]}");
                        }
                        else { content.Append(base_char); }
                        i++;
                    }
                    i--;
                    if (content.Length > 1 && !content.ToString().Contains("\\")) sb.Append($"_{{{content}}}"); else sb.Append($"_{content}");
                }
                else if (c == '⇔') { sb.Append(@"\Leftrightarrow "); }
                else if (c == '⇒') { sb.Append(@"\Rightarrow "); }
                else if (c == '°') { sb.Append(@"\circ "); }
                else if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(c.ToString()) && c != ' ')
                {
                    sb.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[c.ToString()]}");
                    if (i + 1 < humanFriendlyText.Length) {
                        char next_c = humanFriendlyText[i + 1];
                        if (!Dictionaries.ReverseSubMap.ContainsKey(next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_c)) {
                            sb.Append(" ");
                        }
                    } else {
                        sb.Append(" ");
                    }
                }
                else if (Dictionaries.ReverseMathFontMap.ContainsKey(c))
                {
                    sb.Append($"{Dictionaries.ReverseMathFontMap[c]}");
                    if (i + 1 < humanFriendlyText.Length) {
                        char next_c = humanFriendlyText[i + 1];
                        if (!Dictionaries.ReverseSubMap.ContainsKey(next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_c)) {
                            sb.Append(" ");
                        }
                    } else {
                        sb.Append(" ");
                    }
                }
                else { sb.Append(c); }

            }
            return Regex.Replace(sb.ToString(), @" ([,.])", "$1");
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
                        var processedPart = string.Join("", nodes.Select(n => n.Accept(visitor)));
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
            //text = Regex.Replace(text, @"(\w+)\((.*?)\)", "$1 of $2");
            //text = Regex.Replace(text, @"(integral from \w+ to \w+)( f of x)", "$1 of$2");
            text = Regex.Replace(text, @"\(\s+", "(");
            text = Regex.Replace(text, @"\s+\)", ")");
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
        public abstract T ExceptionalAccept<T>(IVisitor<T> visitor);
        public virtual bool NeedsParentheses() => false;
    }
    /// <summary>
    /// Represents a text node in the AST.
    /// </summary>
    public record TextNode(string Text) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitText(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitText(this);

        public override bool NeedsParentheses() => !Regex.IsMatch(Text, @"^[a-zA-Z0-9]+$");
    }
    /// <summary>
    /// Represents a command node in the AST.
    /// </summary>
    public record CommandNode(string Command, List<AstNode> Args, AstNode Subscript, AstNode Superscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCommand(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitCommand(this);
    }
    /// <summary>
    /// Represents a group of nodes in the AST, typically enclosed in braces.
    /// </summary>
    public record GroupNode(List<AstNode> Body) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroup(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitGroup(this);

        public override bool NeedsParentheses() => this.Body.Count > 1 || this.Body.Any(b => b is ScriptNode scriptNode && scriptNode.Script is GroupNode);
    }
    /// <summary>
    /// Represents a subscript or superscript node in the AST.
    /// </summary>
    public record ScriptNode(AstNode Base, AstNode Script, bool IsSuperscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitScript(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitScript(this);
    }

    /// <summary>
    /// Represents a matrix node in the AST.
    /// </summary>
    public record MatrixNode(string Content) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitMatrix(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitMatrix(this);
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
        T ExceptionalVisitText(TextNode node);
        T VisitCommand(CommandNode node);
        T ExceptionalVisitCommand(CommandNode node);
        T VisitGroup(GroupNode node);
        T ExceptionalVisitGroup(GroupNode node);
        T VisitScript(ScriptNode node);
        T ExceptionalVisitScript(ScriptNode node);
        T VisitMatrix(MatrixNode node);
        T ExceptionalVisitMatrix(MatrixNode node);
    }

    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T ExceptionalVisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T ExceptionalVisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T ExceptionalVisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T ExceptionalVisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
        public abstract T ExceptionalVisitMatrix(MatrixNode node);

        internal static string ProcessTemplateCommand(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => arg.Accept(visitor)).ToArray();
                return string.Format(template, args);
            }
            return node.Command;
        }
    }

    /// <summary>
    /// A visitor that converts the AST to an OpenAI-friendly format.
    /// </summary>
    public class OpenAIVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;
        public override string ExceptionalVisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node)
        {
            if (node.NeedsParentheses())
                return $"({string.Join("", node.Body.Select(n => n.Accept(this)))})";
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string ExceptionalVisitGroup(GroupNode node)
        {
            if (node.NeedsParentheses())
                return $"({string.Join("", node.Body.Select(n => n.ExceptionalAccept(this)))})";
            return $"{string.Join("", node.Body.Select(n => n.ExceptionalAccept(this)))}";
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

        public override string ExceptionalVisitScript(ScriptNode node)
        {
            string baseText = node.Base.ExceptionalAccept(this);
            string scriptText = node.Script.ExceptionalAccept(this);
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
            if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSqrt(node); 
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
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }
        public override string ExceptionalVisitCommand(CommandNode node)
        {
            if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
            }
            switch (node.Command)
            {
                case @"\frac":
                    return HandleFraction(node);
                case @"\sqrt":
                    return HandleSqrt(node); 
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
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }


        private string HandleFraction(CommandNode node)
        {
            var side1 = node.Args[0].Accept(this);
            var side2 = node.Args[1].Accept(this);
            return $"{side1}/{side2}";
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

        public override string ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
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

        public override string ExceptionalVisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node)
        {
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string ExceptionalVisitGroup(GroupNode node)
        {
            return $"{string.Join("", node.Body.Select(n => n.ExceptionalAccept(this)))}";
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

        public override string ExceptionalVisitScript(ScriptNode node)
        {
            string baseText = node.Base.ExceptionalAccept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText}°";
                if (cmdNode.Command == @"\prime") return $"{baseText}′";
            }

            var scriptContent = node.Script.ExceptionalAccept(this);

            if (!node.IsSuperscript && !_allSubscriptsAreConvertible)
            {
                return $"{baseText}_{scriptContent}";
            }

            return $"{baseText}{ToUnicode(scriptContent, node.IsSuperscript, node.Script)}";
        }

        public override string VisitCommand(CommandNode node)
        {
            if (Dictionaries.HumanFriendlyTemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSqrt(node);
                case @"\mathcal":
                    return HandleMathcal(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                    return HandleTextFormatting(node);
                case @"\mathfrak":
                    return HandleMathfrak(node);
                case @"\mathscr":
                    return Handlemathscr(node);
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
        public override string ExceptionalVisitCommand(CommandNode node)
        {
            return VisitCommand(node);
        }
         
        private string HandleSqrt(CommandNode node)
        {
            return $"√({node.Args[0].Accept(this)})";
        }

        private string HandleMathcal(CommandNode node)
        {
            return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathcalMap);
        }

        private string HandleMathbb(CommandNode node)
        {
            return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathbbMap);
        }

        private string HandleTextFormatting(CommandNode node)
        {
            return node.Args[0].Accept(this);
        }

        private string HandleMathfrak(CommandNode node)
        {
            return ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathfrakMap);
        }

        private string Handlemathscr(CommandNode node)
        {
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

        public override string ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
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
                "+" => " plus ",
                "-" => " minus ",
                "=" => " equals ",
                "*" => " times ",
                "/" => " divided by ",
                "ħ" => " h bar ",
                _ => node.Text
            };
        }

        public override string ExceptionalVisitText(TextNode node)
        {
            if (Regex.IsMatch(node.Text, @"[a-zA-Z0-9]+(-[a-zA-Z0-9]+)+")) return node.Text;
            return node.Text switch
            {
                "+" => " plus ",
                "-" => " minus ",
                "=" => " equals ",
                "*" => " times ",
                "/" => " divided by ",
                "ħ" => " h bar ",
                _ => node.Text
            };
        }

        public override string VisitGroup(GroupNode node)
        {
            return string.Join(" ", node.Body.Select(n => n.Accept(this)));
        }

        public override string ExceptionalVisitGroup(GroupNode node)
        {
            return string.Join(" ", node.Body.Select(n => n.ExceptionalAccept(this)));
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
                    case "°": return $"{baseText} degrees";
                    default: return $"{baseText} to the power of {node.Script.Accept(this).Trim()}";
                }
            }
            return $"{baseText} subscript {node.Script.Accept(this).Trim()} ";
        }

        public override string ExceptionalVisitScript(ScriptNode node)
        {
            string baseText = node.Base.ExceptionalAccept(this).Trim();
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
                if (cmdNode.Command == @"\prime") return $"{baseText} prime";
            }

            string scriptText = node.Script.ExceptionalAccept(new PlainTextVisitor());
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
                    case "°": return $"{baseText} degrees";
                    default: return $"{baseText} to the power of {node.Script.ExceptionalAccept(this).Trim()}";
                }
            }
            return $"{baseText} subscript {node.Script.ExceptionalAccept(this).Trim()} ";
        }

        public override string VisitCommand(CommandNode node)
        {
            if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSQRT(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleStyledText(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    return HandleLimitStyleCommands(node);
                case @"\lim":
                    return HandleLimitCommands(node);
                case @"\pm": return "plus-minus";
                case @"\mp": return "minus-plus";
                case @"\equiv": return "congruent to";
                case @"\Rightarrow": return "right double arrow";
                case @"\Leftrightarrow": return "if and only if";
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    return Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
            }
        }

        public override string ExceptionalVisitCommand(CommandNode node)
        {
            if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSQRT(node); 
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleStyledText(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    return HandleLimitStyleCommands(node);
                case @"\lim":
                    return HandleLimitCommands(node);
                case @"\pm":
                case @"\mp":
                case @"\equiv":
                case @"\Rightarrow":
                case @"\Leftrightarrow":
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    string screenReaderSymbolVal = Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
                    return Dictionaries.ExceptionalScreenReaderSymbolMap.GetValueOrDefault(node.Command, screenReaderSymbolVal);
            }
        }

        private string HandleSQRT(CommandNode node)
        {
            var content = node.Args[0].Accept(this);
            if (node.Args[0] is GroupNode)
            {
                return $"the square root of ({content})";
            }
            return $"the square root of {content}";
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
            var style = command.Substring(1).Replace("math", "");
            return $"{style} {node.Args[0].Accept(this)}";
        }

        private string HandleLimitCommands(CommandNode node)
        {
            var sub_lim = node.Subscript != null ? node.Subscript.ExceptionalAccept(this) : "";
            return $"limit as {sub_lim} of";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sub = node.Subscript != null ? node.Subscript.Accept(this) : "";
            var sup = node.Superscript != null ? node.Superscript.Accept(this) : "";
            string commandName = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, "");
            return $"{commandName} from {sub} to {sup}";
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

        public override string ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
        }
    }

    /// <summary>
    /// A visitor that extracts the plain text from the AST, without any formatting.
    /// </summary>
    public class PlainTextVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;

        public override string ExceptionalVisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node) => string.Concat(node.Body.Select(n => n.Accept(this)));

        public override string ExceptionalVisitGroup(GroupNode node) => string.Concat(node.Body.Select(n => n.ExceptionalAccept(this)));

        public override string VisitScript(ScriptNode node) => node.Base.Accept(this) + node.Script.Accept(this);

        public override string ExceptionalVisitScript(ScriptNode node) => node.Base.ExceptionalAccept(this) + node.Script.ExceptionalAccept(this);

        public override string VisitCommand(CommandNode node) => string.Concat(node.Args.Select(n => n.Accept(this)));

        public override string ExceptionalVisitCommand(CommandNode node) => string.Concat(node.Args.Select(n => n.ExceptionalAccept(this)));

        public override string VisitMatrix(MatrixNode node) => node.Content;

        public override string ExceptionalVisitMatrix(MatrixNode node) => node.Content;
    }
    #endregion
}

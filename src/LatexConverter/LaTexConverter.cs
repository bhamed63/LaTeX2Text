using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

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

            humanFriendlyText = PreProcessHumanLatexPatterns(humanFriendlyText);

            var sb = new StringBuilder();
            for (int i = 0; i < humanFriendlyText.Length; i++)
            {
                char c = humanFriendlyText[i];

                if (TryHandleIntegral(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleCombiningCharacters(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSuperscript(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSubscript(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSpecialSymbols(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleGeneralSymbol(c, sb, humanFriendlyText, i)) continue;
                if (TryHandleFontCharacter(c, sb, humanFriendlyText, i)) continue;

                sb.Append(c);
            }
            return PostProcessHumanLatex(sb.ToString());
        }

        private string PreProcessHumanLatexPatterns(string text)
        {
            text = text.Replace("sin⁻¹", CommandNames.Arcsin + " ");
            text = text.Replace("cos⁻¹", CommandNames.Arccos + " ");
            text = text.Replace("tan⁻¹", CommandNames.Arctan + " ");
            text = Regex.Replace(text, @"√\((.*?)\)", CommandNames.Sqrt + "{$1}");
            text = Regex.Replace(text, @"√([^ ])", CommandNames.Sqrt + "{$1}");
            text = Regex.Replace(text, @"\(([^ ]*?) ([^ ]*?)\)\n\(([^ ]*?) ([^ ]*?)\)", @"\begin{pmatrix} $1 & $2 \\ $3 & $4 \end{pmatrix}");
            text = Regex.Replace(text, @"\(([^ ]*?) ([^ ]*?)\)", CommandNames.Binom + "{$1}{$2}");
            text = text.Replace("p̅", CommandNames.Overline + "{p}");
            text = text.Replace("xyz̅", CommandNames.Overline + "{xyz}");
            text = Regex.Replace(text, @"lim_{([^}]*)}", CommandNames.Lim + "_{$1}");
            return text;
        }

        private bool TryHandleIntegral(char c, StringBuilder sb, string text, ref int i)
        {
            if (c != '∫') return false;

            sb.Append(CommandNames.Int);
            i++;

            var sub_content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSubMap.ContainsKey(text[i]))
            {
                sub_content.Append(Dictionaries.ReverseSubMap[text[i]]);
                i++;
            }
            if (sub_content.Length > 0)
            {
                if (sub_content.Length > 1) sb.Append($"_{{{sub_content}}}"); else sb.Append($"_{sub_content}");
            }

            var sup_content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSupMap.ContainsKey(text[i]))
            {
                sup_content.Append(Dictionaries.ReverseSupMap[text[i]]);
                i++;
            }
            if (sup_content.Length > 0)
            {
                if (sup_content.Length > 1) sb.Append($"^{{{sup_content}}}"); else sb.Append($"^{sup_content}");
            }
            i--;
            return true;
        }

        private bool TryHandleCombiningCharacters(char c, StringBuilder sb, string text, ref int i)
        {
            if (i + 1 >= text.Length) return false;

            char next_c = text[i + 1];
            string? command = null;

            if (next_c == '\u20D7') command = CommandNames.Vec; // vec
            if (next_c == '\u0302') command = CommandNames.Hat; // hat

            if (command != null)
            {
                sb.Append($"{command}{{{c}}}");
                i++; // consume combining char
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private bool TryHandleSuperscript(char c, StringBuilder sb, string text, ref int i)
        {
            if (!Dictionaries.ReverseSupMap.ContainsKey(c)) return false;

            var content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSupMap.ContainsKey(text[i]))
            {
                content.Append(Dictionaries.ReverseSupMap[text[i]]);
                i++;
            }
            i--;
            if (content.Length > 1) sb.Append($"^{{{content}}}"); else sb.Append($"^{content}");
            return true;
        }

        private bool TryHandleSubscript(char c, StringBuilder sb, string text, ref int i)
        {
            if (!Dictionaries.ReverseSubMap.ContainsKey(c)) return false;

            var content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSubMap.ContainsKey(text[i]))
            {
                var base_char = Dictionaries.ReverseSubMap[text[i]];
                if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(base_char.ToString()))
                {
                    content.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[base_char.ToString()]}");
                }
                else { content.Append(base_char); }
                i++;
            }
            i--;
            if (content.Length > 1 && !content.ToString().Contains("\\")) sb.Append($"_{{{content}}}"); else sb.Append($"_{content}");
            return true;
        }

        private bool TryHandleSpecialSymbols(char c, StringBuilder sb, string text, ref int i)
        {
            string? command = c switch
            {
                '⇔' => CommandNames.BigLeftrightarrow,
                '⇒' => CommandNames.BigRightarrow,
                '°' => CommandNames.Circ,
                _ => null
            };

            if (command != null)
            {
                sb.Append(command).Append(" ");
                return true;
            }
            return false;
        }

        private bool TryHandleGeneralSymbol(char c, StringBuilder sb, string text, int i)
        {
            if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(c.ToString()) && c != ' ')
            {
                sb.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[c.ToString()]}");
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private bool TryHandleFontCharacter(char c, StringBuilder sb, string text, int i)
        {
            if (Dictionaries.ReverseMathFontMap.ContainsKey(c.ToString()))
            {
                sb.Append($"{Dictionaries.ReverseMathFontMap[c.ToString()]}");
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private void AppendSpaceIfNeeded(StringBuilder sb, string text, int i)
        {
            if (i + 1 < text.Length)
            {
                char next_c = text[i + 1];
                if (!Dictionaries.ReverseSubMap.ContainsKey(next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_c))
                {
                    sb.Append(" ");
                }
            }
            else
            {
                sb.Append(" ");
            }
        }

        private string PostProcessHumanLatex(string text)
        {
            return Regex.Replace(text, @" ([,.])", "$1");
        }

        /// <summary>
        /// Normalizes structural patterns in the LaTeX input, such as converting `sqrt(x)` to `\sqrt{x}`.
        /// </summary>
        /// <param name="latex_input">The LaTeX string to normalize.</param>
        /// <returns>The normalized LaTeX string.</returns>
        private string NormalizeStructuralPatterns(string latex_input)
        {
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", CommandNames.Sqrt + "{$1}");
            processed_input = Regex.Replace(processed_input, $@"\\({CommandNames.Cos.Substring(1)}|{CommandNames.Sin.Substring(1)}|{CommandNames.Tan.Substring(1)}|{CommandNames.Log.Substring(1)}|{CommandNames.Ln.Substring(1)}|{CommandNames.Exp.Substring(1)}|{CommandNames.Det.Substring(1)})\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, $@"({CommandNames.Sin.Substring(1)}|{CommandNames.Cos.Substring(1)}|{CommandNames.Tan.Substring(1)})\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
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
            text = Regex.Replace(text, $@"\\({CommandNames.Big}|{CommandNames.Big.ToLower()}|{CommandNames.Bigg.ToLower()}|{CommandNames.Bigg})[l|r]?\s*[\(\)]", "");

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
                        if (visitor is HumanFriendlyVisitor humanFriendlyVisitor)
                        {
                            processedPart = humanFriendlyVisitor.GetPreProcessedResult(processedPart);
                        }
                        else if (visitor is ScreenReaderVisitor screenReaderVisitor)
                        {
                            processedPart = screenReaderVisitor.GetPreProcessedResult(processedPart);
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
}

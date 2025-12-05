using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using LatexConverter.Visitors;

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
            var converter = new HumanToLatexConverter();
            return converter.Convert(humanFriendlyText);
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
            processed_input = Regex.Replace(processed_input, $@"({CommandNames.Sin.Substring(1)}|{CommandNames.Cos.Substring(1)}|{CommandNames.Tan.Substring(1)})\s*\^\s*\(\s*-1\s*\)", $@"{SyntaxNames.Arc}$1");
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
                    var processedPart = part;

                    if (!string.IsNullOrEmpty(processedPart))
                    {
                        var tokens = Tokenizer.Tokenize(processedPart);
                        var parser = new Parser(tokens);
                        var nodes = parser.Parse();
                        var convertedPart = string.Join("", nodes.Select(n => n.Accept(visitor)));
                        convertedPart = (visitor as BaseVisitor<string>).GetPreProcessedResult(convertedPart);
                        resultBuilder.Append(convertedPart);
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

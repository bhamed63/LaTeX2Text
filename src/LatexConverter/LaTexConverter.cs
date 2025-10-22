using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace LatexConverter
{
    public class LaTexConverter
    {
        public string ConvertToOpenAIFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            return Process(normalized_input, new OpenAIVisitor());
        }

        public string ConvertToHumanFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            normalized_input = NormalizePlainTextToLatex(normalized_input);
            return Process(normalized_input, new HumanFriendlyVisitor());
        }

        public string ConvertToScreenReaderFriendlyText(string latex_input)
        {
            var normalized_input = NormalizeStructuralPatterns(latex_input);
            return Process(normalized_input, new ScreenReaderVisitor());
        }

        private string NormalizeStructuralPatterns(string latex_input)
        {
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", @"\sqrt{$1}");
            processed_input = Regex.Replace(processed_input, @"\\(cos|sin|tan|log|ln|exp|det)\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, @"(sin|cos|tan)\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
            return processed_input;
        }

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

        private string Process(string text, IVisitor<string> visitor)
        {
            text = Regex.Replace(text, @"\\(big|Big|bigg|Bigg)[l|r]?\s*[\(\)]", "");
            var lines = text.Split('\n');
            var processedLines = lines.Select(line =>
            {
                var tokens = Tokenizer.Tokenize(line);
                var parser = new Parser(tokens);
                var nodes = parser.Parse();
                var result = string.Join("", nodes.Select(n => n.Accept(visitor)));
                result = Regex.Replace(result, @"[ \t]+", " ").Trim();
                if (visitor is HumanFriendlyVisitor)
                {
                    result = ApplyHumanFriendlyPostProcessing(result);
                }
                else if (visitor is ScreenReaderVisitor)
                {
                    result = ApplyScreenReaderPostProcessing(result);
                }
                return result;
            });
            return string.Join("\n", processedLines);
        }

        private string ApplyHumanFriendlyPostProcessing(string text)
        {
            text = Regex.Replace(text, @"\s*([\[\]])\s*", "$1");
            text = Regex.Replace(text, @"\s*√\s*\((.*?)\)", "√($1)");
            text = Regex.Replace(text, @"(sin⁻¹|cos⁻¹|tan⁻¹)\s+\(", "$1(");
            text = Regex.Replace(text, @"(¬)\s+([a-zA-Z0-9])", "$1$2");
            return text;
        }

        private string ApplyScreenReaderPostProcessing(string text)
        {
            text = Regex.Replace(text, @"\(\s+", "(");
            text = Regex.Replace(text, @"\s+\)", ")");
            return text;
        }
    }

    #region Parser
    public enum TokenType { Command, Text, LBrace, RBrace, Superscript, Subscript, Eof, Space, Matrix }

    public record Token(TokenType Type, string Value = "");

    public static class Tokenizer
    {
        private static readonly Regex CommandRegex = new Regex(@"\\[a-zA-Z]+|\\.", RegexOptions.Compiled);
        private static readonly Regex MatrixRegex = new Regex(@"\\begin\{pmatrix\}(.*?)\\end\{pmatrix\}", RegexOptions.Singleline | RegexOptions.Compiled);

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
                while (pos < text.Length && (char.IsDigit(text[pos]) || text[pos] == '.')) pos++;
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
                case '_': tokens.Add(new Token(TokenType.Subscript)); pos++; return true;
                default:
                    tokens.Add(new Token(TokenType.Text, currentChar.ToString()));
                    pos++;
                    return true;
            }
        }
    }

    public abstract record AstNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
        public virtual bool NeedsParentheses() => false;
    }
    public record TextNode(string Text) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitText(this);
        public override bool NeedsParentheses() => !Regex.IsMatch(Text, @"^[a-zA-Z0-9]+$");
    }
    public record CommandNode(string Command, List<AstNode> Args, AstNode Subscript, AstNode Superscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCommand(this);
    }
    public record GroupNode(List<AstNode> Body) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroup(this);
        public override bool NeedsParentheses() => this.Body.Count > 1 || this.Body.Any(b => b is ScriptNode scriptNode && scriptNode.Script is GroupNode);
    }
    public record ScriptNode(AstNode Base, AstNode Script, bool IsSuperscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitScript(this);

    }

    public record MatrixNode(string Content) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitMatrix(this);
    }

    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;
        private Token CurrentToken => _tokens[_pos];

        public Parser(List<Token> tokens) { _tokens = tokens; }

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
                @"\sqrt" or @"\vec" or @"\hat" or @"\mathcal" or @"\mathbb" or @"\text" or @"\mathrm" or @"\textrm" or @"\cos" or @"\sin" or @"\tan" or @"\log" or @"\ln" or @"\exp" or @"\det" => 1,
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
                args.Add(ParsePrimary());
            }

            return new CommandNode(token.Value, args, null, null);
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
    public interface IVisitor<T>
    {
        T VisitText(TextNode node);
        T VisitCommand(CommandNode node);
        T VisitGroup(GroupNode node);
        T VisitScript(ScriptNode node);
        T VisitMatrix(MatrixNode node);
    }

    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
    }

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

    public class HumanFriendlyVisitor : BaseVisitor<string>
    {
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

            string scriptText = node.Script.Accept(this).Trim();
            if (node.IsSuperscript)
            {
                if (scriptText == "2") return $"{baseText} squared";
                if (scriptText == "3") return $"{baseText} cubed";
                return $"{baseText} to the power of {scriptText}";
            }
            return $"{baseText} subscript {scriptText}";
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
                    return node.Args[0].Accept(this);
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

        private string HandleLimitStyleCommands(CommandNode node)
        {
            if (node.Command == @"\lim")
            {
                var sub_lim = node.Subscript != null ? node.Subscript.Accept(this) : "";
                return $"limit as {sub_lim} of ";
            }

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
    }
    #endregion
}
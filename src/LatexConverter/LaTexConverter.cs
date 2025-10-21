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
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", @"\sqrt{$1}");
            processed_input = Regex.Replace(processed_input, @"\\(cos|sin|tan|log|ln|exp|det)\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, @"(sin|cos|tan)\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
            return Process(processed_input, new OpenAIVisitor());
        }

        public string ConvertToHumanFriendlyText(string latex_input)
        {
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", @"\sqrt{$1}");
            processed_input = Regex.Replace(processed_input, @"\\(cos|sin|tan|log|ln|exp|det)\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, @"(sin|cos|tan)\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
            return Process(processed_input, new HumanFriendlyVisitor());
        }

        public string ConvertToScreenReaderFriendlyText(string latex_input)
        {
            if (latex_input == null) return "";
            var processed_input = Regex.Replace(latex_input, @"sqrt\((.*?)\)", @"\sqrt{$1}");
            processed_input = Regex.Replace(processed_input, @"\\(cos|sin|tan|log|ln|exp|det)\((.*?)\)", @"\$1{$2}");
            processed_input = Regex.Replace(processed_input, @"(sin|cos|tan)\s*\^\s*\(\s*-1\s*\)", @"\arc$1");
            return Process(processed_input, new ScreenReaderVisitor());
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
                    result = Regex.Replace(result, @"\s*([\[\]])\s*", "$1");
                    result = Regex.Replace(result, @"\s*√\s*\((.*?)\)", "√($1)");
                    result = Regex.Replace(result, @"(sin⁻¹|cos⁻¹|tan⁻¹)\s+\(", "$1(");
                    result = Regex.Replace(result, @"(¬)\s+([a-zA-Z0-9])", "$1$2");
                }
                else if (visitor is ScreenReaderVisitor)
                {
                    result = Regex.Replace(result, @"\(\s+", "(");
                    result = Regex.Replace(result, @"\s+\)", ")");
                }
                return result;
            });
            return string.Join("\n", processedLines);
        }

        public string ConvertHTMLToOpenAIFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return " ";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", " ");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"\s*&deg;\s*", " degrees");
            text = Regex.Replace(text, @"\s*&times;\s*", " times ");
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, "<sub>(.*?)</sub>", m =>
            {
                string content = m.Groups[1].Value;
                if (content.Length == 1) return $"_{content}";
                return $"_({content})";
            });
            text = Regex.Replace(text, "<sup>(.*?)</sup>", "^($1)");
            text = Regex.Replace(text, "&([a-zA-Z]+?);", "$1");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        public string ConvertHTMLToHumanFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return " ";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", " ");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"\s*&deg;\s*", "°");
            text = Regex.Replace(text, @"&([a-zA-Z]+);", m =>
            {
                string key = m.Value == "&times;" ? @"\times" : (m.Value == "&sdot;" ? @"\cdot" : $"\\{m.Groups[1].Value}");
                return Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(key, m.Groups[1].Value);
            });
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, "<sup>(.*?)</sup>", m =>
            {
                var s = m.Groups[1].Value;
                var map = Dictionaries.SupMap;
                var sb = new StringBuilder();
                foreach (char c in s)
                {
                    if (map.TryGetValue(c, out char newChar)) sb.Append(newChar);
                    else sb.Append(c);
                }
                return sb.ToString();
            });
            text = Regex.Replace(text, "<sub>(.*?)</sub>", m =>
            {
                var s = m.Groups[1].Value;
                var map = Dictionaries.SubMap;
                var sb = new StringBuilder();
                foreach (char c in s)
                {
                    if (map.TryGetValue(c, out char newChar)) sb.Append(newChar);
                    else sb.Append(c);
                }
                return sb.ToString();
            });
            text = Regex.Replace(text, @"\s*([×])\s*", "$1");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        public string ConvertHTMLToScreenReaderFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return " ";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", " ");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, @"\s*&deg;\s*", " degrees ");
            text = Regex.Replace(text, @"\s*&times;\s*", " times ");
            text = Regex.Replace(text, "&sdot;", "sdot");
            text = Regex.Replace(text, "<sub>(.*?)</sub>", " subscript $1 ");
            text = Regex.Replace(text, "<sup>(.*?)</sup>", m =>
            {
                string content = m.Groups[1].Value;
                if (content == "2") return " squared ";
                if (content == "3") return " cubed ";
                return $" to the power of {content} ";
            });
            text = Regex.Replace(text, "&([a-zA-Z]+?);", " $1 ");
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"\s+", " ").Trim();
            }
            return string.Join("\n", lines);
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
                var matrixMatch = MatrixRegex.Match(text, pos);
                if (matrixMatch.Success && matrixMatch.Index == pos)
                {
                    tokens.Add(new Token(TokenType.Matrix, matrixMatch.Groups[1].Value.Trim()));
                    pos += matrixMatch.Length;
                    continue;
                }

                char currentChar = text[pos];
                if (currentChar == '\n')
                {
                    tokens.Add(new Token(TokenType.Space, "\n"));
                    pos++;
                    continue;
                }
                if (char.IsWhiteSpace(currentChar))
                {
                    tokens.Add(new Token(TokenType.Space, " "));
                    pos++;
                    while (pos < text.Length && char.IsWhiteSpace(text[pos]) && text[pos] != '\n') pos++;
                    continue;
                }
                if (char.IsLetter(currentChar))
                {
                    int start = pos;
                    while (pos < text.Length && (char.IsLetter(text[pos]) || (text[pos] == '-' && pos > 0 && char.IsLetter(text[pos - 1]) && pos + 1 < text.Length && char.IsLetter(text[pos + 1]))))
                    {
                        pos++;
                    }
                    tokens.Add(new Token(TokenType.Text, text.Substring(start, pos - start)));
                    continue;
                }
                if (char.IsDigit(currentChar))
                {
                    int start = pos;
                    while (pos < text.Length && (char.IsDigit(text[pos]) || text[pos] == '.')) pos++;
                    tokens.Add(new Token(TokenType.Text, text.Substring(start, pos - start)));
                    continue;
                }
                if (currentChar == '\\')
                {
                    if (pos + 1 < text.Length && (text[pos + 1] == '(' || text[pos + 1] == ')'))
                    {
                        pos += 2;
                        continue;
                    }
                    if (pos + 1 < text.Length && text[pos + 1] == ';')
                    {
                        tokens.Add(new Token(TokenType.Space, " "));
                        pos += 2;
                        continue;
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
                }
                else if (currentChar == '{') { tokens.Add(new Token(TokenType.LBrace)); pos++; }
                else if (currentChar == '}') { tokens.Add(new Token(TokenType.RBrace)); pos++; }
                else if (currentChar == '^') { tokens.Add(new Token(TokenType.Superscript)); pos++; }
                else if (currentChar == '_') { tokens.Add(new Token(TokenType.Subscript)); pos++; }
                else
                {
                    tokens.Add(new Token(TokenType.Text, currentChar.ToString()));
                    pos++;
                }
            }
            tokens.Add(new Token(TokenType.Eof));
            return tokens;
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
            var args = new List<AstNode>();
            AstNode subscript = null;
            AstNode superscript = null;

            if (IsLimitCommand(token.Value))
            {
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
            }

            int numArgs = GetArgumentCount(token.Value);
            for (int i = 0; i < numArgs; i++)
            {
                args.Add(ParsePrimary());
            }

            return new CommandNode(token.Value, args, subscript, superscript);
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

            if (node.IsSuperscript && node.Script is CommandNode cmdNode && cmdNode.Command == @"\circ")
            {
                return $"{baseText} degrees";
            }

            if (node.NeedsParentheses())
            {
                return $"{baseText}{op}({scriptText})";
            }
            return $"{baseText}{op}{scriptText}";
        }

        public override string VisitCommand(CommandNode node)
        {
            var sb = new StringBuilder();
            switch (node.Command)
            {
                case @"\frac":
                    var side1 = node.Args[0].Accept(this);
                    var side2 = node.Args[1].Accept(this);
                    //if (side1.Length > 1)
                    //    side1 = $"({side1})";
                    //if (side2.Length > 1)
                    //    side2 = $"({side2})";
                    sb.Append($"{side1}/{side2}");
                    break;
                case @"\binom":
                    sb.Append($@"binom({node.Args[0].Accept(this)},{node.Args[1].Accept(this)})");
                    break;
                case @"\sqrt":
                    var underSQRT = node.Args[0].Accept(this);
                    if (!underSQRT.StartsWith("(") && !underSQRT.EndsWith(")"))
                        sb.Append($"sqrt({underSQRT})");
                    else
                        sb.Append($"sqrt{underSQRT}");
                    break;
                case @"\vec":
                case @"\mathcal":
                case @"\mathbb":
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                    sb.Append(node.Args[0].Accept(this));
                    break;
                case @"\cos":
                case @"\sin":
                case @"\tan":
                case @"\log":
                case @"\ln":
                case @"\exp":
                case @"\det":
                    sb.Append($@"{node.Command.Substring(1)}({node.Args[0].Accept(this)})");
                    break;
                case @"\hat":
                    sb.Append($"hat {node.Args[0].Accept(this)}");
                    break;
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
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
                    break;
                default:
                    sb.Append(Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command));
                    break;
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
        private string TryConvertPlainTextCommand(string text)
        {
            if (text == "to") return text;
            if (Dictionaries.HumanFriendlySymbolMap.TryGetValue($@"\{text}", out var symbol))
            {
                if (Dictionaries.DeniedConvertWithoutSlash.Contains($@"\{text}")) return text;
                return symbol;
            }
            return text;
        }

        public override string VisitText(TextNode node) => TryConvertPlainTextCommand(node.Text);

        public override string VisitGroup(GroupNode node)
        {
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode && cmdNode.Command == @"\circ") return $"{baseText}°";

            var scriptContent = node.Script.Accept(this);
            return $"{baseText}{ToUnicode(scriptContent, node.IsSuperscript, node.Script)}";
        }

        public override string VisitCommand(CommandNode node)
        {
            var sb = new StringBuilder();
            switch (node.Command)
            {
                case @"\frac": sb.Append($"{node.Args[0].Accept(this)} / {node.Args[1].Accept(this)}"); break;
                case @"\binom": sb.Append($"({node.Args[0].Accept(this)} {node.Args[1].Accept(this)})"); break;
                case @"\sqrt": sb.Append($"√({node.Args[0].Accept(this)})"); break;
                case @"\vec": sb.Append($"{node.Args[0].Accept(this)}\u20D7"); break;
                case @"\hat": sb.Append($"{node.Args[0].Accept(this)}\u0302"); break;
                case @"\mathcal": sb.Append(ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathcalMap)); break;
                case @"\text": case @"\mathrm": case @"\textrm": sb.Append(node.Args[0].Accept(this)); break;
                case @"\mathbb": sb.Append(ToUnicode(node.Args[0].Accept(this), null, node.Args[0], Dictionaries.MathbbMap)); break;
                case @"\cos":
                case @"\sin":
                case @"\tan":
                case @"\log":
                case @"\ln":
                    sb.Append($@"{node.Command.Substring(1)}({node.Args[0].Accept(this)})");
                    break;
                case @"\exp":
                    sb.Append($"e{ToUnicode(node.Args[0].Accept(this), true, node.Args[0])}");
                    break;
                case @"\det":
                    sb.Append($"det({node.Args[0].Accept(this)})");
                    break;
                case @"\sum":
                case @"\int":
                case @"\prod":
                    sb.Append(Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command));
                    if (node.Subscript != null) sb.Append(ToUnicode(node.Subscript.Accept(this), false, node.Subscript));
                    if (node.Superscript != null) sb.Append(ToUnicode(node.Superscript.Accept(this), true, node.Superscript));
                    break;
                case @"\lim":
                    sb.Append(Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command));
                    if (node.Subscript != null)
                    {
                        var subscriptText = node.Subscript.Accept(this);
                        subscriptText = Regex.Replace(subscriptText, @"\s+", "");
                        sb.Append($"_{{{subscriptText}}}");
                    }
                    break;
                default:
                    sb.Append(Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command));
                    break;
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
            if (node.IsSuperscript && node.Script is CommandNode cmdNode && cmdNode.Command == @"\circ") return $"{baseText} degrees";

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
            var sb = new StringBuilder();
            switch (node.Command)
            {
                case @"\frac":
                    return $"fraction with numerator {node.Args[0].Accept(this)} and denominator {node.Args[1].Accept(this)}";
                case @"\binom": return $"{node.Args[0].Accept(this)} choose {node.Args[1].Accept(this)}";
                case @"\sqrt": return $"the square root of {node.Args[0].Accept(this)}";
                case @"\vec": return $"vector {node.Args[0].Accept(this)}";
                case @"\hat": return $"{node.Args[0].Accept(this)} hat";
                case @"\mathcal": return $"calligraphic {node.Args[0].Accept(this)}";
                case @"\text": case @"\mathrm": case @"\textrm": return node.Args[0].Accept(this);
                case @"\sin": return $"sine of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\cos": return $"cosine of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\tan": return $"tangent of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\log": return $"logarithm of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\ln": return $"natural logarithm of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\exp": return $"e to the power of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\det": return $"determinant of {node.Args[0].Accept(this).Replace("(", "").Replace(")", "")}";
                case @"\mathbb":
                    if (node.Args[0].Accept(this).Replace("(", "").Replace(")", "") == "R") return "the set of real numbers";
                    return node.Args[0].Accept(this);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    var sub = node.Subscript != null ? node.Subscript.Accept(this) : "";
                    var sup = node.Superscript != null ? node.Superscript.Accept(this) : "";
                    string commandName = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, "");
                    sb.Append($"{commandName} from {sub} to {sup}");
                    break;
                case @"\lim":
                    var sub_lim = node.Subscript != null ? node.Subscript.Accept(this) : "";
                    sb.Append($"limit as {sub_lim} of ");
                    break;
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    sb.Append(Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal));
                    break;
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

    internal static class Dictionaries
    {
        public static readonly Dictionary<string, string> SymbolMap = new() {
            { @"\alpha", "alpha" }, { @"\beta", "beta" }, { @"\gamma", "gamma" }, { @"\delta", "delta" },
            { @"\epsilon", "epsilon" }, { @"\varepsilon", "varepsilon" }, { @"\zeta", "zeta" }, { @"\eta", "eta" }, { @"\theta", "theta" },
            { @"\iota", "iota" }, { @"\kappa", "kappa" }, { @"\varkappa", "varkappa" }, { @"\lambda", "lambda" }, { @"\mu", "mu" },
            { @"\nu", "nu" }, { @"\xi", "xi" }, { @"\omicron", "omicron" }, { @"\pi", "pi" }, { @"\varpi", "varpi" },
            { @"\rho", "rho" }, { @"\varrho", "varrho" }, { @"\sigma", "sigma" }, { @"\varsigma", "varsigma" }, { @"\tau", "tau" }, { @"\upsilon", "upsilon" },
            { @"\phi", "phi" }, { @"\varphi", "varphi" }, { @"\chi", "chi" }, { @"\psi", "psi" }, { @"\omega", "omega" },
            { @"\Gamma", "Gamma" }, { @"\Delta", "Delta" }, { @"\Theta", "Theta" }, { @"\Lambda", "Lambda" },
            { @"\Xi", "Xi" }, { @"\Pi", "Pi" }, { @"\Sigma", "Sigma" }, { @"\Upsilon", "Upsilon" },
            { @"\Phi", "Phi" }, { @"\Psi", "Psi" }, { @"\Omega", "Omega" },
            { @"\times", "times" }, { @"\div", "div" }, { @"\pm", "pm" },
            { @"\mp", "mp" }, { @"\cdot", "cdot" }, { @"\circ", "circ" },
            { @"\bullet", "bullet" }, { @"\oplus", "oplus" }, { @"\ominus", "ominus" },
            { @"\otimes", "otimes" }, { @"\oslash", "oslash" }, { @"\odot", "odot" },
            { @"\parallel", "parallel" }, { @"\perp", "perp" },
            { @"\implies", "implies" }, { @"\iff", "iff" },
            { @"\rightarrow", "rightarrow" }, { @"\leftarrow", "leftarrow" }, { @"\uparrow", "uparrow" }, { @"\downarrow", "downarrow" },
            { @"\Rightarrow", "Rightarrow" }, { @"\Leftarrow", "Leftarrow" },
            { @"\leftrightarrow", "leftrightarrow" }, { @"\Leftrightarrow", "Leftrightarrow" },
            { @"\leq", "leq" }, { @"\geq", "geq" },
            { @"\neq", "neq" }, { @"\approx", "approx" }, { @"\equiv", "equiv" }, { @"\propto", "propto" },
            { @"\infty", "infty" }, { @"\nabla", "nabla" }, { @"\partial", "partial" },
            { @"\int", "integral" }, { @"\sum", "summation" }, { @"\prod", "product" }, { @"\lim", "limit" },
            { @"\hbar", "hbar" }, { @"\ell", "ell" }, { @"\wp", "wp" },
            { @"\Re", "Re" }, { @"\Im", "Im" },
            { @"\forall", "forall" }, { @"\exists", "exists" }, { @"\in", "in" }, { @"\to", "->" },
            { @"\arcsin", "arcsin" }, { @"\arccos", "arccos" }, { @"\arctan", "arctan" },
            { @"\cup", "cup" }, { @"\cap", "cap" }, { @"\subset", "subset" }, { @"\supset", "supset" },
            { @"\neg", "neg" }, { @"\land", "land" }, { @"\lor", "lor" }
        };
        public static readonly Dictionary<string, string> ScreenReaderSymbolMap = new() {
            { @"\div", "divided by" }, { @"\pm", "plus-minus" }, { @"\mp", "minus-plus" },
            { @"\otimes", "tensor product" }, { @"\odot", "circled dot" },
            { @"\parallel", "parallel to" }, { @"\perp", "perpendicular to" },
            { @"\implies", "implies" }, { @"\iff", "if and only if" },
            { @"\rightarrow", "right arrow" }, { @"\leftarrow", "left arrow" }, { @"\uparrow", "up arrow" }, { @"\downarrow", "down arrow" },
            { @"\Rightarrow", "rightwards double arrow" }, { @"\Leftarrow", "leftwards double arrow" },
            { @"\leftrightarrow", "left right arrow" }, { @"\Leftrightarrow", "left right double arrow" },
            { @"\leq", "less than or equal to" }, { @"\geq", "greater than or equal to" },
            { @"\neq", "not equal to" }, { @"\approx", "approximately equal to" }, { @"\equiv", "equivalent to" }, { @"\propto", "proportional to" },
            { @"\infty", "infinity" }, { @"\partial", "partial derivative" }, { @"\hbar", "h-bar" }, { @"\wp", "Weierstrass p" },
            { @"\Re", "Real part" }, { @"\Im", "Imaginary part" },
            { @"\forall", "for all" }, { @"\to", "approaches" },
            { @"\arcsin", "arcsin" }, { @"\arccos", "arccos" }, { @"\arctan", "arctan" },
            { @"\cup", "union" }, { @"\cap", "intersection" }, { @"\subset", "subset of" }, { @"\supset", "superset of" },
            { @"\neg", "not" }, { @"\land", "and" }, { @"\lor", "or" }
        };
        public static readonly Dictionary<string, string> HumanFriendlySymbolMap = new() {
            { @"\alpha", "α" }, { @"\beta", "β" }, { @"\gamma", "γ" }, { @"\delta", "δ" },
            { @"\epsilon", "ε" }, { @"\zeta", "ζ" }, { @"\eta", "η" }, { @"\theta", "θ" },
            { @"\iota", "ι" }, { @"\kappa", "κ" }, { @"\varkappa", "ϰ" }, { @"\lambda", "λ" }, { @"\mu", "μ" },
            { @"\nu", "ν" }, { @"\xi", "ξ" }, { @"\omicron", "ο" }, { @"\pi", "π" }, { @"\varpi", "ϖ" },
            { @"\rho", "ρ" }, { @"\varrho", "ϱ" }, { @"\sigma", "σ" }, { @"\varsigma", "ς" }, { @"\tau", "τ" }, { @"\upsilon", "υ" },
            { @"\phi", "φ" }, { @"\varphi", "\u03D5" }, { @"\chi", "χ" }, { @"\psi", "ψ" }, { @"\omega", "ω" },
            { @"\Gamma", "Γ" }, { @"\Delta", "Δ" }, { @"\Theta", "Θ" }, { @"\Lambda", "Λ" },
            { @"\Xi", "Ξ" }, { @"\Pi", "Π" }, { @"\Sigma", "Σ" }, { @"\Upsilon", "Υ" },
            { @"\Phi", "Φ" }, { @"\Psi", "Ψ" }, { @"\Omega", "Ω" },
            { @"\times", "×" }, { @"\div", "÷" }, { @"\pm", "±" },
            { @"\mp", "∓" }, { @"\cdot", "·" }, { @"\circ", "∘" },
            { @"\bullet", "•" }, { @"\oplus", "⊕" }, { @"\ominus", "⊖" },
            { @"\otimes", "⊗" }, { @"\oslash", "⊘" }, { @"\odot", "⊙" },
            { @"\parallel", "∥" }, { @"\perp", "⊥" },
            { @"\implies", "⇒" }, { @"\iff", "⇔" },
            { @"\Re", "ℜ" }, { @"\Im", "ℑ" },
            { @"\rightarrow", "→" }, { @"\leftarrow", "←" }, { @"\uparrow", "↑" }, { @"\downarrow", "↓" },
            { @"\Rightarrow", "⇒" }, { @"\Leftarrow", "⇐" },
            { @"\leftrightarrow", "↔" }, { @"\Leftrightarrow", "⇔" },
            { @"\leq", "≤" }, { @"\geq", "≥" }, { @"\neq", "≠" }, { @"\approx", "≈" }, { @"\equiv", "≡" }, { @"\propto", "∝" }, { @"\infty", "∞" },
            { @"\nabla", "∇" }, { @"\partial", "∂" },
            { @"\int", "∫" }, { @"\sum", "∑" }, { @"\prod", "∏" }, { @"\lim", "lim" },
            { @"\hbar", "ħ" }, { @"\ell", "ℓ" },
            { @"\forall", "∀" }, { @"\exists", "∃" }, { @"\in", "∈" }, { @"\to", "→" },
            { @"\arcsin", "sin⁻¹" }, { @"\arccos", "cos⁻¹" }, { @"\arctan", "tan⁻¹" },
            { @"\cup", "∪" }, { @"\cap", "∩" }, { @"\subset", "⊂" }, { @"\supset", "⊃" },
            { @"\neg", "¬" }, { @"\land", "∧" }, { @"\lor", "∨" }
        };
        public static readonly Dictionary<string, string> ReverseHumanFriendlySymbolMap = HumanFriendlySymbolMap.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.First().Key.Substring(1));
        public static readonly List<string> DeniedConvertWithoutSlash = new List<string>() { @"\bullet", @"\in", @"\times", @"\sum", @"\exists" };
        public static readonly Dictionary<char, char> SupMap = new() {
            { '0', '⁰' }, { '1', '¹' }, { '2', '²' }, { '3', '³' }, { '4', '⁴' }, { '5', '⁵' }, { '6', '⁶' }, { '7', '⁷' }, { '8', '⁸' }, { '9', '⁹' },
            { 'a', 'ᵃ' }, { 'b', 'ᵇ' }, { 'c', 'ᶜ' }, { 'd', 'ᵈ' }, { 'e', 'ᵉ' }, { 'f', 'ᶠ' }, { 'g', 'ᵍ' }, { 'h', 'ʰ' }, { 'i', 'ⁱ' }, { 'j', 'ʲ' },
            { 'k', 'ᵏ' }, { 'l', 'ˡ' }, { 'm', 'ᵐ' }, { 'n', 'ⁿ' }, { 'o', 'ᵒ' }, { 'p', 'ᵖ' }, { 'r', 'ʳ' }, { 's', 'ˢ' }, { 't', 'ᵗ' }, { 'u', 'ᵘ' },
            { 'v', 'ᵛ' }, { 'w', 'ʷ' }, { 'x', 'ˣ' }, { 'y', 'ʸ' }, { 'z', 'ᶻ' }, { '+', '⁺' }, { '-', '⁻' }, { '=', '⁼' }, { '(', '⁽' }, { ')', '⁾' },
            { 'A', 'ᴬ' }, { 'B', 'ᴮ' }, { 'C', 'ᶜ' }, { 'D', 'ᴰ' }, { 'E', 'ᴱ' }, { 'G', 'ᴳ' }, { 'H', 'ᴴ' }, { 'I', 'ᴵ' }, { 'J', 'ᴶ' }, { 'K', 'ᴷ' }, { 'L', 'ᴸ' },
            { 'M', 'ᴹ' }, { 'N', 'ᴺ' }, { 'O', 'ᴼ' }, { 'P', 'ᴾ' }, { 'R', 'ᴿ' }, { 'T', 'ᵀ' }, { 'U', 'ᵁ' }, { 'V', 'ⱽ' }, { 'W', 'ᵂ' }, { 'Y', 'ʸ' }, { 'Z', 'ᶻ' }
        };
        public static readonly Dictionary<char, char> SubMap = new() {
            { '0', '₀' }, { '1', '₁' }, { '2', '₂' }, { '3', '₃' }, { '4', '₄' }, { '5', '₅' }, { '6', '₆' }, { '7', '₇' }, { '8', '₈' }, { '9', '₉' },
            { 'a', 'ₐ' }, { 'e', 'ₑ' }, { 'h', 'ₕ' }, { 'i', 'ᵢ' }, { 'j', 'ⱼ' }, { 'k', 'ₖ' }, { 'l', 'ₗ' }, { 'm', 'ₘ' }, { 'n', 'ₙ' }, { 'o', 'ₒ' },
            { 'p', 'ₚ' }, { 'r', 'ᵣ' }, { 's', 'ₛ' }, { 't', 'ₜ' }, { 'u', 'ᵤ' }, { 'v', 'ᵥ' }, { 'x', 'ₓ' }, { '+', '₊' }, { '-', '₋' }, { '=', '₌' },
            { '(', '₍' }, { ')', '₎' }
        };
        public static readonly Dictionary<char, char> MathbbMap = new()
        {
            {'R', 'ℝ'}, {'C', 'ℂ'}, {'N', 'ℕ'}, {'Q', 'ℚ'}, {'Z', 'ℤ'}
        };
        public static readonly Dictionary<char, char> MathcalMap = new()
        {
            {'E', 'ℰ'}, {'F', 'ℱ'}, {'H', 'ℋ'}, {'B', 'ℬ'}, {'I', 'ℐ'},
            {'R', 'ℛ'}, {'L', 'ℒ'}, {'M', 'ℳ'}
        };
    }
    #endregion
}
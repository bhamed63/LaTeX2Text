
using System.Linq;

namespace LatexConverter
{
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
            if (node.Command == @"\exp")
            {
                return $"e{ToUnicode(node.Args[0].Accept(this), true, node.Args[0])}";
            }

            if (Dictionaries.HumanFriendlyTemplateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => arg.Accept(this)).ToArray();
                return string.Format(template, args);
            }

            switch (node.Command)
            {
                case @"\frac":
                    return HandleFraction(node);
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
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }
        public override string ExceptionalVisitCommand(CommandNode node) => VisitCommand(node);

        private string HandleFraction(CommandNode node)
        {
            return $"{node.Args[0].Accept(this)} / {node.Args[1].Accept(this)}";
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

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command));
            if (node.Command == @"\lim")
            {
                if (node.Subscript != null)
                {
                    var subscriptText = node.Subscript.Accept(this);
                    subscriptText = System.Text.RegularExpressions.Regex.Replace(subscriptText, @"\s+", "");
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

        private string ToUnicode(string s, bool? isSuperscript, AstNode originalNode, System.Collections.Generic.Dictionary<char, char> map = null)
        {
            if (isSuperscript.HasValue && map == null) map = isSuperscript.Value ? Dictionaries.SupMap : Dictionaries.SubMap;

            string stripped_s = System.Text.RegularExpressions.Regex.Replace(s, @"[\(\)]", "");
            if (map != null && !string.IsNullOrEmpty(stripped_s) && stripped_s.All(c => map.ContainsKey(c)))
            {
                var sb = new System.Text.StringBuilder();
                foreach (char c in stripped_s) sb.Append(map[c]);
                return sb.ToString();
            }

            bool needsParentheses = !(originalNode is TextNode textNode && System.Text.RegularExpressions.Regex.IsMatch(textNode.Text, @"^[a-zA-Z0-9]+$"));
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
            var rows = node.Content.Split(new[] { @"\\" }, System.StringSplitOptions.RemoveEmptyEntries);
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
}

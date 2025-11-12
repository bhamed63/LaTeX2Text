using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            switch (node.Command)
            {
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
                case @"\exp":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    if (Dictionaries.HumanFriendlyTemplateMap.ContainsKey(node.Command))
                    {
                        return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
                    }
                    return Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }
        public override string ExceptionalVisitCommand(CommandNode node)
        {
            return VisitCommand(node);
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

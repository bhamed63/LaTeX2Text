
using System.Linq;

namespace LatexConverter
{
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
            if (Dictionaries.OpenAITemplateMap.TryGetValue(node.Command, out var template))
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
        public override string ExceptionalVisitCommand(CommandNode node)
        {
            return VisitCommand(node);
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

        private string HandleHat(CommandNode node)
        {
            return $"hat {node.Args[0].Accept(this)}";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sb = new System.Text.StringBuilder();
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
            var rows = node.Content.Split(new[] { @"\\" }, System.StringSplitOptions.RemoveEmptyEntries);
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
}

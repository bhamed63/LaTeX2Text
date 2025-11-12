using System.Linq;
using System.Text;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to an OpenAI-friendly format.
    /// </summary>
    public class OpenAIVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node)
        {
            if (node.NeedsParentheses())
                return $"({string.Join("", node.Body.Select(n => n.Accept(this)))})";
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        //public override string ExceptionalVisitGroup(GroupNode node)
        //{
        //    if (node.NeedsParentheses())
        //        return $"({string.Join("", node.Body.Select(n => n.ExceptionalAccept(this)))})";
        //    return $"{string.Join("", node.Body.Select(n => n.ExceptionalAccept(this)))}";
        //}

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

        //public override string ExceptionalVisitScript(ScriptNode node)
        //{
        //    string baseText = node.Base.ExceptionalAccept(this);
        //    string scriptText = node.Script.ExceptionalAccept(this);
        //    string op = node.IsSuperscript ? "^" : "_";

        //    if (node.IsSuperscript && node.Script is CommandNode cmdNode)
        //    {
        //        if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
        //        if (cmdNode.Command == @"\prime") return $"{baseText}'";
        //    }

        //    if (node.NeedsParentheses())
        //    {
        //        return $"{baseText}{op}({scriptText})";
        //    }
        //    return $"{baseText}{op}{scriptText}";
        //}

        public override string VisitCommand(CommandNode node)
        {
            if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
            }
            switch (node.Command)
            {
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
                case @"\sum":
                case @"\int":
                case @"\prod":
                case @"\lim":
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
        }
        //public override string ExceptionalVisitCommand(CommandNode node)
        //{
        //    if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
        //    {
        //        return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
        //    }
        //    switch (node.Command)
        //    {
        //        case @"\mathbb":
        //        case @"\text":
        //        case @"\mathrm":
        //        case @"\textrm":
        //        case @"\mathbf":
        //        case @"\mathit":
        //        case @"\mathsf":
        //        case @"\mathtt":
        //        case @"\mathfrak":
        //        case @"\mathscr":
        //            return HandleTextFormatting(node);
        //        case @"\sum":
        //        case @"\int":
        //        case @"\prod":
        //        case @"\lim":
        //            return HandleLimitStyleCommands(node);
        //        default:
        //            return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
        //    }
        //}

        private string HandleTextFormatting(CommandNode node)
        {
            return node.Args[0].Accept(this);
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
            var rows = node.Content.Split(new[] { @"\\" }, System.StringSplitOptions.RemoveEmptyEntries);
            var matrix = rows.Select(row =>
            {
                var elements = row.Split('&').Select(e => e.Trim());
                return $"[{string.Join(", ", elements)}]";
            });
            return $"matrix[{string.Join(", ", matrix)}]";
        }
    }
}

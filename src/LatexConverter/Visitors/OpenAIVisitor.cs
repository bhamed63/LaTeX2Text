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

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            string scriptText = node.Script.Accept(this);

            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == CommandNames.Circ || cmdNode.Command == CommandNames.Prime)
                    return BaseVisitor<string>.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.OpenAITemplateMap);
            }

            string commandName = node.IsSuperscript ? CommandNames.Superscript : CommandNames.Subscript;
            if (node.NeedsParentheses())
                scriptText = $"({scriptText})";
            return BaseVisitor<string>.ProcessTemplateCommand(commandName, new string[] { baseText, scriptText }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string VisitCommand(CommandNode node)
        {
            if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
            }
            switch (node.Command)
            {
                case CommandNames.LeftParen:
                case CommandNames.RightParen:
                case CommandNames.LeftBracket:
                case CommandNames.RightBracket:
                case CommandNames.Dollar:
                case CommandNames.DoubleDollar:
                case CommandNames.Comment:
                case @"\":
                    return "";
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                case CommandNames.Lim:
                    return HandleLimitStyleCommands(node);
                default:
                    return Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
            }
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
            var args = new string[] { string.Join(", ", matrix) };
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Matrix, args, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string VisitSqrt(SqrtNode node)
        {
            var arg = node.Argument.Accept(this);
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Sqrt, new[] { arg }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string VisitFrac(FracNode node)
        {
            var numerator = node.Numerator.Accept(this);
            var denominator = node.Denominator.Accept(this);
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.OpenAITemplateMap);
        }
    }
}

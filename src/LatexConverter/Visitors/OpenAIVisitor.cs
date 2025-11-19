using System.Text;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to an OpenAI-friendly format.
    /// </summary>
    public class OpenAIVisitor : BaseVisitor<string>
    {
        private readonly TemplateProcessor _templateProcessor;

        public OpenAIVisitor()
        {
            _templateProcessor = new TemplateProcessor();
        }

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
                    return _templateProcessor.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.OpenAITemplateMap);
            }

            string commandName = node.IsSuperscript ? CommandNames.Superscript : CommandNames.Subscript;
            if (node.NeedsParentheses())
                scriptText = $"({scriptText})";
            return _templateProcessor.ProcessTemplateCommand(commandName, new string[] { baseText, scriptText }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case CommandNames.Mathbb:
                    var arg = node.Args[0].Accept(new PlainTextVisitor());
                    if (arg.Length == 1 && Dictionaries.OpenAIMathbbMap.TryGetValue(arg[0], out var openAIText))
                    {
                        return openAIText;
                    }
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
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
                    if (Dictionaries.OpenAITemplateMap.ContainsKey(node.Command))
                    {
                        return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.OpenAITemplateMap);
                    }
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
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Matrix, args, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string VisitRoot(RootNode node)
        {
            var radicand = node.Radicand.Accept(this);
            var degree = node.Degree.Accept(this);

            return degree switch
            {
                "2" => _templateProcessor.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                "3" => _templateProcessor.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                "4" => _templateProcessor.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                _ => _templateProcessor.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, degree }, this, Dictionaries.OpenAITemplateMap),
            };
        }

        public override string ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override string VisitFrac(FracNode node)
        {
            var numerator = node.Numerator.Accept(this);
            var denominator = node.Denominator.Accept(this);
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.OpenAITemplateMap);
        }

        public override string GetPreProcessedResult(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"[ \t]+", " ").Trim();
        }

        public override string VisitRoot(RootNode node)
        {
            var radicand = node.Radicand.Accept(this);
            var degree = node.Degree.Accept(this);

            return degree switch
            {
                "2" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                "3" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                "4" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.OpenAITemplateMap),
                _ => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, degree }, this, Dictionaries.OpenAITemplateMap),
            };
        }

        public override string ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
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

        public override string GetPreProcessedResult(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"[ \t]+", " ").Trim();
        }
    }
}

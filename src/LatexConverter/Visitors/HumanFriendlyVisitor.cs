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

        public override string VisitGroup(GroupNode node)
        {
            var content = $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
            return content;
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == CommandNames.Circ || cmdNode.Command == CommandNames.Prime)
                    return BaseVisitor<string>.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
            }

            var scriptContent = node.Script.Accept(this);

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
                case CommandNames.Mathfrak:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathfrakMap);
                case CommandNames.Mathscr:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathscrMap);
                case CommandNames.Mathcal:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathcalMap);
                case CommandNames.Mathbb:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathbbMap);
                case CommandNames.Exp:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommandSubscriptSuperscript(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case CommandNames.Lim:
                    return BaseVisitor<string>.ProcessTemplateCommandSubscript(node, this, Dictionaries.HumanFriendlyTemplateMap);
                default:
                    return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
            }
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

        public override string VisitRoot(RootNode node)
        {
            var radicand = node.Radicand.Accept(this);
            var degree = node.Degree.Accept(this);

            return degree switch
            {
                "2" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                "3" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                "4" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                _ => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, ToUnicode(degree, true, node.Degree) }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
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
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string GetPreProcessedResult(string text)
        {
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ \t]+", " ").Trim();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s*([\[\]])\s*", "$1");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s*√\s*\((.*?)\)", "√($1)");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"(sin⁻¹|cos⁻¹|tan⁻¹)\s+\(", "$1(");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"(¬)\s+([a-zA-Z0-9])", "$1$2");
            return text;
        }
    }
}

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
            return $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ" || cmdNode.Command == @"\prime")
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
                case @"\mathfrak":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathfrakMap);
                case @"\mathscr":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathscrMap);
                case @"\mathcal":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathcalMap);
                case @"\mathbb":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.MathbbMap);
                case @"\exp":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    return BaseVisitor<string>.ToUnicodeProcessTemplateCommandSubscriptSuperscript(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case @"\lim":
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
    }
}

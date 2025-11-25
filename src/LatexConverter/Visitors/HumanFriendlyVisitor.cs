using LatexConverter.Visitors;
using LatexConverter.Ast;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to a human-friendly format with Unicode symbols.
    /// </summary>
    public class HumanFriendlyVisitor : BaseVisitor
    {
        private readonly bool _allSubscriptsAreConvertible;
        private readonly TemplateProcessor _templateProcessor;

        public HumanFriendlyVisitor(bool allSubscriptsAreConvertible = true)
        {
            _allSubscriptsAreConvertible = allSubscriptsAreConvertible;
            _templateProcessor = new TemplateProcessor();
        }
        public override string VisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node)
        {
            var content = $"{string.Join("", node.Body.Select(n => n.Accept(this)))}";
            if (node.Body.Count > 1)
                return $"({content})";
            return content;
        }

        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this);
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == CommandNames.Circ || cmdNode.Command == CommandNames.Prime)
                    return _templateProcessor.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
            }

            var scriptContent = node.Script.Accept(this);

            if (!node.IsSuperscript && !_allSubscriptsAreConvertible)
            {
                return _templateProcessor.ProcessTemplateCommand(CommandNames.SubscriptExceptional, new string[] { baseText, scriptContent }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
            }

            var commandName = node.IsSuperscript ? CommandNames.Superscript : CommandNames.Subscript;
            return _templateProcessor.ProcessTemplateCommand(commandName, new string[] { baseText, _templateProcessor.ToUnicode(scriptContent, node.IsSuperscript, node.Script) }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string VisitCommand(CommandNode node)
        {
            var arg = node.Args.Count > 0 ? node.Args[0].Accept(new PlainTextVisitor()) : "";
            switch (node.Command)
            {
                case CommandNames.Mathfrak:
                    if (arg.Length == 1 && Dictionaries.HumanFriendlyMathfrakMap.TryGetValue(arg[0], out var frak)) return frak;
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
                case CommandNames.Mathscr:
                    if (arg.Length == 1 && Dictionaries.HumanFriendlyMathscrMap.TryGetValue(arg[0], out var scr)) return scr;
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
                case CommandNames.Mathcal:
                    if (arg.Length == 1 && Dictionaries.HumanFriendlyMathcalMap.TryGetValue(arg[0], out var cal)) return cal;
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
                case CommandNames.Mathbb:
                    if (arg.Length == 1 && Dictionaries.HumanFriendlyMathbbMap.TryGetValue(arg[0], out var bb)) return bb;
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
                case CommandNames.Exp:
                    return _templateProcessor.ToUnicodeProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return _templateProcessor.ToUnicodeProcessTemplateCommandSubscriptSuperscript(node, this, Dictionaries.HumanFriendlyTemplateMap);
                case CommandNames.Lim:
                    return _templateProcessor.ProcessTemplateCommandSubscript(node, this, Dictionaries.HumanFriendlyTemplateMap);
                default:
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
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
                "2" => _templateProcessor.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                "3" => _templateProcessor.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                "4" => _templateProcessor.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
                _ => _templateProcessor.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, _templateProcessor.ToUnicode(degree, true, node.Degree) }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap),
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
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.HumanFriendlyTemplateMap, Dictionaries.HumanFriendlySymbolMap);
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

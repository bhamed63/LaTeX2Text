using System.Text;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to a screen reader-friendly format.
    /// </summary>
    public class ScreenReaderVisitor : BaseVisitor<string>
    {
        private readonly TemplateProcessor _templateProcessor;

        public ScreenReaderVisitor()
        {
            _templateProcessor = new TemplateProcessor();
        }

        public override string VisitText(TextNode node)
        {
            switch (node.Text)
            {
                case "+":
                case "-":
                case "=":
                case "*":
                case "/":
                    return _templateProcessor.ProcessTemplateCommand(node.Text, new string[] { }, this, new Dictionary<string, string>(), Dictionaries.ScreenReaderOperatorMap);
                default:
                    return node.Text;
            }
        }

        public override string VisitGroup(GroupNode node)
        {
            string content = string.Join(" ", node.Body.Select(n => n.Accept(this)));
            //if (node.Body.Count > 1)
            //    return $"({content})";
            return content;
        }


        public override string ExceptionalVisitGroup(GroupNode node)
        {
            return string.Join(" ", node.Body.Select(n => n.ExceptionalAccept(this)));
        }


        public override string VisitScript(ScriptNode node)
        {
            string baseText = node.Base.Accept(this).Trim();
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == CommandNames.Circ || cmdNode.Command == CommandNames.Prime)
                    return _templateProcessor.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);

            }

            string scriptText = node.Script.Accept(new PlainTextVisitor());
            if (node.IsSuperscript)
            {
                switch (scriptText)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "′":
                    case "2":
                    case "3":
                    case "°":
                        return _templateProcessor.ProcessTemplateCommand(scriptText, new string[] { baseText }, this, Dictionaries.ScreenReaderOperatorSuperscriptTemplateMap, Dictionaries.ScreenReaderOperatorMap);

                    default:
                        return _templateProcessor.ProcessTemplateCommand(CommandNames.Superscript, new string[] { baseText, node.Script.Accept(this).Trim() }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);

                }
            }
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Subscript, new string[] { baseText, node.Script.Accept(this).Trim() }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap) + " ";

        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case CommandNames.Mathbb:
                    var arg = node.Args[0].Accept(new PlainTextVisitor());
                    if (arg.Length == 1 && Dictionaries.ScreenReaderMathbbMap.TryGetValue(arg[0], out var screenReaderText))
                    {
                        return screenReaderText;
                    }
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return HandleLimitStyleCommands(node);
                case CommandNames.Lim:
                    return HandleLimitCommands(node);
                default:
                    if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
                    {
                        return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
                    }
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    return Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
            }
        }

        public override string ExceptionalVisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case CommandNames.Mathbb:
                    var arg = node.Args[0].Accept(new PlainTextVisitor());
                    if (arg.Length == 1 && Dictionaries.ScreenReaderMathbbMap.TryGetValue(arg[0], out var screenReaderText))
                    {
                        return screenReaderText;
                    }
                    return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return HandleLimitStyleCommands(node);
                case CommandNames.Lim:
                    return HandleLimitCommands(node);
                default:
                    if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
                    {
                        return _templateProcessor.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
                    }
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    string screenReaderSymbolVal = Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
                    return Dictionaries.ExceptionalScreenReaderSymbolMap.GetValueOrDefault(node.Command, screenReaderSymbolVal);
            }
        }

        public override string VisitRoot(RootNode node)
        {
            var radicand = node.Radicand.Accept(this);
            var degree = node.Degree.Accept(this);

            return degree switch
            {
                "2" => _templateProcessor.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                "3" => _templateProcessor.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                "4" => _templateProcessor.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                _ => _templateProcessor.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, ToOrdinal(degree) }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
            };
        }

        private string ToOrdinal(string number)
        {
            if (int.TryParse(number, out int numberInt))
                return NumberToOrdinalWordConverter.NumberToOrdinalWord(numberInt);
            return number;
        }

        public override string ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override string VisitFrac(FracNode node)
        {
            var numerator = node.Numerator.Accept(this);
            var denominator = node.Denominator.Accept(this);
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        private string HandleLimitCommands(CommandNode node)
        {
            var sub_lim = node.Subscript != null ? node.Subscript.ExceptionalAccept(this) : "";
            return _templateProcessor.ProcessTemplateCommand(node.Command, new string[] { sub_lim }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sub = node.Subscript != null ? node.Subscript.Accept(this) : "";
            var sup = node.Superscript != null ? node.Superscript.Accept(this) : "";

            var args = new string[] { sub, sup };
            return _templateProcessor.ProcessTemplateCommand(node.Command, args, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        public override string VisitMatrix(MatrixNode node)
        {
            var rows = node.Content.Split(new[] { @"\\" }, System.StringSplitOptions.RemoveEmptyEntries);
            var num_rows = rows.Length;
            var num_cols = rows[0].Split('&').Length;

            StringBuilder matrix_desc = new StringBuilder();
            for (int i = 0; i < rows.Length; i++)
            {
                var item = rows[i];
                var cols = rows[i].Split('&');
                if (i > 0)
                    matrix_desc.Append(", and ");

                matrix_desc.AppendFormat("row {0}: ", i + 1);
                for (int j = 0; j < cols.Length; j++)
                {
                    if (j > 0)
                        matrix_desc.Append(", ");
                    matrix_desc.Append(cols[j].Trim());
                }
            }

            var args = new string[] { num_rows.ToString(), num_cols.ToString(), matrix_desc.ToString() };
            return _templateProcessor.ProcessTemplateCommand(CommandNames.Matrix, args, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        public override string GetPreProcessedResult(string text)
        {
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ \t]+", " ").Trim();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\(\s+", "(");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+\)", ")");
            return text;
        }

        public override string VisitMath(MathNode node)
        {
            return string.Join(" ", node.Children.Select(child => child.Accept(this)));
        }
    }
}

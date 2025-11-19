using System.Text;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to a screen reader-friendly format.
    /// </summary>
    public class ScreenReaderVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node)
        {
            switch (node.Text)
            {
                case "+":
                case "-":
                case "=":
                case "*":
                case "/":
                    return " " + BaseVisitor<string>.ProcessTemplateCommand(node.Text, new string[] { }, this, new Dictionary<string, string>(), Dictionaries.ScreenReaderOperatorMap) + " ";
                default:
                    return node.Text;
            }
        }

        public override string VisitGroup(GroupNode node)
        {
            return string.Join(" ", node.Body.Select(n => n.Accept(this)));
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
                    return BaseVisitor<string>.ProcessTemplateCommand(cmdNode.Command, new string[] { baseText }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);

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
                        return " " + BaseVisitor<string>.ProcessTemplateCommand(scriptText, new string[] { baseText }, this, Dictionaries.ScreenReaderOperatorSuperscriptTemplateMap, Dictionaries.ScreenReaderOperatorMap);

                    default:
                        return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Superscript, new string[] { baseText, node.Script.Accept(this).Trim() }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);

                }
            }
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Subscript, new string[] { baseText, node.Script.Accept(this).Trim() }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap) + " ";

        }

        public override string VisitCommand(CommandNode node)
        {
            switch (node.Command)
            {
                case CommandNames.Mathbb:
                    return HandleMathbb(node);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return HandleLimitStyleCommands(node);
                case CommandNames.Lim:
                    return HandleLimitCommands(node);
                default:
                    if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
                    {
                        return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
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
                    return HandleMathbb(node);
                case CommandNames.Sum:
                case CommandNames.Int:
                case CommandNames.Prod:
                    return HandleLimitStyleCommands(node);
                case CommandNames.Lim:
                    return HandleLimitCommands(node);
                default:
                    if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
                    {
                        return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
                    }
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    string screenReaderSymbolVal = Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
                    return Dictionaries.ExceptionalScreenReaderSymbolMap.GetValueOrDefault(node.Command, screenReaderSymbolVal);
            }
        }

        public override string VisitRoot(RootNode node)
        {
            var radicand = node.Radicand.Accept(this);
            if (node.Radicand is GroupNode)
                radicand = $"({radicand})";
            var degree = node.Degree.Accept(this);

            return degree switch
            {
                "2" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Sqrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                "3" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Cbrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                "4" => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frthrt, new[] { radicand }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
                _ => BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Nthrt, new[] { radicand, ToOrdinal(degree) }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap),
            };
        }

        private string ToOrdinal(string number)
        {
            if (int.TryParse(number, out int num))
            {
                if (num <= 0) return number;

                if (num == 31) return "thirty-first";

                // Handle special cases for teens
                if ((num % 100) >= 11 && (num % 100) <= 13)
                {
                    return num + "th";
                }

                switch (num % 10)
                {
                    case 1: return num + "st";
                    case 2: return num + "nd";
                    case 3: return num + "rd";
                    default: return num + "th";
                }
            }
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
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Frac, new[] { numerator, denominator }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        public override string VisitBinom(BinomNode node)
        {
            var top = node.Top.Accept(this);
            var bottom = node.Bottom.Accept(this);
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Binom, new[] { top, bottom }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        private string HandleMathbb(CommandNode node)
        {
            //TODO: Need to extend Mathbb based on the accept result
            if (node.Args[0].Accept(this).Replace("(", "").Replace(")", "") == "R")
            {
                return "the set of real numbers";
            }
            return node.Args[0].Accept(this);
        }

        private string HandleLimitCommands(CommandNode node)
        {
            var sub_lim = node.Subscript != null ? node.Subscript.ExceptionalAccept(this) : "";
            return BaseVisitor<string>.ProcessTemplateCommand(node.Command, new string[] { sub_lim }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sub = node.Subscript != null ? node.Subscript.Accept(this) : "";
            var sup = node.Superscript != null ? node.Superscript.Accept(this) : "";

            var args = new string[] { sub, sup };
            return BaseVisitor<string>.ProcessTemplateCommand(node.Command, args, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
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
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Matrix, args, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }

        public override string GetPreProcessedResult(string text)
        {
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[ \t]+", " ").Trim();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\(\s+", "(");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+\)", ")");
            return text;
        }
    }
}

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
               //Need to extend \bar{a} \bar{p}
                case "ħ":
                    return " h bar ";
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
                case CommandNames.Sqrt:
                    return HandleSQRT(node);
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
                case CommandNames.Sqrt:
                    return HandleSQRT(node);
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

        private string HandleSQRT(CommandNode node)
        {
            var content = node.Args[0].Accept(this);
            if (node.Args[0] is GroupNode)
                content = $"({content})";
            return BaseVisitor<string>.ProcessTemplateCommand(node.Command, new string[] { content }, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
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

            var matrix_desc = rows.Select(row =>
            {
                var elements = row.Split('&').Select(e => e.Trim());
                return $"({string.Join(", ", elements)})";
            });

            var args = new string[] { num_rows.ToString(), num_cols.ToString(), string.Join(" and ", matrix_desc) };
            return BaseVisitor<string>.ProcessTemplateCommand(CommandNames.Matrix, args, this, Dictionaries.ScreenReaderTemplateMap, Dictionaries.ScreenReaderSymbolMap);
        }
    }
}

using System.Linq;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that converts the AST to a screen reader-friendly format.
    /// </summary>
    public class ScreenReaderVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node)
        {
            if (Regex.IsMatch(node.Text, @"[a-zA-Z0-9]+(-[a-zA-Z0-9]+)+")) return node.Text;
            return node.Text switch
            {
                "+" => " plus ",
                "-" => " minus ",
                "=" => " equals ",
                "*" => " times ",
                "/" => " divided by ",
                "ħ" => " h bar ",
                _ => node.Text
            };
        }

        public override string ExceptionalVisitText(TextNode node)
        {
            if (Regex.IsMatch(node.Text, @"[a-zA-Z0-9]+(-[a-zA-Z0-9]+)+")) return node.Text;
            return node.Text switch
            {
                "+" => " plus ",
                "-" => " minus ",
                "=" => " equals ",
                "*" => " times ",
                "/" => " divided by ",
                "ħ" => " h bar ",
                _ => node.Text
            };
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
                if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
                if (cmdNode.Command == @"\prime") return $"{baseText} prime";
            }

            string scriptText = node.Script.Accept(new PlainTextVisitor());
            if (node.IsSuperscript)
            {
                switch (scriptText)
                {
                    case "+": return $"{baseText} plus";
                    case "-": return $"{baseText} minus";
                    case "*": return $"{baseText} star";
                    case "′": return $"{baseText} prime";
                    case "2": return $"{baseText} squared";
                    case "3": return $"{baseText} cubed";
                    case "°": return $"{baseText} degrees";
                    default: return $"{baseText} to the power of {node.Script.Accept(this).Trim()}";
                }
            }
            return $"{baseText} subscript {node.Script.Accept(this).Trim()} ";
        }

        public override string ExceptionalVisitScript(ScriptNode node)
        {
            string baseText = node.Base.ExceptionalAccept(this).Trim();
            if (node.IsSuperscript && node.Script is CommandNode cmdNode)
            {
                if (cmdNode.Command == @"\circ") return $"{baseText} degrees";
                if (cmdNode.Command == @"\prime") return $"{baseText} prime";
            }

            string scriptText = node.Script.ExceptionalAccept(new PlainTextVisitor());
            if (node.IsSuperscript)
            {
                switch (scriptText)
                {
                    case "+": return $"{baseText} plus";
                    case "-": return $"{baseText} minus";
                    case "*": return $"{baseText} star";
                    case "′": return $"{baseText} prime";
                    case "2": return $"{baseText} squared";
                    case "3": return $"{baseText} cubed";
                    case "°": return $"{baseText} degrees";
                    default: return $"{baseText} to the power of {node.Script.ExceptionalAccept(this).Trim()}";
                }
            }
            return $"{baseText} subscript {node.Script.ExceptionalAccept(this).Trim()} ";
        }

        public override string VisitCommand(CommandNode node)
        {
            if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSQRT(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleStyledText(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    return HandleLimitStyleCommands(node);
                case @"\lim":
                    return HandleLimitCommands(node);
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    return Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
            }
        }

        public override string ExceptionalVisitCommand(CommandNode node)
        {
            if (Dictionaries.ScreenReaderTemplateMap.ContainsKey(node.Command))
            {
                return BaseVisitor<string>.ProcessTemplateCommand(node, this, Dictionaries.ScreenReaderTemplateMap);
            }
            switch (node.Command)
            {
                case @"\sqrt":
                    return HandleSQRT(node);
                case @"\text":
                case @"\mathrm":
                case @"\textrm":
                case @"\mathbf":
                case @"\mathit":
                case @"\mathsf":
                case @"\mathtt":
                case @"\mathfrak":
                case @"\mathscr":
                    return HandleStyledText(node);
                case @"\mathbb":
                    return HandleMathbb(node);
                case @"\sum":
                case @"\int":
                case @"\prod":
                    return HandleLimitStyleCommands(node);
                case @"\lim":
                    return HandleLimitCommands(node);
                default:
                    string baseVal = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, node.Command);
                    string screenReaderSymbolVal = Dictionaries.ScreenReaderSymbolMap.GetValueOrDefault(node.Command, baseVal);
                    return Dictionaries.ExceptionalScreenReaderSymbolMap.GetValueOrDefault(node.Command, screenReaderSymbolVal);
            }
        }

        private string HandleSQRT(CommandNode node)
        {
            var content = node.Args[0].Accept(this);
            if (node.Args[0] is GroupNode)
            {
                return $"the square root of ({content})";
            }
            return $"the square root of {content}";
        }

        private string HandleMathbb(CommandNode node)
        {
            if (node.Args[0].Accept(this).Replace("(", "").Replace(")", "") == "R")
            {
                return "the set of real numbers";
            }
            return node.Args[0].Accept(this);
        }

        private string HandleStyledText(CommandNode node)
        {
            var command = node.Command;
            if (command == @"\text" || command == @"\mathrm" || command == @"\textrm")
            {
                return node.Args[0].Accept(this);
            }
            var style = command.Substring(1).Replace("math", "");
            return $"{style} {node.Args[0].Accept(this)}";
        }

        private string HandleLimitCommands(CommandNode node)
        {
            var sub_lim = node.Subscript != null ? node.Subscript.ExceptionalAccept(this) : "";
            return $"limit as {sub_lim} of";
        }

        private string HandleLimitStyleCommands(CommandNode node)
        {
            var sub = node.Subscript != null ? node.Subscript.Accept(this) : "";
            var sup = node.Superscript != null ? node.Superscript.Accept(this) : "";
            string commandName = Dictionaries.SymbolMap.GetValueOrDefault(node.Command, "");
            return $"{commandName} from {sub} to {sup}";
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

            return $"a {num_rows}x{num_cols} matrix with rows {string.Join(" and ", matrix_desc)}";
        }

        public override string ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
        }
    }
}

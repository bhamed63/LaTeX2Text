using System.Linq;
using System.Text;
using LatexConverter.Ast;

namespace LatexConverter.Visitors
{
    public class LatexStringVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode textNode)
        {
            return textNode.Text;
        }

        public override string VisitGroup(GroupNode groupNode)
        {
            return "{" + string.Concat(groupNode.Body.Select(n => n.Accept(this))) + "}";
        }

        public override string VisitCommand(CommandNode commandNode)
        {
            if (LatexConverter.Data.RawData.SymbolLibrary.TryGetValue(commandNode.Command, out var symbolDef) && symbolDef.CommandType == CommandType.Formatting)
            {
                return string.Concat(commandNode.Args.Select(arg => arg.Accept(this).Trim('{', '}')));
            }

            var sb = new StringBuilder();
            sb.Append(commandNode.Command);

            if (commandNode.Args.Any())
            {
                foreach (var arg in commandNode.Args)
                {
                    sb.Append(arg.Accept(this));
                }
            }

            if (commandNode.Superscript != null)
            {
                sb.Append("^" + commandNode.Superscript.Accept(this));
            }

            if (commandNode.Subscript != null)
            {
                sb.Append("_" + commandNode.Subscript.Accept(this));
            }

            return sb.ToString();
        }

        public override string VisitScript(ScriptNode scriptNode)
        {
            var sb = new StringBuilder();
            sb.Append(scriptNode.Base.Accept(this));
            sb.Append(scriptNode.IsSuperscript ? "^" : "_");
            if (!scriptNode.IsSuperscript && scriptNode.Script is not GroupNode)
                sb.Append("{");
            sb.Append(scriptNode.Script.Accept(this));
            if (!scriptNode.IsSuperscript && scriptNode.Script is not GroupNode)
                sb.Append("}");
            return sb.ToString();
        }

        public override string VisitMatrix(MatrixNode matrixNode)
        {
            return matrixNode.Content;
        }

        public override string VisitRoot(RootNode rootNode)
        {
            var sb = new StringBuilder();
            sb.Append(CommandNames.Nthrt);
            sb.Append("{" + rootNode.Degree.Accept(this) + "}");
            sb.Append("{" + rootNode.Radicand.Accept(this) + "}");
            return sb.ToString();
        }

        public override string ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override string VisitFrac(FracNode fracNode)
        {
            return $"{CommandNames.Frac}{{{fracNode.Numerator.Accept(this)}}}{{{fracNode.Denominator.Accept(this)}}}";
        }

        public override string VisitBinom(BinomNode binomNode)
        {
            return $"{CommandNames.Binom}{{{binomNode.Top.Accept(this)}}}{{{binomNode.Bottom.Accept(this)}}}";
        }

        public override string VisitMath(MathNode node)
        {
            return string.Concat(node.Children.Select(n => n.Accept(this)));
        }
        public override string ExceptionalVisitMath(MathNode node)
        {
            return VisitMath(node);
        }

        public override string VisitLim(LimNode node)
        {
            return node.Command + "_{" + node.Subscript.Accept(this) + "}";
        }

        public override string VisitRelationalOperator(RelationalOperatorNode node)
        {
            return node.ToString();
        }
    }
}

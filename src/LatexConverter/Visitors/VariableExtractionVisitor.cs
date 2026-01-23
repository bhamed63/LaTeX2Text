using System.Collections.Generic;
using System.Linq;
using System.Text;
using LatexConverter.Ast;

namespace LatexConverter.Visitors
{
    public class VariableExtractionVisitor : BaseVisitor<List<string>>
    {
        private readonly LatexStringVisitor _latexVisitor = new LatexStringVisitor();
        private bool _isMathContext = false;

        public List<string> Visit(AstNode node, bool isMathContext)
        {
            var visitor = new VariableExtractionVisitor();
            visitor._isMathContext = isMathContext;
            return node.Accept(visitor);
        }
        public override List<string> VisitText(TextNode textNode)
        {
            if (_isMathContext && !string.IsNullOrWhiteSpace(textNode.Text))
            {
                var variables = new List<string>();
                var spaceParts = textNode.Text.Split(' ');
                foreach (var spacePart in spaceParts)
                {
                    var slashParts = spacePart.Split('/');
                    foreach (var part in slashParts)
                    {
                        var trimmedPart = part.Trim();
                        if (!string.IsNullOrEmpty(trimmedPart) &&
                            !double.TryParse(trimmedPart, out _) &&
                            trimmedPart != "=")
                        {
                            variables.Add(trimmedPart);
                        }
                    }
                }
                return variables;
            }
            return new List<string>();
        }

        public override List<string> VisitGroup(GroupNode groupNode)
        {
            return groupNode.Body.SelectMany(node => Visit(node, _isMathContext)).ToList();
        }

        public override List<string> VisitCommand(CommandNode commandNode)
        {
            if (commandNode.Command == CommandNames.LeftParen)
            {
                _isMathContext = true;
                return new List<string>();
            }

            if (commandNode.Command == CommandNames.RightParen)
            {
                _isMathContext = false;
                return new List<string>();
            }

            if (LatexConverter.Data.RawData.SymbolLibrary.TryGetValue(commandNode.Command, out var symbolDef))
            {
                switch (symbolDef.CommandType)
                {
                    case CommandType.MathFunction:
                        var variables = new List<string>();
                        foreach (var arg in commandNode.Args)
                        {
                            variables.AddRange(Visit(arg, true));
                        }
                        if (commandNode.Superscript != null)
                        {
                            variables.AddRange(Visit(commandNode.Superscript, true));
                        }
                        if (commandNode.Subscript != null)
                        {
                            variables.AddRange(Visit(commandNode.Subscript, true));
                        }
                        return variables;

                    case CommandType.Formatting:
                        return commandNode.Args.SelectMany(arg => Visit(arg, true)).ToList();

                    case CommandType.GreekLetter:
                    default:
                        return new List<string> { commandNode.Accept(_latexVisitor) };
                }
            }

            return new List<string> { commandNode.Accept(_latexVisitor) };
        }

        public override List<string> VisitScript(ScriptNode scriptNode)
        {
            return new List<string> { scriptNode.Accept(_latexVisitor) };
        }

        public override List<string> VisitMatrix(MatrixNode matrixNode)
        {
            var tokens = Tokenizer.Tokenize(matrixNode.Content);
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            return nodes.SelectMany(node => Visit(node, true)).ToList();
        }

        public override List<string> VisitRoot(RootNode rootNode)
        {
            return Visit(rootNode.Radicand, true);
        }

        public override List<string> ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override List<string> VisitFrac(FracNode fracNode)
        {
            var variables = new List<string>();
            variables.AddRange(Visit(fracNode.Numerator, true));
            variables.AddRange(Visit(fracNode.Denominator, true));
            return variables;
        }

        public override List<string> VisitBinom(BinomNode binomNode)
        {
            var variables = new List<string>();
            variables.AddRange(Visit(binomNode.Top, true));
            variables.AddRange(Visit(binomNode.Bottom, true));
            return variables;
        }

        public override List<string> VisitMath(MathNode node)
        {
            return node.Children.SelectMany(child => Visit(child, true)).ToList();
        }
        public override List<string> ExceptionalVisitMath(MathNode node)
        {
            return VisitMath(node);
        }

        public override List<string> VisitLim(LimNode node)
        {
            return Visit(node.Subscript, true);
        }

        public override List<string> VisitRelationalOperator(RelationalOperatorNode node)
        {
            var variables = new List<string>();
            variables.AddRange(Visit(node.LeftOperand, true));
            variables.AddRange(Visit(node.RightOperand, true));
            return variables;
        }

        public override List<string> VisitAbsoluteValue(AbsoluteValueNode node)
        {
            return Visit(node.InnerGroup, true);
        }
    }
}

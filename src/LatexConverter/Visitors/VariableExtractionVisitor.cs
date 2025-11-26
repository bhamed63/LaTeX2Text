using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Visitors
{
    public class VariableExtractionVisitor : BaseVisitor<List<string>>
    {
        private readonly LatexStringVisitor _latexVisitor = new LatexStringVisitor();
        private bool _isMathContext = false;

        // Public entry point. Safe for multiple calls on the same instance (e.g., in a loop).
        public List<string> Visit(AstNode node, bool isMathContext)
        {
            var originalContext = _isMathContext;
            _isMathContext = isMathContext;
            try
            {
                return node.Accept(this);
            }
            finally
            {
                _isMathContext = originalContext;
            }
        }

        // Private helper for recursive visits that require a specific context,
        // ensuring the original context is restored afterwards.
        private List<string> VisitChildWithNewContext(AstNode node, bool newContext)
        {
            var originalContext = _isMathContext;
            _isMathContext = newContext;
            try
            {
                return node.Accept(this);
            }
            finally
            {
                _isMathContext = originalContext;
            }
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
            // The context doesn't change for the children of a group, so we can just accept them.
            return groupNode.Body.SelectMany(node => node.Accept(this)).ToList();
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
                            variables.AddRange(VisitChildWithNewContext(arg, true));
                        }
                        if (commandNode.Superscript != null)
                        {
                            variables.AddRange(VisitChildWithNewContext(commandNode.Superscript, true));
                        }
                        if (commandNode.Subscript != null)
                        {
                            variables.AddRange(VisitChildWithNewContext(commandNode.Subscript, true));
                        }
                        return variables;

                    case CommandType.Formatting:
                        return commandNode.Args.SelectMany(arg => VisitChildWithNewContext(arg, true)).ToList();

                    case CommandType.Symbol:
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
            return nodes.SelectMany(node => VisitChildWithNewContext(node, true)).ToList();
        }

        public override List<string> VisitRoot(RootNode rootNode)
        {
            return VisitChildWithNewContext(rootNode.Radicand, true);
        }

        public override List<string> ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override List<string> VisitFrac(FracNode fracNode)
        {
            var variables = new List<string>();
            variables.AddRange(VisitChildWithNewContext(fracNode.Numerator, true));
            variables.AddRange(VisitChildWithNewContext(fracNode.Denominator, true));
            return variables;
        }

        public override List<string> VisitBinom(BinomNode binomNode)
        {
            var variables = new List<string>();
            variables.AddRange(VisitChildWithNewContext(binomNode.Top, true));
            variables.AddRange(VisitChildWithNewContext(binomNode.Bottom, true));
            return variables;
        }
    }
}

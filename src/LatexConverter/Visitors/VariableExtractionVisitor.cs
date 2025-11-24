using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Visitors
{
    public class VariableExtractionVisitor : BaseVisitor<List<string>>
    {
        private readonly LatexStringVisitor _latexVisitor = new LatexStringVisitor();

        public override List<string> VisitText(TextNode textNode)
        {
            if (!string.IsNullOrWhiteSpace(textNode.Text))
            {
                return new List<string> { textNode.Text };
            }
            return new List<string>();
        }

        public override List<string> VisitGroup(GroupNode groupNode)
        {
            return groupNode.Body.SelectMany(node => node.Accept(this)).ToList();
        }

        public override List<string> VisitCommand(CommandNode commandNode)
        {
            if (commandNode.Command == CommandNames.LeftParen)
            {
                return new List<string>();
            }

            if (commandNode.Command == CommandNames.RightParen)
            {
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
                            variables.AddRange(arg.Accept(this));
                        }
                        if (commandNode.Superscript != null)
                        {
                            variables.AddRange(commandNode.Superscript.Accept(this));
                        }
                        if (commandNode.Subscript != null)
                        {
                            variables.AddRange(commandNode.Subscript.Accept(this));
                        }
                        return variables;

                    case CommandType.Formatting:
                        if (commandNode.Command == CommandNames.Textrm)
                        {
                             var text = commandNode.Args.FirstOrDefault()?.Accept(new PlainTextVisitor()) ?? "";
                             return text.Split('/').ToList();
                        }
                        return commandNode.Args.SelectMany(arg => arg.Accept(this)).ToList();

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
            return nodes.SelectMany(node => node.Accept(this)).ToList();
        }

        public override List<string> VisitRoot(RootNode rootNode)
        {
            return rootNode.Radicand.Accept(this);
        }

        public override List<string> ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override List<string> VisitFrac(FracNode fracNode)
        {
            var variables = new List<string>();
            variables.AddRange(fracNode.Numerator.Accept(this));
            variables.AddRange(fracNode.Denominator.Accept(this));
            return variables;
        }

        public override List<string> VisitBinom(BinomNode binomNode)
        {
            var variables = new List<string>();
            variables.AddRange(binomNode.Top.Accept(this));
            variables.AddRange(binomNode.Bottom.Accept(this));
            return variables;
        }
    }
}

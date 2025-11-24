using System.Linq;

namespace LatexConverter.Visitors
{
    public class ComponentExtractionVisitor : BaseVisitor<object>
    {
        private readonly ExtractedComponents _components;
        private readonly SimplifiedLatexVisitor _latexVisitor = new SimplifiedLatexVisitor();

        public ComponentExtractionVisitor(ExtractedComponents components)
        {
            _components = components;
        }

        public override object VisitText(TextNode textNode)
        {
            return null;
        }

        public override object VisitGroup(GroupNode groupNode)
        {
            foreach (var node in groupNode.Body)
            {
                node.Accept(this);
            }
            return null;
        }

        public override object VisitCommand(CommandNode commandNode)
        {
            if (LatexConverter.Data.RawData.SymbolLibrary.TryGetValue(commandNode.Command, out var symbolDef))
            {
                switch (symbolDef.CommandType)
                {
                    case CommandType.MathFunction:
                        _components.FunctionsAndCommands.Add(commandNode.Command);
                        foreach (var arg in commandNode.Args) arg.Accept(this);
                        commandNode.Superscript?.Accept(this);
                        commandNode.Subscript?.Accept(this);
                        break;

                    case CommandType.Formatting:
                        foreach (var arg in commandNode.Args) arg.Accept(this);
                        break;

                    case CommandType.Symbol:
                    default:
                        _components.Variables.Add(commandNode.Accept(_latexVisitor));
                        break;
                }
            }
            else
            {
                _components.Variables.Add(commandNode.Accept(_latexVisitor));
            }
            return null;
        }

        public override object VisitScript(ScriptNode scriptNode)
        {
            _components.Variables.Add(scriptNode.Accept(_latexVisitor));
            return null;
        }

        public override object VisitMatrix(MatrixNode matrixNode)
        {
            _components.FunctionsAndCommands.Add(CommandNames.Matrix);
            var tokens = Tokenizer.Tokenize(matrixNode.Content);
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            foreach (var node in nodes)
            {
                node.Accept(this);
            }
            return null;
        }

        public override object VisitRoot(RootNode rootNode)
        {
            _components.FunctionsAndCommands.Add(CommandNames.Sqrt);
            rootNode.Radicand.Accept(this);
            return null;
        }

        public override object ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override object VisitFrac(FracNode fracNode)
        {
            _components.FunctionsAndCommands.Add(CommandNames.Frac);
            fracNode.Numerator.Accept(this);
            fracNode.Denominator.Accept(this);
            return null;
        }

        public override object VisitBinom(BinomNode binomNode)
        {
            _components.FunctionsAndCommands.Add(CommandNames.Binom);
            binomNode.Top.Accept(this);
            binomNode.Bottom.Accept(this);
            return null;
        }
    }
}

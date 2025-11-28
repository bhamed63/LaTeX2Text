using System;
using System.Collections.Generic;
using System.Linq;
using LatexConverter.Data;

namespace LatexConverter.Visitors
{
    public class ComponentExtractionVisitor : BaseVisitor<object>
    {
        public ExtractionResult Result { get; } = new ExtractionResult();
        private readonly HashSet<AstNode> _visitedNodes = new HashSet<AstNode>();

        public override object VisitText(TextNode textNode)
        {
            if (_visitedNodes.Contains(textNode))
            {
                return null;
            }
            _visitedNodes.Add(textNode);

            if (string.IsNullOrWhiteSpace(textNode.Text))
            {
                return null;
            }

            var text = textNode.Text;
            var i = 0;
            while (i < text.Length)
            {
                if (char.IsDigit(text[i]))
                {
                    var j = i;
                    while (j < text.Length && (char.IsDigit(text[j]) || text[j] == '.'))
                    {
                        j++;
                    }
                    Result.Numbers.Add(text.Substring(i, j - i));
                    i = j;
                }
                else if (RawData.Operators.Contains(text[i].ToString()))
                {
                    Result.Operators.Add(text[i].ToString());
                    i++;
                }
                else if (char.IsLetter(text[i]))
                {
                    var j = i;
                    while (j < text.Length && char.IsLetter(text[j]))
                    {
                        j++;
                    }
                    Result.Variables.Add(text.Substring(i, j - i));
                    i = j;
                }
                else
                {
                    i++;
                }
            }
            return null;
        }

        public override object VisitGroup(GroupNode groupNode)
        {
            if (_visitedNodes.Contains(groupNode))
            {
                return null;
            }
            _visitedNodes.Add(groupNode);

            foreach (var node in groupNode.Body)
            {
                node.Accept(this);
            }
            return null;
        }

        public override object VisitCommand(CommandNode commandNode)
        {
            if (_visitedNodes.Contains(commandNode))
            {
                return null;
            }
            _visitedNodes.Add(commandNode);

            if (RawData.SymbolLibrary.TryGetValue(commandNode.Command, out var symbolDef))
            {
                Result.Commands.Add(new Tuple<CommandType, string>(symbolDef.CommandType, commandNode.Command));
            }

            foreach (var arg in commandNode.Args)
            {
                arg.Accept(this);
            }
            if (commandNode.Superscript != null)
            {
                commandNode.Superscript.Accept(this);
            }
            if (commandNode.Subscript != null)
            {
                commandNode.Subscript.Accept(this);
            }
            return null;
        }

        public override object VisitScript(ScriptNode scriptNode)
        {
            if (_visitedNodes.Contains(scriptNode))
            {
                return null;
            }
            _visitedNodes.Add(scriptNode);

            scriptNode.Base.Accept(this);
            scriptNode.Script.Accept(this);
            return null;
        }

        public override object VisitMatrix(MatrixNode matrixNode)
        {
            if (_visitedNodes.Contains(matrixNode))
            {
                return null;
            }
            _visitedNodes.Add(matrixNode);

            Result.Commands.Add(new Tuple<CommandType, string>(CommandType.Matrix, CommandNames.Matrix));
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
            if (_visitedNodes.Contains(rootNode))
            {
                return null;
            }
            _visitedNodes.Add(rootNode);

            Result.Commands.Add(new Tuple<CommandType, string>(CommandType.MathFunction, CommandNames.Sqrt));
            rootNode.Radicand.Accept(this);
            if (rootNode.Degree != null)
            {
                rootNode.Degree.Accept(this);
            }
            return null;
        }

        public override object ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override object VisitFrac(FracNode fracNode)
        {
            if (_visitedNodes.Contains(fracNode))
            {
                return null;
            }
            _visitedNodes.Add(fracNode);

            Result.Commands.Add(new Tuple<CommandType, string>(CommandType.MathFunction, CommandNames.Frac));
            fracNode.Numerator.Accept(this);
            fracNode.Denominator.Accept(this);
            return null;
        }

        public override object VisitBinom(BinomNode binomNode)
        {
            if (_visitedNodes.Contains(binomNode))
            {
                return null;
            }
            _visitedNodes.Add(binomNode);

            Result.Commands.Add(new Tuple<CommandType, string>(CommandType.MathFunction, CommandNames.Binom));
            binomNode.Top.Accept(this);
            binomNode.Bottom.Accept(this);
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using LatexConverter.Ast;

namespace LatexConverter.Visitors
{
    public class CommandExtractionVisitor : BaseVisitor<List<string>>
    {
        public override List<string> VisitText(TextNode textNode)
        {
            return new List<string>();
        }

        public override List<string> VisitGroup(GroupNode groupNode)
        {
            return groupNode.Body.SelectMany(node => node.Accept(this)).ToList();
        }

        public override List<string> VisitCommand(CommandNode commandNode)
        {
            var commands = new List<string>();
            if (LatexConverter.Data.RawData.SymbolLibrary.TryGetValue(commandNode.Command, out var symbolDef))
            {
                if (symbolDef.CommandType == CommandType.MathFunction)
                {
                    commands.Add(commandNode.Command);
                }
            }

            foreach (var arg in commandNode.Args)
            {
                commands.AddRange(arg.Accept(this));
            }
            if (commandNode.Superscript != null)
            {
                commands.AddRange(commandNode.Superscript.Accept(this));
            }
            if (commandNode.Subscript != null)
            {
                commands.AddRange(commandNode.Subscript.Accept(this));
            }
            return commands;
        }

        public override List<string> VisitScript(ScriptNode scriptNode)
        {
            return scriptNode.Base.Accept(this).Concat(scriptNode.Script.Accept(this)).ToList();
        }

        public override List<string> VisitMatrix(MatrixNode matrixNode)
        {
            var commands = new List<string> { CommandNames.Matrix };
            var tokens = Tokenizer.Tokenize(matrixNode.Content);
            var parser = new Parser(tokens);
            var nodes = parser.Parse();
            commands.AddRange(nodes.SelectMany(node => node.Accept(this)));
            return commands;
        }

        public override List<string> VisitRoot(RootNode rootNode)
        {
            var commands = new List<string> { CommandNames.Sqrt };
            commands.AddRange(rootNode.Radicand.Accept(this));
            commands.AddRange(rootNode.Degree.Accept(this));
            return commands;
        }

        public override List<string> ExceptionalVisitRoot(RootNode node)
        {
            return VisitRoot(node);
        }

        public override List<string> VisitFrac(FracNode fracNode)
        {
            var commands = new List<string> { CommandNames.Frac };
            commands.AddRange(fracNode.Numerator.Accept(this));
            commands.AddRange(fracNode.Denominator.Accept(this));
            return commands;
        }

        public override List<string> VisitBinom(BinomNode binomNode)
        {
            var commands = new List<string> { CommandNames.Binom };
            commands.AddRange(binomNode.Top.Accept(this));
            commands.AddRange(binomNode.Bottom.Accept(this));
            return commands;
        }

        public override List<string> VisitMath(MathNode node)
        {
            return node.Children.SelectMany(child => child.Accept(this)).ToList();
        }
        public override List<string> ExceptionalVisitMath(MathNode node)
        {
            return VisitMath(node);
        }

        public override List<string> VisitLim(LimNode node)
        {
            var commands = new List<string> { node.Command };
            commands.AddRange(node.Subscript.Accept(this));
            return commands;
        }

        public override List<string> VisitRelationalOperator(RelationalOperatorNode node)
        {
            var commands = new List<string> { node.OperatorName };
            commands.AddRange(node.LeftOperand.Accept(this));
            commands.AddRange(node.RightOperand.Accept(this));
            return commands;
        }

        public override List<string> VisitAbsoluteValue(AbsoluteValueNode node)
        {
            var commands = new List<string> { "abs" };
            commands.AddRange(node.InnerGroup.Accept(this));
            return commands;
        }

        public override List<string> VisitPrescript(PrescriptNode node)
        {
            var commands = new List<string> { "prescript" };
            commands.AddRange(node.Script.Accept(this));
            commands.AddRange(node.Base.Accept(this));
            return commands;
        }
    }
}

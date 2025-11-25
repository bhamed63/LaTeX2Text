using System.Text;
using LatexConverter.Ast;

namespace LatexConverter.Visitors
{
    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor : IVisitor<string>
    {
        public abstract string VisitText(TextNode node);
        public abstract string VisitCommand(CommandNode node);
        public abstract string VisitGroup(GroupNode node);
        public abstract string VisitScript(ScriptNode node);
        public abstract string VisitMatrix(MatrixNode node);
        public abstract string VisitRoot(RootNode node);
        public abstract string VisitFrac(FracNode node);
        public abstract string VisitBinom(BinomNode node);

        public virtual string VisitMath(MathNode node)
        {
            var sb = new StringBuilder();
            foreach (var child in node.Content)
            {
                sb.Append(child.Accept(this));
            }
            return sb.ToString();
        }

        public virtual string ExceptionalVisitText(TextNode node) => VisitText(node);
        public virtual string ExceptionalVisitCommand(CommandNode node) => VisitCommand(node);
        public virtual string ExceptionalVisitGroup(GroupNode node) => VisitGroup(node);
        public virtual string ExceptionalVisitScript(ScriptNode node) => VisitScript(node);
        public virtual string ExceptionalVisitMatrix(MatrixNode node) => VisitMatrix(node);
        public abstract string ExceptionalVisitRoot(RootNode node);
        public virtual string ExceptionalVisitFrac(FracNode node) => VisitFrac(node);
        public virtual string ExceptionalVisitBinom(BinomNode node) => VisitBinom(node);
        public virtual string ExceptionalVisitMath(MathNode node) => VisitMath(node);

        public virtual string GetPreProcessedResult(string text) => text;
    }
}

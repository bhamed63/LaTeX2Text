namespace LatexConverter
{
    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
        public abstract T VisitRoot(RootNode node);
        public abstract T VisitFrac(FracNode node);
        public abstract T VisitBinom(BinomNode node);
        public abstract T VisitMath(MathNode node);

        public virtual T ExceptionalVisitText(TextNode node)
        {
            return VisitText(node);
        }
        public virtual T ExceptionalVisitCommand(CommandNode node)
        {
            return VisitCommand(node);
        }

        public virtual T ExceptionalVisitGroup(GroupNode node)
        {
            return VisitGroup(node);
        }



        public virtual T ExceptionalVisitScript(ScriptNode node)
        {
            return VisitScript(node);
        }

        public virtual T ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
        }

        public abstract T ExceptionalVisitRoot(RootNode node);

        public virtual T ExceptionalVisitFrac(FracNode node)
        {
            return VisitFrac(node);
        }

        public virtual T ExceptionalVisitBinom(BinomNode node)
        {
            return VisitBinom(node);
        }
        public abstract T VisitLim(LimNode node);
        public virtual T ExceptionalVisitLim(LimNode node)
        {
            return VisitLim(node);
        }
        public virtual T ExceptionalVisitMath(MathNode node)
        {
            return VisitMath(node);
        }

        public virtual string GetPreProcessedResult(string text)
        {
            return text;
        }
    }
}

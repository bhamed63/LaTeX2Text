
namespace LatexConverter
{
    /// <summary>
    /// Defines the interface for a visitor that traverses the AST.
    /// </summary>
    public interface IVisitor<T>
    {
        T VisitText(TextNode node);
        T ExceptionalVisitText(TextNode node);
        T VisitCommand(CommandNode node);
        T ExceptionalVisitCommand(CommandNode node);
        T VisitGroup(GroupNode node);
        T ExceptionalVisitGroup(GroupNode node);
        T VisitScript(ScriptNode node);
        T ExceptionalVisitScript(ScriptNode node);
        T VisitMatrix(MatrixNode node);
        T ExceptionalVisitMatrix(MatrixNode node);
    }

    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T ExceptionalVisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T ExceptionalVisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T ExceptionalVisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T ExceptionalVisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
        public abstract T ExceptionalVisitMatrix(MatrixNode node);
    }
}

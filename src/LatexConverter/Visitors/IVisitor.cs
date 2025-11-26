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
        T VisitRoot(RootNode node);
        T ExceptionalVisitRoot(RootNode node);
        T VisitFrac(FracNode node);
        T ExceptionalVisitFrac(FracNode node);
        T VisitBinom(BinomNode node);
        T ExceptionalVisitBinom(BinomNode node);
    }
}

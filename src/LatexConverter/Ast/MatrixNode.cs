namespace LatexConverter
{
    /// <summary>
    /// Represents a matrix node in the AST.
    /// </summary>
    public record MatrixNode(string Content) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitMatrix(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitMatrix(this);
    }
}

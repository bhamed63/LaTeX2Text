namespace LatexConverter
{
    /// <summary>
    /// Represents a square root node in the AST.
    /// </summary>
    public record SqrtNode(AstNode Argument) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitSqrt(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitSqrt(this);
    }
}

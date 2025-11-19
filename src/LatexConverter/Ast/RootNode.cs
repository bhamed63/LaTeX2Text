namespace LatexConverter
{
    public record RootNode(AstNode Radicand, AstNode Degree) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitRoot(this);
        }

        public override T ExceptionalAccept<T>(IVisitor<T> visitor)
        {
            return visitor.ExceptionalVisitRoot(this);
        }
    }
}

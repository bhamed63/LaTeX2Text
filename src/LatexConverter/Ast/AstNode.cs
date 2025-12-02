namespace LatexConverter
{
    /// <summary>
    /// Base class for all nodes in the Abstract Syntax Tree (AST).
    /// </summary>
    public abstract record AstNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
        public abstract T ExceptionalAccept<T>(IVisitor<T> visitor);
        public virtual bool NeedsParentheses() => false;

        public abstract List<AstNode> GetAllSubNodes();

        public abstract string CreateCommandName();
    }
}

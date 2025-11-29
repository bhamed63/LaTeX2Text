namespace LatexConverter
{
    /// <summary>
    /// Represents a subscript or superscript node in the AST.
    /// </summary>
    public record ScriptNode(AstNode Base, AstNode Script, bool IsSuperscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitScript(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitScript(this);

        public override string ToString()
        {
            var scriptChar = IsSuperscript ? '^' : '_';
            if (Script is TextNode textNode && textNode.Text.Length == 1)
            {
                return $"{Base}{scriptChar}{Script}";
            }
            return $"{Base}{scriptChar}{{{Script}}}";
        }
    }
}

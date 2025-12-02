namespace LatexConverter
{
    /// <summary>
    /// Represents a fraction node in the AST.
    /// </summary>
    public record FracNode(string Command, AstNode Numerator, AstNode Denominator) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitFrac(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitFrac(this);

        public override string ToString() => $"{this.CreateCommandName()}{{{Numerator}}}{{{Denominator}}}";


        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>()
            {
                Numerator, Denominator
            };
        }

        public override string CreateCommandName()
        {
            return $"\\frac";
        }
    }
}

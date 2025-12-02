
namespace LatexConverter
{
    /// <summary>
    /// Represents a binomial coefficient node in the AST.
    /// </summary>
    public record BinomNode(string Command, AstNode Top, AstNode Bottom) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinom(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitBinom(this);

        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>()
            {
                Top, Bottom
            };
        }

        public override string CreateCommandName()
        {
            return $"\\binom";
        }
    }
}

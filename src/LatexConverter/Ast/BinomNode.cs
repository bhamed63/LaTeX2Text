using System.Collections.Generic;

namespace LatexConverter
{
    public record BinomNode(string Command, AstNode Top, AstNode Bottom) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinom(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitBinom(this);

        public override string ToString() => $"\\binom{{{Top}}}{{{Bottom}}}";


        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>()
            {
                Top, Bottom
            };
        }
    }
}

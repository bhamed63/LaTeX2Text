using System.Collections.Generic;

namespace LatexConverter.Ast
{
    public record MathNode(IReadOnlyList<AstNode> Content) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitMath(this);
        }

        public override T ExceptionalAccept<T>(IVisitor<T> visitor)
        {
            return visitor.ExceptionalVisitMath(this);
        }
    }
}

using System.Collections.Generic;

namespace LatexConverter
{
    /// <summary>
    /// Represents a math environment in the AST.
    /// </summary>
    public record MathNode(List<AstNode> Children) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitMath(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitMath(this);
    }
}

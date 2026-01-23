using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Ast
{
    /// <summary>
    /// Represents an absolute value node in the AST, enclosed in vertical bars.
    /// </summary>
    public record AbsoluteValueNode(GroupNode InnerGroup) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitAbsoluteValue(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitAbsoluteValue(this);

        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode> { InnerGroup };
        }

        public override string CreateCommandName()
        {
            return "abs";
        }

        public override string ToString()
        {
            return $"|{InnerGroup.ToString()}|";
        }
    }
}

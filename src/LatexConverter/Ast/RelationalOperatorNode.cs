using System.Collections.Generic;

namespace LatexConverter.Ast
{
    public record RelationalOperatorNode(AstNode LeftOperand, AstNode RightOperand, string OperatorName, IEnumerable<AstNode> OriginalNodes) : AstNode
    {
        private readonly GroupNode _originalNodes = new GroupNode(OriginalNodes);

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitRelationalOperator(this);
        }

        public override T ExceptionalAccept<T>(IVisitor<T> visitor)
        {
            return visitor.ExceptionalVisitRelationalOperator(this);
        }

        public override List<AstNode> GetAllSubNodes()
        {
            var subNodes = new List<AstNode>();
            subNodes.AddRange(LeftOperand.GetAllSubNodes());
            subNodes.AddRange(RightOperand.GetAllSubNodes());
            return subNodes;
        }

        public override string CreateCommandName() => OperatorName;

        public override string ToString()
        {
            return _originalNodes.ToString();
        }
    }
}

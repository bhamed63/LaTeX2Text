using LatexConverter.Parsing;
using System.Collections.Generic;

namespace LatexConverter.Ast
{
    public record RelationalOperatorNode(AstNode LeftOperand, string OperatorName, AstNode RightOperand) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override T ExceptionalAccept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override List<AstNode> GetAllSubNodes()
        {
            var nodes = new List<AstNode> { LeftOperand, RightOperand };
            nodes.AddRange(LeftOperand.GetAllSubNodes());
            nodes.AddRange(RightOperand.GetAllSubNodes());
            return nodes;
        }

        public override string CreateCommandName()
        {
            return OperatorName;
        }

        public override string ToString()
        {
            var op = OperatorName.Length > 1 ? $"\\{OperatorName}" : OperatorName;
            return $"{LeftOperand} {op} {RightOperand}";
        }
    }
}

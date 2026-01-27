using System.Collections.Generic;

namespace LatexConverter.Ast
{
    /// <summary>
    /// Represents a prescript node in the AST, where a script (superscript or subscript) 
    /// is applied to a following base node (e.g., isotopes like ^{235}U).
    /// </summary>
    public record PrescriptNode(ScriptNode Script, AstNode Base) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrescript(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitPrescript(this);

        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode> { Script, Base };
        }

        public override string CreateCommandName()
        {
            return "prescript";
        }

        public override string ToString()
        {
            return $"{Script.ToString()}{Base.ToString()}";
        }
    }
}

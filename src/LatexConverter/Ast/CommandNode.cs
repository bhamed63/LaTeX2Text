using System.Collections.Generic;

namespace LatexConverter
{
    /// <summary>
    /// Represents a command node in the AST.
    /// </summary>
    public record CommandNode(string Command, List<AstNode> Args, AstNode Subscript, AstNode Superscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCommand(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitCommand(this);
    }
}

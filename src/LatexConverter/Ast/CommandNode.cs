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

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"\\{Command}");
            if (Args != null)
            {
                foreach (var arg in Args)
                {
                    sb.Append($"{{{arg}}}");
                }
            }
            if (Subscript != null)
            {
                sb.Append($"_{{{Subscript}}}");
            }
            if (Superscript != null)
            {
                sb.Append($"^{{{Superscript}}}");
            }
            return sb.ToString();
        }

        public override List<AstNode> GetAllSubNodes()
        {
            return Args.ToList();
        }
    }
}

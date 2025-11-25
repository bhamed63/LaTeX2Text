using System.Text.RegularExpressions;

namespace LatexConverter
{
    /// <summary>
    /// Represents a text node in the AST.
    /// </summary>
    public record TextNode(string Text) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitText(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitText(this);

        public override bool NeedsParentheses() => !Regex.IsMatch(Text, @"^[a-zA-Z0-9]+$");
    }
}

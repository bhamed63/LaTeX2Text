using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
    /// <summary>
    /// Represents a group of nodes in the AST, typically enclosed in braces.
    /// </summary>
    public record GroupNode(List<AstNode> Body, string OpenDelimiter, string CloseDelimiter) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroup(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitGroup(this);

        public override bool NeedsParentheses() => this.Body.Count > 1 || this.Body.Any(b => b is ScriptNode scriptNode && scriptNode.Script is GroupNode);

        public override string ToString() => $"{OpenDelimiter}{string.Join("", Body.Select(n => n.ToString()))}{CloseDelimiter}";
    }
}

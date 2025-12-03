using System.Collections.Generic;
using System.Text.RegularExpressions;
using LatexConverter.Ast;

namespace LatexConverter
{
    /// <summary>
    /// Represents a text node in the AST.
    /// </summary>
    public record TextNode(string Text) : AstNode
    {
        public List<OperatorNode> Operators { get; init; } = new List<OperatorNode>();

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitText(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitText(this);

        public override bool NeedsParentheses() => !Regex.IsMatch(Text, @"^[a-zA-Z0-9]+$");

        public override string ToString() => Text;


        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>() { };
        }

        public override string CreateCommandName()
        {
            return $"\\{Text}";
        }
    }
}

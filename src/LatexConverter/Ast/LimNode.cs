using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatexConverter
{
    public record LimNode(string Command, List<AstNode> Args, AstNode Subscript) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLim(this);
        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitLim(this);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"\\{Command}");
            if (Args != null && Args.Any())
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
            return sb.ToString();
        }

        public override List<AstNode> GetAllSubNodes()
        {
            var nodes = new List<AstNode>();
            if (Args != null)
            {
                nodes.AddRange(Args);
            }
            if (Subscript != null)
            {
                nodes.Add(Subscript);
            }
            return nodes;
        }
    }
}

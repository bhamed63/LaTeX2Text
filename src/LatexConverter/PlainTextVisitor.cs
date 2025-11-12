
using System.Linq;

namespace LatexConverter
{
    /// <summary>
    /// A visitor that extracts the plain text from the AST, without any formatting.
    /// </summary>
    public class PlainTextVisitor : BaseVisitor<string>
    {
        public override string VisitText(TextNode node) => node.Text;

        public override string ExceptionalVisitText(TextNode node) => node.Text;

        public override string VisitGroup(GroupNode node) => string.Concat(node.Body.Select(n => n.Accept(this)));

        public override string ExceptionalVisitGroup(GroupNode node) => string.Concat(node.Body.Select(n => n.ExceptionalAccept(this)));

        public override string VisitScript(ScriptNode node) => node.Base.Accept(this) + node.Script.Accept(this);

        public override string ExceptionalVisitScript(ScriptNode node) => node.Base.ExceptionalAccept(this) + node.Script.ExceptionalAccept(this);

        public override string VisitCommand(CommandNode node) => string.Concat(node.Args.Select(n => n.Accept(this)));

        public override string ExceptionalVisitCommand(CommandNode node) => string.Concat(node.Args.Select(n => n.ExceptionalAccept(this)));

        public override string VisitMatrix(MatrixNode node) => node.Content;

        public override string ExceptionalVisitMatrix(MatrixNode node) => node.Content;
    }
}

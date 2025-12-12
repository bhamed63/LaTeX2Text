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
            sb.Append(CreateCommandName());
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
            var result = Args.ToList();
            if (Subscript is not null)
                result.Add(Subscript);
            if (Superscript is not null)
                result.Add(Superscript);
            return result;
        }

        public override string CreateCommandName()
        {
            return $"\\{Command}";
        }

        public string CreateSpecialCommandText()
        {
            return $"{this.CreateCommandName()}{{{this.Args[0].ToString()}}}";
        }

        public string CreateSpecialCommandLabel()
        {
            if (CreateCommandName() == "\\hat")
                return $"{this.Args[0].ToString()}&#x0302;";

            if (CreateCommandName() == "\\vec")
                return $"{this.Args[0].ToString()}&#x20d7;";

            return "";
        }

        public bool IsSpecialCommandNode()
        {
            return CreateCommandName() == "\\hat" ||
                 CreateCommandName() == "\\vec";
        }
    }
}

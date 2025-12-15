namespace LatexConverter
{
    public record RootNode(string Command, AstNode Radicand, AstNode Degree) : AstNode
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitRoot(this);
        }

        public override T ExceptionalAccept<T>(IVisitor<T> visitor)
        {
            return visitor.ExceptionalVisitRoot(this);
        }

        public override string ToString()
        {
            if (Degree is TextNode textNode && textNode.Text == "2")
            {
                return $"{this.CreateCommandName()}{{{Radicand}}}";
            }
            return $"{this.CreateCommandName()}[{Degree}]{{{Radicand}}}";
        }

        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>()
            {
                Degree, Radicand
            };
        }

        public override string CreateCommandName()
        {
            if (Degree is TextNode textNode && textNode.Text != "2")
                return $"\\sqrt[]";

            if (Degree is GroupNode groupNode && groupNode.Body.Count == 1 && groupNode.Body.First() is TextNode textNode2 && textNode2.Text != "2")
                return $"\\sqrt[]";
            return $"\\sqrt";
        }
    }
}

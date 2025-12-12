
namespace LatexConverter
{
    /// <summary>
    /// Represents a subscript or superscript node in the AST.
    /// </summary>
    public record ScriptNode(AstNode Base, AstNode Script, bool IsSuperscript) : AstNode
    {
        private char getScriptChar()
        {
            return IsSuperscript ? '^' : '_';
        }

        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitScript(this);

        public override T ExceptionalAccept<T>(IVisitor<T> visitor) => visitor.ExceptionalVisitScript(this);

        public override string ToString()
        {
            char scriptChar = getScriptChar();
            if (Script is TextNode textNode && textNode.Text.Length == 1)
            {
                return $"{Base}{scriptChar}{Script}";
            }
            return $"{Base}{scriptChar}{{{Script}}}";
        }


        public override List<AstNode> GetAllSubNodes()
        {
            return new List<AstNode>()
            {
                Base, Script
            };
        }

        internal string ToVariableName()
        {
            if(IsSuperscript)
                return Base.ToString();;
            return ToString();
        }

        internal string ToSpecialBaseCommandText()
        {
            char scriptChar = getScriptChar();
            var baseCommandNode = this.Base as CommandNode;
            if (IsSuperscript)
                return $"{baseCommandNode.CreateCommandName()}{{{baseCommandNode.Args[0]}}}{scriptChar}{{{Script}}}";
            return $"{baseCommandNode.CreateCommandName()}{{{baseCommandNode.Args[0]}}}{scriptChar}{{{Script}}}";
        }

        internal string ToSpecialBaseCommandLabel()
        {
            char scriptChar = getScriptChar();
            var baseCommandNode = this.Base as CommandNode;
            if (IsSuperscript)
                return $"{baseCommandNode.CreateSpecialCommandLabel()}{scriptChar}{{{Script}}}";
            return $"{baseCommandNode.CreateSpecialCommandLabel()}{scriptChar}{{{Script}}}";
        }

        public override string CreateCommandName()
        {
            return this.ToString();
        }
    }
}

namespace LatexConverter
{
    /// <summary>
    /// Base class for all nodes in the Abstract Syntax Tree (AST).
    /// </summary>
    public abstract record AstNode
    {
        public abstract T Accept<T>(IVisitor<T> visitor);
        public abstract T ExceptionalAccept<T>(IVisitor<T> visitor);
        public virtual bool NeedsParentheses() => false;

        public abstract List<AstNode> GetAllSubNodes();

        public abstract string CreateCommandName();
        public abstract override string ToString();

        public virtual bool IsGreekLetter()
        {
            var commandName = this.CreateCommandName();
            if (commandName.StartsWith("\\"))
                commandName = commandName.Substring(1);
            return findSimilarGreekLetters(commandName).Count == 1;
        }

        private List<string> findSimilarGreekLetters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            Dictionary<string, SymbolDefinition> symbols = Dictionaries.GreekLetters;

            input = input.Trim();

            var symbol = symbols.Where(kvp =>
                input.StartsWith(kvp.Key) ||
                input.StartsWith(kvp.Value.PlainText ?? ""))
                .Select(c => c.Key.Substring(1))
                .ToList();

            if (symbol.Count == 0)
                symbol = symbols.Where(kvp =>
                    input.Contains(kvp.Key) ||
                    input.Contains(kvp.Value.PlainText ?? ""))
                    .Select(c => c.Key.Substring(1))
                    .ToList();

            return symbol;
        }

    }
}

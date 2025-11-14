using System.Collections.Generic;

namespace LatexConverter
{
    /// <summary>
    /// Parses a sequence of tokens into an Abstract Syntax Tree (AST).
    /// </summary>
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _pos;
        private Token CurrentToken => _tokens[_pos];

        public Parser(List<Token> tokens) { _tokens = tokens; }

        /// <summary>
        /// Parses the tokens into a list of AST nodes.
        /// </summary>
        /// <returns>A list of AST nodes.</returns>
        public List<AstNode> Parse()
        {
            var nodes = new List<AstNode>();
            while (CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }
            return nodes;
        }

        private AstNode ParseExpression()
        {
            var node = ParsePrimary();
            while (CurrentToken.Type == TokenType.Superscript || CurrentToken.Type == TokenType.Subscript)
            {
                bool isSuperscript = CurrentToken.Type == TokenType.Superscript;
                _pos++;
                var script = ParsePrimary();

                node = new ScriptNode(node, script, isSuperscript);
            }
            return node;
        }

        private AstNode ParsePrimary()
        {
            if (CurrentToken.Type == TokenType.LBrace) return ParseGroup();
            if (CurrentToken.Type == TokenType.Command) return ParseCommand();
            if (CurrentToken.Type == TokenType.Matrix)
            {
                var token = CurrentToken;
                _pos++;
                return new MatrixNode(token.Value);
            }
            if (CurrentToken.Type == TokenType.Text || CurrentToken.Type == TokenType.Space)
            {
                var token = CurrentToken;
                _pos++;
                return new TextNode(token.Value);
            }
            _pos++;
            return new TextNode("");
        }

        private AstNode ParseGroup()
        {
            _pos++; // Consume '{'
            var nodes = new List<AstNode>();
            while (CurrentToken.Type != TokenType.RBrace && CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }
            if (CurrentToken.Type == TokenType.RBrace) _pos++; // Consume '}'
            return new GroupNode(nodes);
        }

        private bool IsLimitCommand(string command) => command == CommandNames.Sum || command == CommandNames.Int || command == CommandNames.Prod || command == CommandNames.Lim;

        private int GetArgumentCount(string command)
        {
            return command switch
            {
                CommandNames.Frac or CommandNames.Binom => 2,
                CommandNames.Sqrt or CommandNames.Vec or CommandNames.Hat or CommandNames.Mathcal or CommandNames.Mathbb or CommandNames.Text or CommandNames.Mathrm or CommandNames.Textrm or CommandNames.Cos or CommandNames.Sin or CommandNames.Tan or CommandNames.Log or CommandNames.Ln or CommandNames.Exp or CommandNames.Det or CommandNames.Mathbf or CommandNames.Mathit or CommandNames.Mathsf or CommandNames.Mathtt or CommandNames.Mathfrak or CommandNames.Mathscr or CommandNames.Overline or CommandNames.Bar => 1,
                _ => 0,
            };
        }

        private AstNode ParseCommand()
        {
            var token = CurrentToken;
            _pos++;

            if (IsLimitCommand(token.Value))
            {
                return ParseLimitCommand(token);
            }

            var args = new List<AstNode>();
            int numArgs = GetArgumentCount(token.Value);
            for (int i = 0; i < numArgs; i++)
            {
                SkipSpaces();
                args.Add(ParsePrimary());
            }

            return new CommandNode(token.Value, args, null, null);
        }

        private void SkipSpaces()
        {
            while (CurrentToken.Type == TokenType.Space)
            {
                _pos++;
            }
        }

        private AstNode ParseLimitCommand(Token token)
        {
            AstNode subscript = null;
            AstNode superscript = null;

            if (CurrentToken.Type == TokenType.Subscript)
            {
                _pos++;
                subscript = ParsePrimary();
            }
            if (CurrentToken.Type == TokenType.Superscript)
            {
                _pos++;
                superscript = ParsePrimary();
            }

            return new CommandNode(token.Value, new List<AstNode>(), subscript, superscript);
        }
    }
}

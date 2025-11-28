using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
    public class HierarchicalParser
    {
        private readonly List<Token> _tokens;
        private int _pos;
        private Token CurrentToken => _tokens[_pos];

        public HierarchicalParser(List<Token> tokens) { _tokens = tokens; }

        public List<AstNode> Parse()
        {
            var nodes = new List<AstNode>();
            while (CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }

            var mergedNodes = new List<AstNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is TextNode textNode && string.IsNullOrEmpty(textNode.Text))
                {
                    continue;
                }
                if (mergedNodes.Count > 0 && nodes[i] is TextNode && mergedNodes.Last() is TextNode)
                {
                    var previousNode = (TextNode)mergedNodes.Last();
                    var currentNode = (TextNode)nodes[i];
                    mergedNodes[mergedNodes.Count - 1] = new TextNode(previousNode.Text + currentNode.Text);
                }
                else
                {
                    mergedNodes.Add(nodes[i]);
                }
            }

            return mergedNodes;
        }

        private AstNode ParseExpression()
        {
            var node = ParsePrimary();
            while (CurrentToken.Type == TokenType.Superscript || CurrentToken.Type == TokenType.Subscript)
            {
                bool isSuperscript = CurrentToken.Type == TokenType.Superscript;
                _pos++;
                var script = ParsePrimary();
                if (script is TextNode scriptText && scriptText.Text == "-")
                {
                    var nextToken = CurrentToken;
                    if (nextToken.Type == TokenType.Text)
                    {
                        _pos++;
                        script = new TextNode("-" + nextToken.Value);
                    }
                }

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
            var unhandledToken = CurrentToken;
            _pos++;
            return new TextNode(unhandledToken.Value);
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
                CommandNames.Vec or CommandNames.Hat or CommandNames.Mathcal or CommandNames.Mathbb or CommandNames.Text or CommandNames.Mathrm or CommandNames.Textrm or CommandNames.Cos or CommandNames.Sin or CommandNames.Tan or CommandNames.Log or CommandNames.Ln or CommandNames.Exp or CommandNames.Det or CommandNames.Mathbf or CommandNames.Mathit or CommandNames.Mathsf or CommandNames.Mathtt or CommandNames.Mathfrak or CommandNames.Mathscr or CommandNames.Overline or CommandNames.Bar => 1,
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

            switch (token.Value)
            {
                case CommandNames.BeginMath:
                case CommandNames.BeginMathDisplay:
                case CommandNames.BeginMathDisplay2:
                    return ParseMath(token.Value);
                case CommandNames.Sqrt:
                    return ParseRootCommand();
                case CommandNames.Frac:
                    return ParseFracCommand();
                case CommandNames.Binom:
                    return ParseBinomCommand();
                case CommandNames.Comment:
                    var commentArgs = new List<AstNode>();
                    if (CurrentToken.Type == TokenType.Text)
                    {
                        commentArgs.Add(ParsePrimary());
                    }
                    return new CommandNode(token.Value, commentArgs, null, null);
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

        private AstNode ParseRootCommand()
        {
            SkipSpaces();
            var degree = ParseOptionalArgument();
            SkipSpaces();
            var radicand = ParsePrimary();

            if (degree == null)
            {
                return new RootNode(radicand, new TextNode("2"));
            }
            else
            {
                return new RootNode(radicand, degree);
            }
        }

        private AstNode ParseOptionalArgument()
        {
            if (CurrentToken.Type == TokenType.Text && CurrentToken.Value == "[")
            {
                _pos++; // Consume '['
                var nodes = new List<AstNode>();
                while (!(CurrentToken.Type == TokenType.Text && CurrentToken.Value == "]") && CurrentToken.Type != TokenType.Eof)
                {
                    nodes.Add(ParseExpression());
                }
                if (CurrentToken.Type == TokenType.Text && CurrentToken.Value == "]")
                {
                    _pos++; // Consume ']'
                }
                return new GroupNode(nodes);
            }
            return null;
        }

        private AstNode ParseFracCommand()
        {
            SkipSpaces();
            var numerator = ParsePrimary();
            SkipSpaces();
            var denominator = ParsePrimary();
            return new FracNode(numerator, denominator);
        }

        private AstNode ParseBinomCommand()
        {
            SkipSpaces();
            var top = ParsePrimary();
            SkipSpaces();
            var bottom = ParsePrimary();
            return new BinomNode(top, bottom);
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

        private AstNode ParseMath(string beginCommand)
        {
            var endCommand = beginCommand switch
            {
                CommandNames.BeginMath => CommandNames.EndMath,
                CommandNames.BeginMathDisplay => CommandNames.EndMathDisplay,
                CommandNames.BeginMathDisplay2 => CommandNames.BeginMathDisplay2,
                _ => ""
            };

            var nodes = new List<AstNode>();
            while (!(CurrentToken.Type == TokenType.Command && CurrentToken.Value == endCommand) && CurrentToken.Type != TokenType.Eof)
            {
                nodes.Add(ParseExpression());
            }

            if (CurrentToken.Type == TokenType.Command && CurrentToken.Value == endCommand)
            {
                _pos++; // Consume end command
            }

            return new MathNode(nodes);
        }
    }
}

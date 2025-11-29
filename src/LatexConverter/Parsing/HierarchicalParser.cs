using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return MergeTextNodes(nodes);
        }

        private List<AstNode> MergeTextNodes(List<AstNode> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return new List<AstNode>();
            }

            var mergedNodes = new List<AstNode>();
            var textBuffer = new StringBuilder();

            foreach (var node in nodes)
            {
                if (node is TextNode textNode)
                {
                    textBuffer.Append(textNode.Text);
                }
                else if (node is CommandNode cmdNode && (cmdNode.Command == CommandNames.Thinspace || cmdNode.Command == @"\,"))
                {
                    textBuffer.Append(" ");
                }
                else
                {
                    if (textBuffer.Length > 0)
                    {
                        mergedNodes.Add(new TextNode(textBuffer.ToString()));
                        textBuffer.Clear();
                    }

                    if (node is GroupNode groupNode)
                    {
                        mergedNodes.Add(new GroupNode(MergeTextNodes(groupNode.Body), "", ""));
                    }
                    else
                    {
                        mergedNodes.Add(node);
                    }
                }
            }

            if (textBuffer.Length > 0)
            {
                mergedNodes.Add(new TextNode(textBuffer.ToString()));
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
            return new GroupNode(nodes, "", "");
        }

        private bool IsLimitCommand(string command) => command == CommandNames.Sum || command == CommandNames.Int || command == CommandNames.Prod || command == CommandNames.Lim;

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
                case @"\(":
                case @"\[":
                case "$$":
                    return ParseMath(token.Value);
                case CommandNames.Sqrt:
                    return ParseRootCommand();
                case CommandNames.Frac:
                    return ParseFracCommand();
                case CommandNames.Binom:
                    return ParseBinomCommand();
                case "%":
                    var commentArgs = new List<AstNode>();
                    if (CurrentToken.Type == TokenType.Text)
                    {
                        commentArgs.Add(ParsePrimary());
                    }
                    return new CommandNode(token.Value, commentArgs, null, null);
                    //case CommandNames.Times:
                    //    return new TextNode("×");
            }


            var args = new List<AstNode>();
            int numArgs = 0;
            switch (token.Value)
            {
                case CommandNames.Vec:
                case CommandNames.Hat:
                case CommandNames.Mathcal:
                case CommandNames.Mathbb:
                case CommandNames.Text:
                case CommandNames.Mathrm:
                case CommandNames.Textrm:
                case CommandNames.Mathbf:
                case CommandNames.Mathit:
                case CommandNames.Mathsf:
                case CommandNames.Mathtt:
                case CommandNames.Mathfrak:
                case CommandNames.Mathscr:
                case CommandNames.Overline:
                case CommandNames.Bar:
                    numArgs = 1;
                    break;
            }
            for (int i = 0; i < numArgs; i++)
            {
                SkipSpaces();
                args.Add(ParsePrimary());
            }

            SkipSpaces();
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
                return new GroupNode(nodes, "", "");
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
            //while (CurrentToken.Type == TokenType.Space)
            //{
            //    _pos++;
            //}
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

            SkipSpaces();
            return new CommandNode(token.Value, new List<AstNode>(), subscript, superscript);
        }

        private AstNode ParseMath(string beginCommand)
        {
            var endCommand = beginCommand switch
            {
                @"\(" => @"\)",
                @"\[" => @"\]",
                "$$" => "$$",
                _ => ""
            };

            var nodes = new List<AstNode>();
            while (!(CurrentToken.Type == TokenType.Command && CurrentToken.Value == endCommand) && CurrentToken.Type != TokenType.Eof)
            {
                if (CurrentToken.Type == TokenType.Space)
                {
                    _pos++;
                    continue;
                }
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

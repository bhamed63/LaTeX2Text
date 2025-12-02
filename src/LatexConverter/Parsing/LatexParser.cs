using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LatexConverter.Parsing.LatexCommandExtractor;

namespace LatexConverter.Parsing
{
    public class LatexParser
    {
        List<string> delimiters = new List<string> { ",", "/", ";", "\\", ";", " " };

        public List<AstNode> Parse(string input)
        {
            if (input == null)
                return new List<AstNode>();

            var nodes = new List<AstNode>();
            int position = 0;

            while (position < input.Length)
            {
                if (position + 1 < input.Length &&
                    input[position] == '\\' && (input[position + 1] == '(' || input[position + 1] == '['))
                {
                    var groupNode = ParseGroup(input, ref position);
                    if (groupNode != null)
                        nodes.Add(groupNode);
                }
                else if (input[position] == '\\')
                {
                    var commandNode = ParseCommand(input, ref position);
                    if (commandNode != null)
                        nodes.Add(commandNode);
                }
                else
                {
                    var textNodes = ParseText(input, ref position);
                    nodes.AddRange(textNodes);
                }
            }

            return nodes;
        }

        private List<AstNode> ParseText(string input, ref int position)
        {
            var nodes = new List<AstNode>();
            int start = position;

            while (position < input.Length)
            {
                if (input[position] == '\\')
                {
                    if (position + 1 < input.Length)
                    {
                        char nextChar = input[position + 1];
                        if (nextChar == '(' || nextChar == '[' || nextChar == ')' || nextChar == ']' || char.IsLetter(nextChar))
                        {
                            break;
                        }
                    }
                    position += 2;
                }
                else if (input[position] == '_' || input[position] == '^')
                {
                    string textBefore = input.Substring(start, position - start);


                    if (string.IsNullOrWhiteSpace(textBefore))
                    {
                        if (!string.IsNullOrEmpty(textBefore))
                        {
                            nodes.Add(new TextNode(textBefore));
                        }
                        nodes.Add(new TextNode(input[position].ToString()));
                        position++;
                        return nodes;
                    }

                    int baseEnd = position;
                    while (baseEnd > start && char.IsWhiteSpace(input[baseEnd - 1]))
                    {
                        baseEnd--;
                    }

                    int baseStart = baseEnd;
                    while (baseStart > start && !delimiters.Contains(input[baseStart - 1].ToString()))
                    {
                        baseStart--;
                    }

                    string precedingText = input.Substring(start, baseStart - start);
                    string baseText = input.Substring(baseStart, baseEnd - baseStart);


                    var notallowedForStart = new List<string> { ",", "/", ";", "\\", ";" };
                    while (baseText.Length > 0 && notallowedForStart.Contains(baseText[0].ToString()))
                    {
                        baseText = baseText.Substring(1);
                    }


                    if (!string.IsNullOrEmpty(precedingText))
                    {
                        nodes.Add(new TextNode(precedingText));
                    }
                    var baseNode = new TextNode(baseText);

                    AstNode finalNode = baseNode;
                    int currentPos = position;

                    while (true)
                    {
                        bool isSuperscript = input[currentPos] == '^';
                        currentPos++;

                        while (currentPos < input.Length && char.IsWhiteSpace(input[currentPos]))
                        {
                            currentPos++;
                        }

                        var scriptContent = ParseScriptContent(input, ref currentPos);
                        if (scriptContent == null)
                        {
                            scriptContent = new TextNode("");
                        }
                        finalNode = new ScriptNode(finalNode, scriptContent, isSuperscript);

                        int nextScriptPos = currentPos;
                        while (nextScriptPos < input.Length && char.IsWhiteSpace(input[nextScriptPos]))
                        {
                            nextScriptPos++;
                        }

                        if (nextScriptPos < input.Length && (input[nextScriptPos] == '_' || input[nextScriptPos] == '^'))
                        {
                            currentPos = nextScriptPos;
                        }
                        else
                        {
                            break;
                        }
                    }

                    nodes.Add(finalNode);
                    position = currentPos;
                    return nodes;
                }
                else
                {
                    position++;
                }
            }

            if (position > start)
            {
                nodes.Add(new TextNode(input.Substring(start, position - start)));
            }

            return nodes;
        }

        private AstNode ParseScriptContent(string input, ref int position)
        {
            if (position >= input.Length)
            {
                return null;
            }

            if (input[position] == '{')
            {
                position++; // Skip '{'
                var content = ExtractBracedContent(input, ref position);
                if (content != null)
                {
                    var contentNodes = Parse(content);
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }

                    if (contentNodes.Count == 1) return contentNodes[0];
                    if (contentNodes.Count > 1) return new GroupNode(contentNodes, "", "");
                    return new TextNode("");
                }
                else
                {
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }
                    return new TextNode("");
                }
            }

            if (input[position] == '\\')
            {
                return ParseCommand(input, ref position);
            }

            int start = position;
            while (position < input.Length)
            {
                char c = input[position];
                if (char.IsWhiteSpace(c) || c == '\\' || c == '}' || c == '_' || c == '^')
                {
                    break;
                }
                position++;
            }

            if (position > start)
            {
                return new TextNode(input.Substring(start, position - start));
            }

            return new TextNode("");
        }

        private GroupNode ParseGroup(string input, ref int position)
        {
            string openGroup = input.Substring(position, 2);
            string closeGroup = openGroup[0] + (openGroup[1] == '(' ? ")" : "]");

            position += 2;

            int start = position;
            int groupLevel = 1;

            while (position < input.Length && groupLevel > 0)
            {
                if (position + 1 < input.Length &&
                    input[position] == '\\' && input[position + 1] == openGroup[1])
                {
                    groupLevel++;
                    position += 2;
                }
                else if (position + 1 < input.Length &&
                         input[position] == '\\' && input[position + 1] == closeGroup[1])
                {
                    groupLevel--;
                    if (groupLevel == 0)
                    {
                        string groupContent = input.Substring(start, position - start);
                        var children = Parse(groupContent);
                        position += 2;
                        return new GroupNode(children, openGroup, closeGroup);
                    }
                    else
                    {
                        position += 2;
                    }
                }
                else
                {
                    position++;
                }
            }

            if (position > start)
            {
                string remainingContent = input.Substring(start, position - start);
                return new GroupNode(Parse(remainingContent), openGroup, closeGroup);
            }

            return new GroupNode(new List<AstNode>(), openGroup, closeGroup);
        }

        private AstNode ParseCommand(string input, ref int position)
        {
            position++;
            string commandName = ReadCommandName(input, ref position);

            if (IsSingleArgumentCommand(commandName))
            {
                var arg = ParseArgument(input, ref position);
                return new CommandNode(commandName, new List<AstNode> { arg }, null, null);
            }

            switch (commandName)
            {
                case "frac":
                    return ParseFracCommand(commandName, input, ref position);
                case "sqrt":
                    return ParseSqrtCommand(commandName, input, ref position);
                case "lim":
                case "sum":
                case "int":
                case "prod":
                    return ParseLimitStyleCommand(commandName, input, ref position);
                case "binom":
                    return ParseBinomCommand(commandName, input, ref position);
                default:
                    var arguments = new List<AstNode>();
                    while (position < input.Length && input[position] == '{')
                    {
                        position++; // Skip '{'
                        var argumentContent = ExtractBracedContent(input, ref position);
                        if (argumentContent != null)
                        {
                            arguments.AddRange(Parse(argumentContent));
                            if (position < input.Length && input[position] == '}')
                                position++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    return new CommandNode(commandName, arguments, null, null);
            }
        }

        private static readonly HashSet<string> SingleArgumentCommands = new HashSet<string>
        {
            "vec", "hat", "mathcal", "mathbb", "text", "mathrm", "textrm", "cos", "sin", "tan",
            "log", "ln", "exp", "det", "mathbf", "mathit", "mathsf", "mathtt", "mathfrak",
            "mathscr", "overline", "bar"
        };

        private bool IsSingleArgumentCommand(string commandName)
        {
            return SingleArgumentCommands.Contains(commandName);
        }

        private AstNode ParseArgument(string input, ref int position)
        {
            while (position < input.Length && char.IsWhiteSpace(input[position]))
            {
                position++;
            }

            if (position >= input.Length)
            {
                return new TextNode("");
            }

            if (input[position] == '{')
            {
                position++; // Consume '{'
                var content = ExtractBracedContent(input, ref position);
                position++; // Consume '}'
                var nodes = Parse(content);
                if (nodes.Count == 1) return nodes[0];
                return new GroupNode(nodes, "", "");
            }

            if (input[position] == '\\')
            {
                return ParseCommand(input, ref position);
            }

            return new TextNode(input[position++].ToString());
        }

        private AstNode ParseLimitStyleCommand(string commandName, string input, ref int position)
        {
            AstNode subscript = null;
            AstNode superscript = null;

            // First pass for either _ or ^
            int checkpoint = position;
            while (position < input.Length && char.IsWhiteSpace(input[position])) position++;

            if (position < input.Length && input[position] == '_')
            {
                position++;
                while (position < input.Length && char.IsWhiteSpace(input[position])) position++;
                subscript = ParseScriptContent(input, ref position);
            }
            else if (position < input.Length && input[position] == '^')
            {
                position++;
                while (position < input.Length && char.IsWhiteSpace(input[position])) position++;
                superscript = ParseScriptContent(input, ref position);
            }
            else
            {
                position = checkpoint;
            }

            // Second pass for the other script
            checkpoint = position;
            while (position < input.Length && char.IsWhiteSpace(input[position])) position++;

            if (subscript == null && position < input.Length && input[position] == '_')
            {
                position++;
                while (position < input.Length && char.IsWhiteSpace(input[position])) position++;
                subscript = ParseScriptContent(input, ref position);
            }
            else if (superscript == null && position < input.Length && input[position] == '^')
            {
                position++;
                while (position < input.Length && char.IsWhiteSpace(input[position])) position++;
                superscript = ParseScriptContent(input, ref position);
            }
            else
            {
                position = checkpoint;
            }

            return new CommandNode(commandName, new List<AstNode>(), subscript, superscript);
        }

        private FracNode ParseFracCommand(string commandName, string input, ref int position)
        {
            var numeratorArgs = ParseCommandArguments(input, ref position);
            var denominatorArgs = ParseCommandArguments(input, ref position);

            var numerator = new GroupNode(numeratorArgs, "", "");
            var denominator = new GroupNode(denominatorArgs, "", "");

            return new FracNode(commandName, numerator, denominator);
        }

        private RootNode ParseSqrtCommand(string commandName, string input, ref int position)
        {
            var degreeArgs = ParseOptionalArgument(input, ref position);
            var radicandArgs = ParseCommandArguments(input, ref position);

            AstNode degree = degreeArgs != null ? new GroupNode(degreeArgs, "", "") : new TextNode("2");
            var radicand = new GroupNode(radicandArgs, "", "");

            return new RootNode(commandName, radicand, degree);
        }

        private BinomNode ParseBinomCommand(string commandName, string input, ref int position)
        {
            var topArgs = ParseCommandArguments(input, ref position);
            var bottomArgs = ParseCommandArguments(input, ref position);

            var top = new GroupNode(topArgs, "", "");
            var bottom = new GroupNode(bottomArgs, "", "");

            return new BinomNode(commandName, top, bottom);
        }

        private List<AstNode> ParseOptionalArgument(string input, ref int position)
        {
            if (position < input.Length && input[position] == '[')
            {
                position++; // Skip '['
                var content = new StringBuilder();
                while (position < input.Length && input[position] != ']')
                {
                    content.Append(input[position]);
                    position++;
                }
                if (position < input.Length && input[position] == ']')
                {
                    position++; // Skip ']'
                }
                return Parse(content.ToString());
            }
            return null;
        }

        private List<AstNode> ParseCommandArguments(string input, ref int position)
        {
            var arguments = new List<AstNode>();
            if (position < input.Length && input[position] == '{')
            {
                position++; // Skip '{'
                var argumentContent = ExtractBracedContent(input, ref position);
                if (argumentContent != null)
                {
                    arguments.AddRange(Parse(argumentContent));
                    if (position < input.Length && input[position] == '}')
                        position++;
                }
            }
            return arguments;
        }

        private string ReadCommandName(string input, ref int position)
        {
            int start = position;
            while (position < input.Length && char.IsLetter(input[position]))
            {
                position++;
            }

            if (position == start)
            {
                if (position < input.Length)
                {
                    string specialChar = input[position].ToString();
                    position++;
                    return specialChar;
                }
                return "";
            }

            return input.Substring(start, position - start);
        }

        private string ExtractBracedContent(string input, ref int position)
        {
            int start = position;
            int braceLevel = 1;

            while (position < input.Length)
            {
                char currentChar = input[position];

                if (currentChar == '{')
                {
                    braceLevel++;
                }
                else if (currentChar == '}')
                {
                    braceLevel--;
                }

                if (braceLevel == 0)
                {
                    return input.Substring(start, position - start);
                }
                position++;
            }
            return null;
        }
    }

    public class LatexCommandExtractor
    {
        public class CommandInfo
        {
            public string CommandName { get; set; }
            public List<string> TextArguments { get; set; } = new List<string>();
            public int ArgumentCount => TextArguments.Count;

            public override string ToString()
            {
                if (TextArguments.Count == 0)
                    return $"\\{CommandName}";
                return $"\\{CommandName}{{{string.Join("}{", TextArguments)}}}";
            }
        }

        public List<CommandInfo> ExtractCommands(List<AstNode> nodes)
        {
            var commands = new List<CommandInfo>();
            ExtractCommandsRecursive(nodes, commands, null);
            return commands;
        }

        private void ExtractCommandsRecursive(List<AstNode> nodes, List<CommandInfo> commands, CommandInfo currentCommandInfo)
        {
            foreach (var node in nodes)
            {
                if (node is CommandNode cmdNode)
                {
                    var commandInfo = new CommandInfo { CommandName = cmdNode.Command };
                    foreach (var arg in cmdNode.Args)
                    {
                        if (arg is not TextNode)
                            continue;
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    commands.Add(commandInfo);
                    ExtractCommandsRecursive(cmdNode.Args.Where(c => c is not TextNode).ToList(), commands, commandInfo);
                }
                else if (node is FracNode fracNode)
                {
                    var commandInfo = new CommandInfo { CommandName = fracNode.Command };
                    var args = node.GetAllSubNodes();
                    foreach (var arg in args)
                    {
                        if (arg is not TextNode)
                            continue;
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    commands.Add(commandInfo);
                    ExtractCommandsRecursive(args.Where(c => c is not TextNode).ToList(), commands, commandInfo);
                }
                else if (node is RootNode rootNode)
                {
                    var commandInfo = new CommandInfo { CommandName = rootNode.Command };
                    var args = node.GetAllSubNodes();
                    foreach (var arg in args)
                    {
                        if (arg is not TextNode)
                            continue;
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    commands.Add(commandInfo);
                    ExtractCommandsRecursive(args.Where(c => c is not TextNode).ToList(), commands, commandInfo);
                }
                else if (node is GroupNode groupNode)
                {
                    foreach (var arg in groupNode.Body.OfType<TextNode>())
                    {
                        if (!isValidArgument(arg.Text))
                            continue;

                        CommandInfo commandInfo = new CommandInfo() { CommandName = arg.ToString() };
                        commands.Add(commandInfo);
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    ExtractCommandsRecursive(groupNode.Body.Where(c => c is not TextNode).ToList(), commands, null);
                }
                else if (node is ScriptNode scriptNode)
                {
                    CommandInfo commandInfo = null;
                    if (scriptNode.Base is TextNode && scriptNode.Script is TextNode)
                    {
                        if (isValidArgument(scriptNode.Base.ToString()))
                        {
                            //commandInfo = new CommandInfo { CommandName = "" };
                            commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName() };
                            commandInfo.TextArguments.Add(scriptNode.ToVariableName());
                            commands.Add(commandInfo);
                        }
                    }
                    else
                        ExtractCommandsRecursive(new List<AstNode> { scriptNode.Base, scriptNode.Script }, commands, commandInfo);
                }
                else if (node is TextNode textNode && currentCommandInfo != null)
                {
                    currentCommandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(textNode));
                }
            }
        }

        private List<string> ExtractTextContentIfArgument(AstNode node)
        {
            var textContents = new List<string>();
            if (node is TextNode textNode)
            {
                textContents.Add(textNode.Text);
            }
            else if (node is CommandNode commandNode)
            {
                textContents.AddRange(commandNode.Args.SelectMany(c => ExtractTextContentIfArgument(c)));
            }
            else if (node is GroupNode groupNode)
            {
                textContents.AddRange(groupNode.Body.SelectMany(c => ExtractTextContentIfArgument(c)));
            }
            else if (node is RootNode rootNode)
            {
                textContents.AddRange(ExtractTextContentIfArgument(rootNode.Radicand));
                textContents.AddRange(ExtractTextContentIfArgument(rootNode.Degree));
            }
            else if (node is ScriptNode scriptNode)
            {
                textContents.AddRange(ExtractTextContentIfArgument(scriptNode.Base));
                textContents.AddRange(ExtractTextContentIfArgument(scriptNode.Script));
            }

            return textContents
                .Where(c => isValidArgument(c))
                .ToList();
        }

        private bool isValidArgument(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();
            while (
                (text.StartsWith("(") && text.EndsWith(")")) ||
                (text.StartsWith("{") && text.EndsWith("}")) ||
                (text.StartsWith("[") && text.EndsWith("]")) ||
                (text.StartsWith("'") && text.EndsWith("'")) ||
                (text.StartsWith("\"") && text.EndsWith("\""))
                )
            {
                text = text.Substring(1, text.Length - 2);
            }

            var notAllowedForStart = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_"};

            var notAllowedForContain = new List<string>()
            {
                "{", "}", ")", "(",
                ",", ";", "'", "\"",
                "/", "\\", "="
            };

            if (notAllowedForStart.Any(c => text.StartsWith(c)))
                return false;

            if (notAllowedForContain.Any(c => text.Contains(c)))
                return false;

            if (text.Trim().Contains(" "))
                return false;

            return true;
        }
    }
}

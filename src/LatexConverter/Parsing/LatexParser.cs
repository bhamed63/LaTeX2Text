using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LatexConverter.Parsing.LatexCommandExtractor;

namespace LatexConverter.Parsing
{
    public class LatexParser
    {
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
                    // Found script character
                    if (position == start)
                    {
                        // Script at beginning - treat as text
                        nodes.Add(new TextNode(input.Substring(start, 1)));
                        position++;
                        return nodes;
                    }

                    // Check if there's whitespace immediately before the script
                    if (char.IsWhiteSpace(input[position - 1]))
                    {
                        // Script with whitespace before - treat script char as separate text
                        if (position > start)
                        {
                            nodes.Add(new TextNode(input.Substring(start, position - start)));
                        }
                        nodes.Add(new TextNode(input[position].ToString()));
                        position++;
                        return nodes;
                    }

                    // Find the base text (go backwards until whitespace or delimiter)
                    int baseEnd = position;
                    int baseStart = baseEnd;

                    // Go backwards to find the start of the base text
                    while (baseStart > start)
                    {
                        if (char.IsWhiteSpace(input[baseStart - 1]) ||
                            input[baseStart - 1] == '\\' ||
                            input[baseStart - 1] == '(' || input[baseStart - 1] == '[' ||
                            input[baseStart - 1] == ')' || input[baseStart - 1] == ']')
                        {
                            break;
                        }
                        baseStart--;
                    }

                    if (baseStart >= baseEnd)
                    {
                        // No valid base found
                        nodes.Add(new TextNode(input.Substring(start, position - start + 1)));
                        position++;
                        return nodes;
                    }

                    List<AstNode> resultNodes = new List<AstNode>();

                    // Add text before the base
                    if (baseStart > start)
                    {
                        string precedingText = input.Substring(start, baseStart - start);
                        resultNodes.Add(new TextNode(precedingText));
                    }

                    // Extract base text (can be multiple characters like "ab")
                    string baseText = input.Substring(baseStart, baseEnd - baseStart);
                    var baseNode = new TextNode(baseText);

                    // Parse script
                    bool isSuperscript = input[position] == '^';
                    position++;

                    var scriptContent = ParseScriptContent(input, ref position);

                    // For empty scripts like x_{}, still create ScriptNode
                    if (scriptContent == null)
                    {
                        scriptContent = new TextNode("");
                    }

                    var scriptNode = new ScriptNode(baseNode, scriptContent, isSuperscript);

                    // Check for consecutive scripts (like x_i_j or x^{2}_{i})
                    if (position < input.Length && (input[position] == '_' || input[position] == '^'))
                    {
                        // Parse the consecutive script with the current script as base
                        var consecutiveNodes = ParseConsecutiveScript(input, ref position, scriptNode);
                        resultNodes.AddRange(consecutiveNodes);
                    }
                    else
                    {
                        resultNodes.Add(scriptNode);
                    }

                    return resultNodes;
                }
                else
                {
                    position++;
                }
            }

            // Plain text without scripts
            if (position > start)
            {
                nodes.Add(new TextNode(input.Substring(start, position - start)));
            }

            return nodes;
        }

        private List<AstNode> ParseConsecutiveScript(string input, ref int position, AstNode currentBase)
        {
            var nodes = new List<AstNode>();

            while (position < input.Length && (input[position] == '_' || input[position] == '^'))
            {
                bool isSuperscript = input[position] == '^';
                position++;

                var scriptContent = ParseScriptContent(input, ref position);

                if (scriptContent == null)
                {
                    scriptContent = new TextNode("");
                }

                currentBase = new ScriptNode(currentBase, scriptContent, isSuperscript);
            }

            nodes.Add(currentBase);
            return nodes;
        }

        private AstNode ParseScriptContent(string input, ref int position)
        {
            if (position >= input.Length)
            {
                return null;
            }

            // Handle case where there's no content after script char (like "x_")
            if (position >= input.Length)
            {
                return null;
            }

            if (input[position] == '{')
            {
                int braceStart = position;
                position++; // Skip '{'

                var content = ExtractBracedContent(input, ref position);
                if (content != null)
                {
                    var contentNodes = Parse(content);

                    // Skip the closing brace
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }

                    if (contentNodes.Count == 1)
                    {
                        return contentNodes[0];
                    }
                    else if (contentNodes.Count > 1)
                    {
                        return new GroupNode(contentNodes, "", "");
                    }
                    else
                    {
                        return new TextNode("");
                    }
                }
                else
                {
                    // If extraction failed, return empty content
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }
                    return new TextNode("");
                }
            }
            else if (input[position] == '}' || input[position] == '{')
            {
                // Skip invalid script content
                position++;
                return new TextNode("");
            }
            else
            {
                // Single character script
                var singleCharNode = new TextNode(input[position].ToString());
                position++;
                return singleCharNode;
            }
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

            switch (commandName)
            {
                case "frac":
                    return ParseFracCommand(commandName, input, ref position);
                case "sqrt":
                    return ParseSqrtCommand(commandName, input, ref position);
                case "lim":
                    return ParseLimitCommand(commandName, input, ref position);
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

        private AstNode ParseLimitCommand(string commandName, string input, ref int position)
        {
            AstNode subscript = null;
            if (position < input.Length && input[position] == '_')
            {
                position++; // Skip '_'
                subscript = ParseScriptContent(input, ref position);
            }
            return new LimNode(commandName, new List<AstNode>(), subscript);
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

            while (position < input.Length && braceLevel > 0)
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
                    break;
                }

                position++;
            }

            if (braceLevel == 0 && start < position)
            {
                return input.Substring(start, position - start);
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
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    commands.Add(commandInfo);
                    // Recurse to find more commands, but pass null to avoid argument duplication
                    ExtractCommandsRecursive(cmdNode.Args, commands, null);
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
                    ExtractCommandsRecursive(args, commands, commandInfo);
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
                    ExtractCommandsRecursive(args, commands, commandInfo);
                }
                else if (node is GroupNode groupNode)
                {
                    foreach (var arg in groupNode.Body)
                    {
                        if (arg is not TextNode || currentCommandInfo == null)
                            continue;
                        currentCommandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    ExtractCommandsRecursive(groupNode.Body, commands, null);
                }
                else if (node is ScriptNode scriptNode)
                {
                    ExtractCommandsRecursive(new List<AstNode> { scriptNode.Base, scriptNode.Script }, commands, null);
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
            var notAllowedForStart = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            if (notAllowedForStart.Any(c => text.StartsWith(c)))
                return false;

            if (text.Trim().Contains(" "))
                return false;

            return true;
        }
    }
}

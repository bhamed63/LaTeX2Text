using LatexConverter.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexConverter.Parsing
{

    public class LatexParser
    {
        public List<LatexNode> Parse(string input)
        {
            if (input == null)
                return new List<LatexNode>();

            var nodes = new List<LatexNode>();
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

        private List<LatexNode> ParseText(string input, ref int position)
        {
            var nodes = new List<LatexNode>();
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
                        nodes.Add(new LatexTextNode(input.Substring(start, 1)));
                        position++;
                        return nodes;
                    }

                    // Check if there's whitespace immediately before the script
                    if (char.IsWhiteSpace(input[position - 1]))
                    {
                        // Script with whitespace before - treat script char as separate text
                        if (position > start)
                        {
                            nodes.Add(new LatexTextNode(input.Substring(start, position - start)));
                        }
                        nodes.Add(new LatexTextNode(input[position].ToString()));
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
                        nodes.Add(new LatexTextNode(input.Substring(start, position - start + 1)));
                        position++;
                        return nodes;
                    }

                    List<LatexNode> resultNodes = new List<LatexNode>();

                    // Add text before the base
                    if (baseStart > start)
                    {
                        string precedingText = input.Substring(start, baseStart - start);
                        resultNodes.Add(new LatexTextNode(precedingText));
                    }

                    // Extract base text (can be multiple characters like "ab")
                    string baseText = input.Substring(baseStart, baseEnd - baseStart);
                    var baseNode = new LatexTextNode(baseText);

                    // Parse script
                    char scriptChar = input[position];
                    var scriptType = scriptChar == '_' ? ScriptTypeEnum.Subscript : ScriptTypeEnum.Superscript;
                    position++;

                    var scriptContent = ParseScriptContent(input, ref position);

                    // For empty scripts like x_{}, still create ScriptNode
                    if (scriptContent == null)
                    {
                        scriptContent = new LatexTextNode("");
                    }

                    var scriptNode = new LatexScriptNode(scriptType, baseNode, scriptContent);

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
                nodes.Add(new LatexTextNode(input.Substring(start, position - start)));
            }

            return nodes;
        }

        private List<LatexNode> ParseConsecutiveScript(string input, ref int position, LatexNode currentBase)
        {
            var nodes = new List<LatexNode>();

            while (position < input.Length && (input[position] == '_' || input[position] == '^'))
            {
                char scriptChar = input[position];
                var scriptType = scriptChar == '_' ? ScriptTypeEnum.Subscript : ScriptTypeEnum.Superscript;
                position++;

                var scriptContent = ParseScriptContent(input, ref position);

                if (scriptContent == null)
                {
                    scriptContent = new LatexTextNode("");
                }

                currentBase = new LatexScriptNode(scriptType, currentBase, scriptContent);
            }

            nodes.Add(currentBase);
            return nodes;
        }

        private LatexNode ParseScriptContent(string input, ref int position)
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
                        return new LatexGroupNode("{", "}") { Children = contentNodes };
                    }
                    else
                    {
                        return new LatexTextNode("");
                    }
                }
                else
                {
                    // If extraction failed, return empty content
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }
                    return new LatexTextNode("");
                }
            }
            else if (input[position] == '}' || input[position] == '{')
            {
                // Skip invalid script content
                position++;
                return new LatexTextNode("");
            }
            else
            {
                // Single character script
                var singleCharNode = new LatexTextNode(input[position].ToString());
                position++;
                return singleCharNode;
            }
        }

        private LatexGroupNode ParseGroup(string input, ref int position)
        {
            string openGroup = input.Substring(position, 2);
            string closeGroup = openGroup[0] + (openGroup[1] == '(' ? ")" : "]");

            position += 2;

            var groupNode = new LatexGroupNode(openGroup, closeGroup);
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
                        groupNode.Children = Parse(groupContent);
                        position += 2;
                        return groupNode;
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
                groupNode.Children = Parse(remainingContent);
            }

            return groupNode;
        }

        private LatexCommandNode ParseCommand(string input, ref int position)
        {
            position++;

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
                    return new LatexCommandNode(specialChar);
                }
                return new LatexCommandNode("");
            }

            string commandName = input.Substring(start, position - start);
            var commandNode = new LatexCommandNode(commandName);

            while (position < input.Length && input[position] == '{')
            {
                int braceStart = position;
                position++; // Skip '{'

                var argumentContent = ExtractBracedContent(input, ref position);
                if (argumentContent != null)
                {
                    var argumentNodes = Parse(argumentContent);
                    commandNode.Args.AddRange(argumentNodes);

                    // Skip the closing brace
                    if (position < input.Length && input[position] == '}')
                    {
                        position++;
                    }
                }
                else
                {
                    break;
                }
            }

            return commandNode;
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

        public List<CommandInfo> ExtractCommands(List<LatexNode> nodes)
        {
            var commands = new List<CommandInfo>();
            ExtractCommandsRecursive(nodes, commands);
            return commands;
        }

        private void ExtractCommandsRecursive(List<LatexNode> nodes, List<CommandInfo> commands)
        {
            foreach (var node in nodes)
            {
                if (node is LatexCommandNode cmdNode)
                {
                    var commandInfo = new CommandInfo { CommandName = cmdNode.Command };

                    foreach (var arg in cmdNode.Args)
                    {
                        if (arg is not LatexTextNode)
                            continue;
                        commandInfo.TextArguments.Add(ExtractTextContent(arg));
                    }

                    commands.Add(commandInfo);

                    ExtractCommandsRecursive(cmdNode.Args, commands);
                }
                else if (node is LatexGroupNode groupNode)
                {
                    ExtractCommandsRecursive(groupNode.Children, commands);
                }
                else if (node is LatexScriptNode scriptNode)
                {
                    ExtractCommandsRecursive(new List<LatexNode> { scriptNode.Base, scriptNode.Script }, commands);
                }
            }
        }

        private string ExtractTextContent(LatexNode node)
        {
            if (node is LatexTextNode textNode)
            {
                return textNode.Text;
            }
            else if (node is LatexCommandNode cmdNode)
            {
                return node.ToString();
            }
            else if (node is LatexGroupNode groupNode)
            {
                return string.Join("", groupNode.Children.Select(ExtractTextContent));
            }
            else if (node is LatexScriptNode scriptNode)
            {
                return node.ToString();
            }
            return node.ToString();
        }
    }
}

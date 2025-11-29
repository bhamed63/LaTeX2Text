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
                char currentChar = input[position];

                if (currentChar == '\\')
                {
                    if (position + 1 < input.Length && (input[position + 1] == '(' || input[position + 1] == '['))
                    {
                        var groupNode = ParseGroup(input, ref position);
                        if (groupNode != null)
                            nodes.Add(groupNode);
                    }
                    else
                    {
                        var commandNode = ParseCommand(input, ref position);
                        if (commandNode != null)
                            nodes.Add(commandNode);
                    }
                }
                else if (currentChar == '_' || currentChar == '^')
                {
                    if (nodes.Any())
                    {
                        var baseNode = nodes.Last();
                        nodes.RemoveAt(nodes.Count - 1);
                        var scriptNodes = HandleScript(input, ref position, baseNode);
                        nodes.AddRange(scriptNodes);
                    }
                    else
                    {
                        nodes.Add(new LatexTextNode(currentChar.ToString()));
                        position++;
                    }
                }
                else
                {
                    var textNode = ParseText(input, ref position);
                    if (textNode != null)
                        nodes.Add(textNode);
                }
            }
            return nodes;
        }

        private LatexTextNode ParseText(string input, ref int position)
        {
            var sb = new StringBuilder();
            while (position < input.Length)
            {
                char c = input[position];

                if (c == '\\')
                {
                    if (position + 1 < input.Length)
                    {
                        char nextChar = input[position + 1];
                        if (char.IsLetter(nextChar) || nextChar == '(' || nextChar == '[')
                        {
                            break;
                        }
                        sb.Append(c);
                        sb.Append(nextChar);
                        position += 2;
                    }
                    else
                    {
                        sb.Append(c);
                        position++;
                    }
                }
                else if (c == '_' || c == '^')
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                    position++;
                }
            }

            if (sb.Length > 0)
            {
                return new LatexTextNode(sb.ToString());
            }

            return null;
        }

        private List<LatexNode> HandleScript(string input, ref int position, LatexNode baseNode)
        {
            if (baseNode is not LatexTextNode textBase || string.IsNullOrEmpty(textBase.Text))
            {
                var scriptTree = BuildScriptTree(input, ref position, baseNode);
                return new List<LatexNode> { scriptTree };
            }

            string baseContent = textBase.Text;
            if (char.IsWhiteSpace(baseContent.LastOrDefault()))
            {
                var result = new List<LatexNode>
                {
                    baseNode,
                    new LatexTextNode(input[position].ToString())
                };
                position++;
                return result;
            }

            int baseStart = baseContent.Length - 1;
            while (baseStart > 0)
            {
                if (char.IsWhiteSpace(baseContent[baseStart - 1]) ||
                    baseContent[baseStart - 1] == '\\' ||
                    baseContent[baseStart - 1] == '(' || baseContent[baseStart - 1] == '[' ||
                    baseContent[baseStart - 1] == ')' || baseContent[baseStart - 1] == ']')
                {
                    break;
                }
                baseStart--;
            }

            var precedingText = baseContent.Substring(0, baseStart);
            var actualBaseText = baseContent.Substring(baseStart);
            var newBaseNode = new LatexTextNode(actualBaseText);

            var resultingNodes = new List<LatexNode>();
            if (!string.IsNullOrEmpty(precedingText))
            {
                resultingNodes.Add(new LatexTextNode(precedingText));
            }

            var builtScriptTree = BuildScriptTree(input, ref position, newBaseNode);
            resultingNodes.Add(builtScriptTree);

            return resultingNodes;
        }

        private LatexNode BuildScriptTree(string input, ref int position, LatexNode currentBase)
        {
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
            return currentBase;
        }

        private LatexNode ParseScriptContent(string input, ref int position)
        {
            if (position >= input.Length)
                return null;

            if (input[position] == '{')
            {
                position++; // Skip '{'
                var content = ExtractBracedContent(input, ref position);
                if (content != null)
                {
                    var contentNodes = Parse(content);
                    if (position < input.Length && input[position] == '}')
                        position++; // Skip '}'

                    if (contentNodes.Count == 1)
                        return contentNodes[0];
                    if (contentNodes.Count > 1)
                        return new LatexGroupNode("{", "}") { Children = contentNodes };
                    return new LatexTextNode("");
                }
                else
                {
                    if (position < input.Length && input[position] == '}')
                        position++;
                    return new LatexTextNode("");
                }
            }
            else
            {
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
                if (position + 1 < input.Length && input.Substring(position, 2) == openGroup)
                {
                    groupLevel++;
                    position += 2;
                }
                else if (position + 1 < input.Length && input.Substring(position, 2) == closeGroup)
                {
                    groupLevel--;
                    if (groupLevel == 0)
                    {
                        string groupContent = input.Substring(start, position - start);
                        groupNode.Children = Parse(groupContent);
                        position += 2;
                        return groupNode;
                    }
                    position += 2;
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
            string commandName;
            if (position == start)
            {
                if (position < input.Length)
                {
                    commandName = input[position].ToString();
                    position++;
                }
                else
                {
                    commandName = "";
                }
            }
            else
            {
                commandName = input.Substring(start, position - start);
            }
            var commandNode = new LatexCommandNode(commandName);
            var arguments = ParseCommandArguments(input, ref position);
            commandNode.Args.AddRange(arguments);
            return commandNode;
        }

        private List<LatexNode> ParseCommandArguments(string input, ref int position)
        {
            var arguments = new List<LatexNode>();
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
            return arguments;
        }

        private string ExtractBracedContent(string input, ref int position)
        {
            int start = position;
            int braceLevel = 1;
            while (position < input.Length && braceLevel > 0)
            {
                char currentChar = input[position];
                if (currentChar == '{')
                    braceLevel++;
                else if (currentChar == '}')
                    braceLevel--;
                if (braceLevel == 0)
                    break;
                position++;
            }
            if (braceLevel == 0 && start <= position)
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
            else if (node is LatexCommandNode)
            {
                return node.ToString();
            }
            else if (node is LatexGroupNode groupNode)
            {
                return string.Join("", groupNode.Children.Select(ExtractTextContent));
            }
            else if (node is LatexScriptNode)
            {
                return node.ToString();
            }
            return node.ToString();
        }
    }
}

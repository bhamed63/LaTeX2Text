using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatexConverter.Parsing
{
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

        public List<string> ExtractArgumentsFromOperators(List<AstNode> nodes)
        {
            var arguments = new HashSet<string>();
            ExtractArgumentsRecursive(nodes, arguments);
            return arguments.ToList();
        }

        private void ExtractArgumentsRecursive(List<AstNode> nodes, HashSet<string> arguments)
        {
            foreach (var node in nodes)
            {
                if (node is TextNode textNode)
                {
                    if (textNode.Operators.Any())
                    {
                        foreach (var opNode in textNode.Operators)
                        {
                            ExtractFromRightOperand(opNode.RightOperand, arguments);
                            ExtractFromLeftOperand(opNode.LeftOperand, arguments);
                        }
                    }
                    else
                    {
                        if (isValidArgument(textNode.Text))
                        {
                            arguments.Add(textNode.Text.Trim());
                        }
                    }
                }
                else if (node is GroupNode groupNode)
                {
                    ExtractArgumentsRecursive(groupNode.Body, arguments);
                }
                else if (node is CommandNode cmdNode)
                {
                    ExtractArgumentsRecursive(cmdNode.Args, arguments);
                    if (cmdNode.Subscript != null)
                    {
                        ExtractArgumentsRecursive(new List<AstNode> { cmdNode.Subscript }, arguments);
                    }
                    if (cmdNode.Superscript != null)
                    {
                        ExtractArgumentsRecursive(new List<AstNode> { cmdNode.Superscript }, arguments);
                    }
                }
                else if (node is ScriptNode scriptNode)
                {
                    ExtractArgumentsRecursive(new List<AstNode> { scriptNode.Base, scriptNode.Script }, arguments);
                }
                else if (node is RootNode rootNode)
                {
                    ExtractArgumentsRecursive(new List<AstNode> { rootNode.Radicand }, arguments);
                    if (rootNode.Degree != null)
                    {
                        ExtractArgumentsRecursive(new List<AstNode> { rootNode.Degree }, arguments);
                    }
                }
                else if (node is FracNode fracNode)
                {
                    ExtractArgumentsRecursive(new List<AstNode> { fracNode.Numerator, fracNode.Denominator }, arguments);
                }
                else if (node is BinomNode binomNode)
                {
                    ExtractArgumentsRecursive(new List<AstNode> { binomNode.Top, binomNode.Bottom }, arguments);
                }
            }
        }

        private void ExtractFromRightOperand(string rightOperand, HashSet<string> arguments)
        {
            if (string.IsNullOrEmpty(rightOperand))
                return;

            int startIndex = 0;
            while (startIndex < rightOperand.Length && ParsingRules.IsScriptDelimiter(rightOperand[startIndex]))
            {
                startIndex++;
            }
            if (startIndex >= rightOperand.Length) return;

            int endIndex = startIndex;
            while (endIndex < rightOperand.Length && !ParsingRules.IsScriptDelimiter(rightOperand[endIndex]))
            {
                endIndex++;
            }

            string potentialArgument = rightOperand.Substring(startIndex, endIndex - startIndex);
            if (isValidArgument(potentialArgument))
            {
                arguments.Add(potentialArgument);
            }
        }

        private void ExtractFromLeftOperand(string leftOperand, HashSet<string> arguments)
        {
            if (string.IsNullOrEmpty(leftOperand))
                return;

            int endIndex = leftOperand.Length;
            while (endIndex > 0 && ParsingRules.IsScriptDelimiter(leftOperand[endIndex - 1]))
            {
                endIndex--;
            }
            if (endIndex == 0) return;

            int startIndex = endIndex;
            while (startIndex > 0 && !ParsingRules.IsScriptDelimiter(leftOperand[startIndex - 1]))
            {
                startIndex--;
            }

            string potentialArgument = leftOperand.Substring(startIndex, endIndex - startIndex);
            if (isValidArgument(potentialArgument))
            {
                arguments.Add(potentialArgument);
            }
        }

        private void ExtractCommandsRecursive(List<AstNode> nodes, List<CommandInfo> commands, CommandInfo currentCommandInfo)
        {
            foreach (var node in nodes)
            {
                if (node is CommandNode cmdNode)
                {
                    var commandInfo = new CommandInfo { CommandName = cmdNode.CreateCommandName() };
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
                    var commandInfo = new CommandInfo { CommandName = fracNode.CreateCommandName() };
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
                    var commandInfo = new CommandInfo { CommandName = rootNode.CreateCommandName() };
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
                    foreach (var arg in groupNode.Body.OfType<TextNode>())
                    {
                        if (!isValidArgument(arg.Text))
                            continue;

                        CommandInfo commandInfo = new CommandInfo() { CommandName = "" };
                        commands.Add(commandInfo);
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg));
                    }
                    ExtractCommandsRecursive(groupNode.Body, commands, null);
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
                    else if (scriptNode.Base is CommandNode commandNode &&
                        commandNode.Args.Count == 0 &&
                        commandNode.Subscript is null &&
                        commandNode.Superscript is null)
                    {
                        commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName() };
                        commandInfo.TextArguments.Add(scriptNode.ToVariableName());
                        commands.Add(commandInfo);
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

            var notAllowedForStart = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_" };

            var notAllowedForContain = new List<string>()
            {
                "{", "}", ")", "(",
                ",", ";", "'", "\"",
                "/", "\\", "=", "+"
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

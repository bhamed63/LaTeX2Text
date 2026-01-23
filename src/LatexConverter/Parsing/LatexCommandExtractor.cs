using LatexConverter.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LatexConverter.Parsing.LatexCommandExtractor;

namespace LatexConverter.Parsing
{
    public class LatexCommandExtractor
    {
        public class CommandInfo
        {
            public CommandInfo()
            {
                TextArguments = new List<string>();
                LabelArguments = new List<string>();
            }

            public string CommandName { get; set; }
            public List<string> TextArguments { get; set; } = new List<string>();
            public List<string> LabelArguments { get; set; } = new List<string>();
            public int ArgumentCount => TextArguments.Count;
            public bool IsSpecialCommand { get; set; }

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

        public List<CommandInfo> ExtractArgumentsFromOperators(List<AstNode> nodes)
        {
            var arguments = new List<CommandInfo>();
            ExtractArgumentsRecursive(nodes, arguments);
            return arguments.ToList();
        }

        public List<string> ExtractMathrmArguments(List<AstNode> nodes)
        {
            var arguments = new HashSet<string>();
            ExtractMathrmArgumentsRecursive(nodes, arguments);
            return arguments.ToList();
        }

        private void ExtractMathrmArgumentsRecursive(List<AstNode> nodes, HashSet<string> arguments)
        {
            foreach (var node in nodes)
            {
                if (node is TextNode textNode)
                    continue;

                else if (node is GroupNode groupNode)
                {
                    ExtractMathrmArgumentsRecursive(groupNode.Body, arguments);
                }
                else if (node is CommandNode cmdNode)
                {
                    if (cmdNode.CreateCommandName().ToLower() == "\\mathrm")
                    {
                        var textNodeArgs = cmdNode.Args.Where(c => c is TextNode).ToList();
                        if (textNodeArgs.Count == 0 && cmdNode.Args.First() is GroupNode)
                        {
                            arguments.Add(cmdNode.Args.First().ToString());
                        }
                        foreach (var arg in textNodeArgs)
                        {
                            var argValue = arg.ToString();
                            if (isValidRegularArgument(argValue))
                            {
                                arguments.Add(argValue);
                            }
                        }
                        //foreach (var groupNode2 in cmdNode.Args.Where(c => c is GroupNode))
                        //{
                        //    ExtractMathrmArgumentsRecursive((groupNode2 as GroupNode).Body, arguments);
                        //}
                    }

                    ExtractMathrmArgumentsRecursive(cmdNode.Args, arguments);
                    if (cmdNode.Subscript != null)
                    {
                        ExtractMathrmArgumentsRecursive(new List<AstNode> { cmdNode.Subscript }, arguments);
                    }
                    if (cmdNode.Superscript != null)
                    {
                        ExtractMathrmArgumentsRecursive(new List<AstNode> { cmdNode.Superscript }, arguments);
                    }
                }
                else if (node is ScriptNode scriptNode)
                {
                    ExtractMathrmArgumentsRecursive(new List<AstNode> { scriptNode.Base, scriptNode.Script }, arguments);
                }
                else if (node is RootNode rootNode)
                {
                    ExtractMathrmArgumentsRecursive(new List<AstNode> { rootNode.Radicand }, arguments);
                    if (rootNode.Degree != null)
                    {
                        ExtractMathrmArgumentsRecursive(new List<AstNode> { rootNode.Degree }, arguments);
                    }
                }
                else if (node is FracNode fracNode)
                {
                    ExtractMathrmArgumentsRecursive(new List<AstNode> { fracNode.Numerator, fracNode.Denominator }, arguments);
                }
                else if (node is BinomNode binomNode)
                {
                    ExtractMathrmArgumentsRecursive(new List<AstNode> { binomNode.Top, binomNode.Bottom }, arguments);
                }
            }
        }

        private void ExtractArgumentsRecursive(List<AstNode> nodes, List<CommandInfo> arguments)
        {
            foreach (var node in nodes)
            {
                if (node is TextNode textNode && textNode.Operators.Any())
                {
                    for (int i = 0; i < textNode.Operators.Count; i++)
                    {
                        var opNode = textNode.Operators[i];
                        CommandInfo commandInfo = new CommandInfo() { CommandName = opNode.Operator };
                        arguments.Add(commandInfo);
                        string rightOperand = ExtractFromRightOperand(opNode.RightOperand);
                        string leftOperand = ExtractFromLeftOperand(opNode.LeftOperand);
                        string appendedOperands = $"{leftOperand}{opNode.Operator}{rightOperand}";
                        if (isInAllowedAppendedOperands(appendedOperands))
                        {
                            if (opNode.Operator == "-" ||
                                opNode.Operator == "/" ||
                                opNode.Operator == "*" ||
                                opNode.Operator == "+")
                            {
                                if (isValidRegularArgument(rightOperand) && isValidRegularArgument(leftOperand))
                                {
                                    if ((opNode.LeftOperand.EndsWith(" ") && opNode.RightOperand.StartsWith(" "))
                                        ||
                                        (!opNode.LeftOperand.EndsWith(" ") && !opNode.RightOperand.StartsWith(" ")))
                                    {
                                        commandInfo.TextArguments.Add(rightOperand);
                                        commandInfo.TextArguments.Add(leftOperand);
                                    }
                                }
                                else if (isValidRegularArgument(rightOperand) && (isValidNumber(opNode.LeftOperand) || string.IsNullOrEmpty(opNode.LeftOperand)))
                                {
                                    commandInfo.TextArguments.Add(rightOperand);
                                }
                                else if ((isValidNumber(opNode.RightOperand) || string.IsNullOrEmpty(opNode.RightOperand)) && isValidRegularArgument(leftOperand))
                                {
                                    commandInfo.TextArguments.Add(rightOperand);
                                }
                            }
                            else
                            {
                                if (isValidRegularArgument(rightOperand))
                                    commandInfo.TextArguments.Add(rightOperand);

                                if (isValidRegularArgument(leftOperand))
                                    commandInfo.TextArguments.Add(leftOperand);

                            }
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

        private bool isValidNumber(string givenString)
        {
            return double.TryParse(givenString, out double d);
        }

        private string ExtractFromRightOperand(string rightOperand)
        {
            if (string.IsNullOrEmpty(rightOperand))
                return string.Empty;

            string text = rightOperand.Trim();

            int startIndex = 0;
            // Skip any leading delimiters (including spaces) to find the start of the argument.
            while (startIndex < text.Length && ParsingRules.SubscripDelimitor.Contains(text[startIndex]))
            {
                startIndex++;
            }

            if (startIndex >= text.Length)
                return string.Empty;

            // Now, find the end of the argument
            int endIndex = startIndex;
            while (endIndex < text.Length && !ParsingRules.SubscripDelimitor.Contains(text[endIndex]))
            {
                endIndex++;
            }

            string potentialArgument = text.Substring(startIndex, endIndex - startIndex);
            if (ParsingRules.NotValidVariableNames.Contains(potentialArgument) || isValidRegularArgument(potentialArgument))
            {
                return potentialArgument;
            }
            return string.Empty;
        }

        private string ExtractFromLeftOperand(string leftOperand)
        {
            if (string.IsNullOrEmpty(leftOperand))
                return string.Empty;

            leftOperand = leftOperand.Trim();

            int startIndex = leftOperand.Length - 1;
            while (startIndex >= 0 && !ParsingRules.SubscripDelimitor.Contains(leftOperand[startIndex]))
            {
                startIndex--;
            }

            string potentialArgument = leftOperand.Substring(startIndex + 1).Trim();
            if (isValidRegularArgument(potentialArgument))
            {
                return potentialArgument;
            }
            return string.Empty;
        }

        private void ExtractCommandsRecursive(List<AstNode> nodes, List<CommandInfo> commands, CommandInfo currentCommandInfo)
        {
            foreach (var node in nodes)
            {
                if (node is CommandNode cmdNode)
                {
                    if (cmdNode.IsSpecialCommandNode())
                    {
                        var commandInfo = new CommandInfo { CommandName = cmdNode.CreateSpecialCommandText(), IsSpecialCommand = true };
                        commandInfo.TextArguments.Add(cmdNode.CreateSpecialCommandText());
                        commandInfo.LabelArguments.Add(cmdNode.CreateSpecialCommandLabel());
                        commands.Add(commandInfo);
                    }
                    else
                    {
                        var commandInfo = new CommandInfo { CommandName = cmdNode.CreateCommandName() };
                        foreach (var arg in cmdNode.Args)
                        {
                            if (arg is not TextNode)
                                continue;
                            commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg, false));
                        }

                        if (cmdNode.IsGreekLetter() && cmdNode.Args.Count == 0)
                            commandInfo.TextArguments.Add(commandInfo.CommandName);

                        commands.Add(commandInfo);
                        ExtractCommandsRecursive(cmdNode.Args.Where(c => c is not TextNode).ToList(), commands, commandInfo);
                    }
                }
                else if (node is FracNode fracNode)
                {
                    var commandInfo = new CommandInfo { CommandName = fracNode.CreateCommandName() };
                    var args = node.GetAllSubNodes();
                    foreach (var arg in args)
                    {
                        if (arg is not TextNode)
                            continue;
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg, false));
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
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg, false));
                    }
                    commands.Add(commandInfo);
                    ExtractCommandsRecursive(args, commands, commandInfo);
                }
                else if (node is GroupNode groupNode)
                {
                    foreach (var arg in groupNode.Body.OfType<TextNode>())
                    {
                        if (!isValidRegularArgument(arg.Text))
                            continue;

                        CommandInfo commandInfo = new CommandInfo() { CommandName = "" };
                        commands.Add(commandInfo);
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(arg, false));
                    }
                    ExtractCommandsRecursive(groupNode.Body, commands, null);
                }
                else if (node is ScriptNode scriptNode)
                {
                    if (!isInAllowedCommands(scriptNode))
                        continue;

                    CommandInfo commandInfo = null;
                    if (scriptNode.Base is TextNode && scriptNode.Script is TextNode)
                    {
                        if (isValidBaseArgument(scriptNode.Base.ToString()))
                        {
                            //commandInfo = new CommandInfo { CommandName = "" };
                            commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName() };
                            commandInfo.TextArguments.Add(scriptNode.ToVariableName());
                            commands.Add(commandInfo);
                        }
                    }
                    else if (scriptNode.Base is TextNode && scriptNode.Script is GroupNode grpNode)
                    {
                        if (isValidBaseArgument(scriptNode.Base.ToString()))
                        {
                            //commandInfo = new CommandInfo { CommandName = "" };
                            commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName() };
                            commandInfo.TextArguments.Add(scriptNode.ToVariableName());
                            commands.Add(commandInfo);
                        }
                    }
                    else if (scriptNode.Base is TextNode && scriptNode.Script is CommandNode scriptCmdNode)
                    {
                        if (isValidBaseArgument(scriptNode.Base.ToString()))// && scriptCmdNode.IsGreekCommandNode())
                        {
                            //commandInfo = new CommandInfo { CommandName = "" };
                            commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName() };
                            commandInfo.TextArguments.Add(scriptNode.ToVariableName());
                            commands.Add(commandInfo);
                        }
                    }
                    else if (scriptNode.Base is CommandNode specialCommandNode &&
                        specialCommandNode.IsSpecialCommandNode() &&
                        specialCommandNode.Args.Count == 1)
                    {
                        commandInfo = new CommandInfo { CommandName = scriptNode.ToVariableName(), IsSpecialCommand = true };
                        commandInfo.TextArguments.Add(scriptNode.ToSpecialBaseCommandText());
                        commandInfo.LabelArguments.Add(scriptNode.ToSpecialBaseCommandLabel());
                        commands.Add(commandInfo);
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
                else if (node is RelationalOperatorNode relationalNode)
                {
                    CommandInfo commandInfo = new CommandInfo { CommandName = relationalNode.OperatorName };
                    if (relationalNode.LeftOperand is TextNode leftOperand)
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(leftOperand, true));
                    else
                        ExtractCommandsRecursive(new List<AstNode> { relationalNode.LeftOperand }, commands, commandInfo);

                    if (relationalNode.RightOperand is TextNode rightOperand)
                        commandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(rightOperand, true));
                    else
                        ExtractCommandsRecursive(new List<AstNode> { relationalNode.RightOperand }, commands, commandInfo);

                    commands.Add(commandInfo);
                }
                else if (node is AbsoluteValueNode absNode)
                {
                    CommandInfo commandInfo = new CommandInfo { CommandName = "abs" };
                    commands.Add(commandInfo);
                    ExtractCommandsRecursive(new List<AstNode> { absNode.InnerGroup }, commands, commandInfo);
                }
                else if (node is TextNode textNode && currentCommandInfo != null)
                {
                    currentCommandInfo.TextArguments.AddRange(ExtractTextContentIfArgument(textNode, true));
                }
            }
        }

        private List<string> ExtractTextContentIfArgument(AstNode node, bool lengthCheck)
        {
            var textContents = new List<string>();
            var subTextContents = new List<string>();
            if (node is TextNode textNode)
            {
                textContents.Add(textNode.Text);
            }
            else if (node is CommandNode commandNode)
            {
                subTextContents.AddRange(commandNode.Args.SelectMany(c => ExtractTextContentIfArgument(c, false)));
            }
            else if (node is GroupNode groupNode)
            {
                subTextContents.AddRange(groupNode.Body.SelectMany(c => ExtractTextContentIfArgument(c, false)));
            }
            else if (node is RootNode rootNode)
            {
                subTextContents.AddRange(ExtractTextContentIfArgument(rootNode.Radicand, false));
                subTextContents.AddRange(ExtractTextContentIfArgument(rootNode.Degree, false));
            }
            else if (node is ScriptNode scriptNode)
            {
                subTextContents.AddRange(ExtractTextContentIfArgument(scriptNode.Base, false));
                subTextContents.AddRange(ExtractTextContentIfArgument(scriptNode.Script, false));
            }
            else if (node is RelationalOperatorNode relationalNode)
            {
                subTextContents.AddRange(ExtractTextContentIfArgument(relationalNode.LeftOperand, false));
                subTextContents.AddRange(ExtractTextContentIfArgument(relationalNode.RightOperand, false));
            }
            else if (node is AbsoluteValueNode absNode)
            {
                subTextContents.AddRange(ExtractTextContentIfArgument(absNode.InnerGroup, false));
            }

            return textContents
                .Where(c => lengthCheck ? isValidRegularArgument(c) : isValidBaseArgument(c))
                .Union(subTextContents)
                .ToList();
        }

        private bool isValidBaseArgument(string text)
        {
            return checkValidArgument(text, false);
        }

        private bool isValidRegularArgument(string text)
        {
            return checkValidArgument(text, true);
        }

        private bool checkValidArgument(string text, bool validateLength)
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

            var notAllowedForStart = ParsingRules.NotValidForStartVariable;

            var notAllowedForContain = ParsingRules.NotValidForContainVariableName;

            if (notAllowedForStart.Any(c => text.StartsWith(c)))
                return false;

            if (notAllowedForContain.Any(c => text.Contains(c)))
                return false;

            if (text.Trim().Contains(" "))
                return false;

            if (validateLength && text.Length > 3)
                return false;

            if (ParsingRules.NotValidVariableNames.Contains(text))
                return false;

            return true;
        }

        private bool isInAllowedCommands(ScriptNode scriptNode)
        {
            var baseText = scriptNode.Base.ToString().ToLower().Trim();
            var scriptText = scriptNode.Script.ToString().ToLower().Trim();
            return !(baseText == "cm" && (scriptText == "2" || scriptText == "3"));
        }

        private bool isInAllowedAppendedOperands(string appendedOperand)
        {
            return !ParsingRules.NotValidForAppendedOperandVariableName.Any(c => c == appendedOperand);


            //return
            //        appendedOperand != "m/s" &&
            //        appendedOperand != "m/s2" &&
            //        appendedOperand != "m/s3" &&

            //        appendedOperand != "km/sec" &&
            //        appendedOperand != "km/s" &&
            //        appendedOperand != "km/h" &&

            //        appendedOperand != "kg/sec" &&
            //        appendedOperand != "kg/s" &&
            //        appendedOperand != "kg/h" &&

            //        appendedOperand != "m/sec" &&
            //        appendedOperand != "m/s" &&
            //        appendedOperand != "m/h" &&

            //        appendedOperand != "N/sec" &&
            //        appendedOperand != "N/s" &&
            //        appendedOperand != "N/h" &&
            //        appendedOperand != "N/m" &&

            //        appendedOperand != "rad/sec" &&
            //        appendedOperand != "rad/s" &&
            //        appendedOperand != "rad/h" &&
            //        appendedOperand != "rad/m" &&

            //        appendedOperand != "rev/sec" &&
            //        appendedOperand != "rev/s" &&
            //        appendedOperand != "rev/h" &&

            //        appendedOperand != "W/m" &&
            //        appendedOperand != "W/m2" &&
            //        appendedOperand != "W/m3" &&

            //        appendedOperand != "kW/m" &&
            //        appendedOperand != "kW/m2" &&
            //        appendedOperand != "kW/m3" &&


            //        appendedOperand.ToLower() != "x-ray"
            //        ;
        }
    }
}

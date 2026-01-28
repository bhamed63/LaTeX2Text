using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LatexConverter.Ast;

namespace LatexConverter.Parsing
{
    public class LatexParser
    {
        public List<AstNode> Parse(string input)
        {
            var nodes = ExecuteParsing(input);
            AnalyzeOperators(nodes);
            return ProcessRelationalOperators(nodes);
        }

        private List<AstNode> ProcessRelationalOperators(List<AstNode> nodes)
        {
            var processedNodes = new List<AstNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node is CommandNode cmdNode && ParsingRules.RelationalCommands.Contains(cmdNode.Command))
                {
                    // Find the true left operand by skipping whitespace
                    int leftIndex = processedNodes.Count - 1;
                    while (leftIndex >= 0 && IsWhitespaceNode(processedNodes[leftIndex]))
                    {
                        leftIndex--;
                    }

                    // Find the true right operand by skipping whitespace
                    int rightIndex = i + 1;
                    while (rightIndex < nodes.Count && IsWhitespaceNode(nodes[rightIndex]))
                    {
                        rightIndex++;
                    }

                    if (leftIndex >= 0 && rightIndex < nodes.Count)
                    {
                        var leftOperand = processedNodes[leftIndex];
                        var rightOperand = nodes[rightIndex];

                        // Handle splitting of text nodes
                        if (leftOperand is TextNode leftTextNode)
                        {
                            var text = leftTextNode.Text;
                            var trimmedText = text.TrimEnd();
                            int splitIndex = trimmedText.LastIndexOfAny(new[] { ' ', '\t', '\n', '\r' });

                            if (splitIndex != -1 && splitIndex < trimmedText.Length - 1)
                            {
                                string preceding = text.Substring(0, splitIndex + 1);
                                processedNodes[leftIndex] = new TextNode(preceding);
                                leftOperand = new TextNode(trimmedText.Substring(splitIndex + 1));
                            }
                        }

                        if (rightOperand is TextNode rightTextNode)
                        {
                            var text = rightTextNode.Text;
                            var trimmedText = text.TrimStart();
                            int splitIndex = trimmedText.IndexOfAny(new[] { ' ', '\t', '\n', '\r' });

                            if (splitIndex != -1)
                            {
                                string succeeding = text.Substring(splitIndex);
                                nodes[rightIndex] = new TextNode(succeeding);
                                rightOperand = new TextNode(trimmedText.Substring(0, splitIndex));
                            }
                        }

                        // Get the original sequence of nodes including whitespace
                        var originalSequence = new List<AstNode>();
                        for (int j = leftIndex; j < processedNodes.Count; j++)
                        {
                            originalSequence.Add(processedNodes[j]);
                        }
                        originalSequence.Add(cmdNode);
                        for (int j = i + 1; j <= rightIndex; j++)
                        {
                            originalSequence.Add(nodes[j]);
                        }

                        // Remove the left operand and any trailing whitespace from processedNodes
                        processedNodes.RemoveRange(leftIndex, processedNodes.Count - leftIndex);

                        leftOperand = leftOperand is TextNode ? new TextNode(leftOperand.ToString().Trim()) : leftOperand;
                        rightOperand = rightOperand is TextNode ? new TextNode(rightOperand.ToString().Trim()): rightOperand;
                        var relNode = new RelationalOperatorNode(leftOperand, rightOperand, cmdNode.Command, originalSequence);
                        processedNodes.Add(relNode);

                        // Skip the nodes that are now part of the RelationalOperatorNode
                        i = rightIndex;
                    }
                    else
                    {
                        processedNodes.Add(node);
                    }
                }
                else
                {
                    processedNodes.Add(node);
                }
            }

            return processedNodes;
        }

        private bool IsWhitespaceNode(AstNode node)
        {
            return node is TextNode textNode && string.IsNullOrWhiteSpace(textNode.Text);
        }

        private void AnalyzeOperators(List<AstNode> nodes)
        {
            foreach (var node in nodes)
            {
                AnalyzeNodeRecursively(node);
            }
        }

        private void AnalyzeNodeRecursively(AstNode node)
        {
            switch (node)
            {
                case TextNode textNode:
                    ProcessTextNode(textNode);
                    break;
                case GroupNode groupNode:
                    foreach (var child in groupNode.Body) AnalyzeNodeRecursively(child);
                    break;
                case CommandNode cmdNode:
                    foreach (var arg in cmdNode.Args) AnalyzeNodeRecursively(arg);
                    if (cmdNode.Subscript != null) AnalyzeNodeRecursively(cmdNode.Subscript);
                    if (cmdNode.Superscript != null) AnalyzeNodeRecursively(cmdNode.Superscript);
                    break;
                case ScriptNode sNode:
                    AnalyzeNodeRecursively(sNode.Base);
                    AnalyzeNodeRecursively(sNode.Script);
                    break;
                case RootNode rNode:
                    AnalyzeNodeRecursively(rNode.Radicand);
                    if (rNode.Degree != null) AnalyzeNodeRecursively(rNode.Degree);
                    break;
                case FracNode fNode:
                    AnalyzeNodeRecursively(fNode.Numerator);
                    AnalyzeNodeRecursively(fNode.Denominator);
                    break;
                case BinomNode bNode:
                    AnalyzeNodeRecursively(bNode.Top);
                    AnalyzeNodeRecursively(bNode.Bottom);
                    break;
                case AbsoluteValueNode absNode:
                    AnalyzeNodeRecursively(absNode.InnerGroup);
                    break;
                case PrescriptNode prescriptNode:
                    AnalyzeNodeRecursively(prescriptNode.Script);
                    AnalyzeNodeRecursively(prescriptNode.Base);
                    break;
            }
        }

        private void ProcessTextNode(TextNode textNode)
        {
            var text = textNode.Text;
            var foundOperators = new List<(string Operator, int Index)>();

            for (int j = 0; j < text.Length; j++)
            {
                foreach (var op in ParsingRules.Operators.OrderByDescending(o => o.Length))
                {
                    if (j + op.Length <= text.Length && text.Substring(j, op.Length) == op)
                    {
                        foundOperators.Add((op, j));
                        j += op.Length - 1;
                        break;
                    }
                }
            }

            if (foundOperators.Any())
            {
                var newOperators = new List<OperatorNode>();
                for (int k = 0; k < foundOperators.Count; k++)
                {
                    var (op, index) = foundOperators[k];
                    var leftOperandStartIndex = (k > 0) ? foundOperators[k - 1].Index + foundOperators[k - 1].Operator.Length : 0;
                    var leftOperand = text.Substring(leftOperandStartIndex, index - leftOperandStartIndex);

                    string rightOperand;
                    if (k + 1 < foundOperators.Count)
                    {
                        var nextOpIndex = foundOperators[k + 1].Index;
                        rightOperand = text.Substring(index + op.Length, nextOpIndex - (index + op.Length));
                    }
                    else
                    {
                        rightOperand = text.Substring(index + op.Length);
                    }
                    newOperators.Add(new OperatorNode(op, leftOperand, rightOperand));
                }
                textNode.Operators.AddRange(newOperators);
            }
        }

        private List<AstNode> ExecuteParsing(string input)
        {
            if (input == null)
                return new List<AstNode>();

            var nodes = new List<AstNode>();
            int position = 0;

            while (position < input.Length)
            {
                AstNode node = null;
                if (position + 1 < input.Length &&
                    input[position] == '\\' && (input[position + 1] == '(' || input[position + 1] == '['))
                {
                    node = ParseGroup(input, ref position);
                }
                else if (input[position] == '\\')
                {
                    node = ParseCommand(input, ref position);
                }
                else if (input[position] == '|')
                {
                    int end = FindMatchingBar(input, position);
                    if (end != -1)
                    {
                        string content = input.Substring(position + 1, end - (position + 1));
                        var body = ExecuteParsing(content);
                        node = new AbsoluteValueNode(new GroupNode(body, "", ""));
                        position = end + 1;
                    }
                    else
                    {
                        position++;
                        node = new TextNode("|");
                    }
                }
                else
                {
                    var textNodes = ParseText(input, ref position);
                    if (textNodes.Count > 0)
                    {
                        nodes.AddRange(textNodes);
                    }
                    continue;
                }

                if (node != null)
                {
                    int currentPos = position;
                    while (currentPos < input.Length && char.IsWhiteSpace(input[currentPos]))
                    {
                        currentPos++;
                    }

                    if (currentPos < input.Length && (input[currentPos] == '_' || input[currentPos] == '^'))
                    {
                        position = currentPos;
                        var scriptNode = ParseScript(input, ref position, node);
                        nodes.Add(scriptNode);
                    }
                    else
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        private AstNode ParseSingleNode(string input, ref int position)
        {
            if (position >= input.Length) return null;

            AstNode node = null;
            if (position + 1 < input.Length &&
                input[position] == '\\' && (input[position + 1] == '(' || input[position + 1] == '['))
            {
                node = ParseGroup(input, ref position);
            }
            else if (input[position] == '\\')
            {
                node = ParseCommand(input, ref position);
            }
            else if (input[position] == '|')
            {
                int end = FindMatchingBar(input, position);
                if (end != -1)
                {
                    string content = input.Substring(position + 1, end - (position + 1));
                    var body = ExecuteParsing(content);
                    node = new AbsoluteValueNode(new GroupNode(body, "", ""));
                    position = end + 1;
                }
                else
                {
                    position++;
                    node = new TextNode("|");
                }
            }
            else if (input[position] == '{')
            {
                position++; // Skip '{'
                var argumentContent = ExtractBracedContent(input, ref position);
                if (argumentContent != null)
                {
                    if (position < input.Length && input[position] == '}')
                        position++;
                    var contentNodes = ExecuteParsing(argumentContent);
                    if (contentNodes.Count == 1) node = contentNodes[0];
                    else node = new GroupNode(contentNodes, "", "");
                }
                else
                {
                    node = new TextNode("{");
                }
            }
            else
            {
                node = new TextNode(input[position++].ToString());
            }

            if (node != null)
            {
                int currentPos = position;
                while (currentPos < input.Length && char.IsWhiteSpace(input[currentPos]))
                {
                    currentPos++;
                }

                if (currentPos < input.Length && (input[currentPos] == '_' || input[currentPos] == '^'))
                {
                    position = currentPos;
                    node = ParseScript(input, ref position, node);
                }
            }
            return node;
        }

        private AstNode ParseFollowNode(string input, ref int position)
        {
            if (position >= input.Length) return null;

            if (input[position] == '\\' || input[position] == '{' || input[position] == '(' || input[position] == '[' || input[position] == '|')
            {
                return ParseSingleNode(input, ref position);
            }

            int start = position;
            while (position < input.Length && !ParsingRules.IsScriptDelimiter(input[position]))
            {
                position++;
            }

            if (position > start)
            {
                AstNode node = new TextNode(input.Substring(start, position - start));
                int currentPos = position;
                while (currentPos < input.Length && char.IsWhiteSpace(input[currentPos]))
                {
                    currentPos++;
                }

                if (currentPos < input.Length && (input[currentPos] == '_' || input[currentPos] == '^'))
                {
                    position = currentPos;
                    node = ParseScript(input, ref position, node);
                }
                return node;
            }

            return ParseSingleNode(input, ref position);
        }

        private AstNode ParseScript(string input, ref int position, AstNode baseNode)
        {
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
            position = currentPos;
            return finalNode;
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
                else if (input[position] == '|')
                {
                    break;
                }
                else if (input[position] == '_' || input[position] == '^')
                {
                    string textBefore = input.Substring(start, position - start);
                    AstNode baseOfScript;

                    if (string.IsNullOrWhiteSpace(textBefore))
                    {
                        if (!string.IsNullOrEmpty(textBefore))
                        {
                            nodes.Add(new TextNode(textBefore));
                        }
                        baseOfScript = new TextNode("");
                    }
                    else
                    {
                        // If there's whitespace before the script, try to treat it as a prescript first
                        if (position > start && char.IsWhiteSpace(input[position - 1]))
                        {
                            int checkpoint = position;
                            var tryScriptNode = ParseScript(input, ref position, new TextNode(""));
                            if (tryScriptNode is ScriptNode trySn && position < input.Length)
                            {
                                int followCheckpoint = position;
                                while (position < input.Length && char.IsWhiteSpace(input[position])) position++;

                                if (position < input.Length && input[position] != '^' && input[position] != '_')
                                {
                                    var followNode = ParseFollowNode(input, ref position);
                                    if (followNode != null)
                                    {
                                        if (!string.IsNullOrEmpty(textBefore))
                                        {
                                            nodes.Add(new TextNode(textBefore));
                                        }
                                        nodes.Add(new PrescriptNode(trySn, followNode));
                                        return nodes;
                                    }
                                }
                                position = followCheckpoint;
                            }
                            // Fallback if no follow-node found
                            position = checkpoint;
                        }

                        int baseEnd = position;
                        while (baseEnd > start && char.IsWhiteSpace(input[baseEnd - 1]))
                        {
                            baseEnd--;
                        }

                        int baseStart = baseEnd;
                        while (baseStart > start)
                        {
                            char prevChar = input[baseStart - 1];
                            //if (char.IsWhiteSpace(prevChar) || prevChar == '/')
                            if (ParsingRules.IsScriptDelimiter(prevChar))
                            {
                                break;
                            }
                            baseStart--;
                        }

                        string precedingText = input.Substring(start, baseStart - start);
                        string baseText = input.Substring(baseStart, baseEnd - baseStart);

                        if (!string.IsNullOrEmpty(precedingText))
                        {
                            nodes.Add(new TextNode(precedingText));
                        }
                        baseOfScript = new TextNode(baseText);
                    }

                    var scriptNode = ParseScript(input, ref position, baseOfScript);

                    // If it's a single script at start/after delimiter, bind with following base
                    if (scriptNode is ScriptNode sn && sn.Base is TextNode tn && tn.Text == "" && position < input.Length)
                    {
                        int checkpoint = position;
                        while (position < input.Length && char.IsWhiteSpace(input[position])) position++;

                        if (position < input.Length && input[position] != '^' && input[position] != '_')
                        {
                            var followNode = ParseFollowNode(input, ref position);
                            if (followNode != null)
                            {
                                nodes.Add(new PrescriptNode(sn, followNode));
                                return nodes;
                            }
                        }
                        position = checkpoint;
                    }

                    nodes.Add(scriptNode);
                    return nodes;
                }
                else
                {
                    position++;
                }
            }

            //if (position > start)
            //{
            //    nodes.Add(new TextNode(input.Substring(start, position - start)));
            //}

            if (position > start && (start + position - start) <= input.Length)
            {
                nodes.Add(new TextNode(input.Substring(start, position - start)));
            }

            return nodes;
        }

        private int FindMatchingBar(string input, int startPos)
        {
            int braceLevel = 0;
            int depth = 1;
            for (int i = startPos + 1; i < input.Length; i++)
            {
                if (input[i] == '\\' && i + 1 < input.Length)
                {
                    if (input[i + 1] == '|') { i++; continue; }
                }
                if (input[i] == '{') braceLevel++;
                else if (input[i] == '}') braceLevel--;
                else if (input[i] == '|' && braceLevel == 0)
                {
                    if (IsClosingBar(input, i))
                    {
                        depth--;
                        if (depth == 0) return i;
                    }
                    else
                    {
                        depth++;
                    }
                }
            }
            return -1;
        }

        private bool IsClosingBar(string input, int pos)
        {
            if (pos == 0) return false;
            char prev = '\0';
            for (int i = pos - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    prev = input[i];
                    break;
                }
            }

            if (prev == '\0') return false;
            if (char.IsLetterOrDigit(prev) || prev == '}' || prev == ')' || prev == ']' || prev == '|' || prev == '.' || prev == '!')
                return true;

            return false;
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
                    var contentNodes = ExecuteParsing(content);
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
                if (ParsingRules.IsScriptDelimiter(c))
                {
                    break;
                }
                if (ParsingRules.IsLastOrNextIsDelimiter(input, position) && ParsingRules.IsNotAllowedAtLastChar(c))
                    break;
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
                        var children = ExecuteParsing(groupContent);
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
                return new GroupNode(ExecuteParsing(remainingContent), openGroup, closeGroup);
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
                            arguments.AddRange(ExecuteParsing(argumentContent));
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
                var nodes = ExecuteParsing(content);
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
                return ExecuteParsing(content.ToString());
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
                    arguments.AddRange(ExecuteParsing(argumentContent));
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
}

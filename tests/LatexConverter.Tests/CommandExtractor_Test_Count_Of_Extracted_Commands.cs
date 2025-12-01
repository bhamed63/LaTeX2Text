using LatexConverter.Parsing;
using Xunit;

namespace LatexConvertorTests
{
    public class CommandExtractor_Test_Count_Of_Extracted_Commands
    {
        LatexParser _latexParser;
        LatexCommandExtractor _commandExtractor;

        public CommandExtractor_Test_Count_Of_Extracted_Commands()
        {
            _latexParser = new LatexParser();
            _commandExtractor = new LatexCommandExtractor();
        }

        [Fact]
        public void Test_Parser_Check_The_Count_Of_Nodes_Created_On_First_Level()
        {
            checkCommandsAndArgumentsCount("\\frac{x}{y}, it is simple Latex.", 1, 2);
            checkCommandsAndArgumentsCount("here it is a \\sqrt{x}, it is simple Latex.", 1, 1);
            checkCommandsAndArgumentsCount("here it is a \\sqrt{x}", 1, 1);
            checkCommandsAndArgumentsCount("here it is a \\sqrt{\\frac{x}{y}}, it is simple Latex.", 2, 2);
            checkCommandsAndArgumentsCount("here it is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.", 3, 1);
            checkCommandsAndArgumentsCount("here \\(it \\)is a \\sqrt{\\frac{x}{\\alpha}}, it is simple Latex.", 3, 1);
        }

        private void checkCommandsAndArgumentsCount(string text, int commandsCount, int argumentCount)
        {
            var nodes = _latexParser.Parse(text);
            var commands = _commandExtractor.ExtractCommands(nodes);
            if (commands.Count != commandsCount)
                Assert.Fail("Extracted commands count is invalid: " + text);
            if (commands.Sum(c => c.ArgumentCount) != argumentCount)
                Assert.Fail("Extracted arguments count is invalid: " + text);
        }

        //[Fact]
        //public void Test_CommandExtractor_Comprehensive_Scenarios()
        //{
        //    // Generic command with one argument
        //    checkCommandsAndArgumentsCount(@"\sin{x}", 1, 1);

        //    // Generic command with multiple arguments (note: only "arg1" and "arg2" are valid args)
        //    checkCommandsAndArgumentsCount(@"\command{arg1}{arg2}{123}", 1, 2);

        //    // Nested generic commands
        //    checkCommandsAndArgumentsCount(@"\textbf{\textit{i}}", 2, 2);

        //    // Command with no arguments
        //    checkCommandsAndArgumentsCount(@"\alpha", 1, 0);

        //    // Mixed content with multiple commands
        //    checkCommandsAndArgumentsCount(@"Some text \command{arg} and \another", 2, 1);

        //    // Special command containing a generic command
        //    checkCommandsAndArgumentsCount(@"\sqrt{\textbf{x}}", 2, 1);

        //    // Generic command with a special command as an argument
        //    checkCommandsAndArgumentsCount(@"\textit{\frac{a}{b}}", 2, 2);

        //    // Command with an argument that is invalid and should be ignored
        //    checkCommandsAndArgumentsCount(@"\item{1. First point}", 1, 0);

        //    // Command with an argument containing spaces, which is invalid
        //    checkCommandsAndArgumentsCount(@"\textbf{hello world}", 1, 0);

        //    // Malformed frac - extractor should still find the command, but no valid arguments
        //    checkCommandsAndArgumentsCount(@"\frac{a}", 1, 1);

        //    // Unclosed command argument - should find command and valid part of argument
        //    // checkCommandsAndArgumentsCount(@"\command{unclosed", 1, 1);

        //    // Deeply nested structure
        //    checkCommandsAndArgumentsCount(@"\textbf{\sqrt{\frac{a}{b}}}", 3, 3);
        //}
    }
}

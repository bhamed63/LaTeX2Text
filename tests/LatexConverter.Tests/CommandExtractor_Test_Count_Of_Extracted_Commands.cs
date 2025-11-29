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
    }
}

using LatexConverter.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LatexConverter.Tests
{
    public class CommandExtractor_Test_Not_Allowed_Variables_And_Commands
    {
        LatexParser _latexParser;
        LatexCommandExtractor _commandExtractor;

        public CommandExtractor_Test_Not_Allowed_Variables_And_Commands()
        {
            _latexParser = new LatexParser();
            _commandExtractor = new LatexCommandExtractor();
        }

        [Fact]
        public void Test_CommandExtractor_Test_CM_Should_Not_Appear_In_Commands()
        {
            string text = "of density 0.81 g/cm^3";
            var nodes = _latexParser.Parse(text);
            var commands = _commandExtractor.ExtractCommands(nodes);

            Assert.DoesNotContain("cm", commands.Select(c => c.CommandName));
        }

        [Fact]
        public void Test_CommandExtractor_Test_ms_Should_Not_Appear_In_Commands()
        {
            string text = "and a speed of (4.99 m/s) when it is at it initial position";
            var nodes = _latexParser.Parse(text);

            Assert.Single(nodes);
            Assert.IsType<TextNode>(nodes[0]);
            Assert.Single((nodes[0] as TextNode).Operators);


            var commands = _commandExtractor.ExtractArgumentsFromOperators(nodes);

            Assert.DoesNotContain("m", commands.SelectMany(c => c.TextArguments));
            Assert.DoesNotContain("s", commands.SelectMany(c => c.TextArguments));
        }
    }
}

using System.Collections.Generic;

namespace LatexConverter.Parsing
{
    public class EnrichedCommandInfo
    {
        public string CommandName { get; set; }
        public List<string> TextArguments { get; set; } = new List<string>();
        public int ArgumentCount => TextArguments.Count;

        public string PlainText { get; set; }
        public string ScreenReader { get; set; }
        public string HumanFriendly { get; set; }
        public string OpenAI { get; set; }
        public string ExceptionalScreenReader { get; set; }
        public string HumanFriendlyKey { get; set; }
        public CommandType CommandType { get; set; }
        public int ArgsNumber { get; set; }
    }
}

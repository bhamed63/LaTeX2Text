using System.Collections.Generic;

namespace LatexConverter
{
    public class ExtractedComponents
    {
        public List<string> Variables { get; } = new List<string>();
        public List<string> FunctionsAndCommands { get; } = new List<string>();
    }
}

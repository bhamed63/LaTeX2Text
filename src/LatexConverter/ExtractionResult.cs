using System;
using System.Collections.Generic;

namespace LatexConverter
{
    public class ExtractionResult
    {
        public List<Tuple<CommandType, string>> Commands { get; set; } = new List<Tuple<CommandType, string>>();
        public List<string> Operators { get; set; } = new List<string>();
        public List<string> Variables { get; set; } = new List<string>();
        public List<string> Numbers { get; set; } = new List<string>();
    }
}

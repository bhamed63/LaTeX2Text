using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Parsing
{
    public static class ParsingRules
    {
        public static readonly List<string> Operators = new List<string> { "/", "+", "*", "-", "=", ">", "<", ">=", "<=", "≤", "≥", "±", "∓" };
        public static readonly List<string> RelationalCommands = new List<string> { "ge", "le", "ne", "approx", "equiv", "ll", "gg", "pm", "mp", "in", "ni", "subset", "supset", "subseteq", "supseteq", "parallel", "perp", "vdash", "models", "mid", "sim", "simeq", "cong", "asymp", "propto", "bowtie", ">", "<" };
        public static readonly char[] SubscripDelimitor = { '\\', '}', '_', '^', '/', ' ', '`', '.', ')', '(', '+', '-', '*', '/', '=' };
        public static readonly char[] NotAllowedAtLastChar = { ',' };
        public static readonly List<string> NotValidVariableNames = new List<string> { "cm", "cm2", "cm3", "dB", "km", "kg" };//, "m/s", "m/s2", "m/s3" };

        public static bool IsNotAllowedAtLastChar(char c)
        {
            return NotAllowedAtLastChar.Contains(c);
        }

        public static bool IsLastOrNextIsDelimiter(string input, int position)
        {
            return IsLast(input, position) || IsScriptDelimiter(input[position + 1]);
        }

        public static bool IsLast(string input, int position)
        {
            return position == input.Length - 1;
        }

        public static bool IsScriptDelimiter(char input)
        {
            return char.IsWhiteSpace(input) || SubscripDelimitor.Contains(input);
        }
    }
}

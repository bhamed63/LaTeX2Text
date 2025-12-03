using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Parsing
{
    public static class ParsingRules
    {
        public static readonly List<string> Operators = new List<string> { "/", "+", "*", "-", "=", ">", "<", ">=", "<=", "≤", "≥", "±", "∓" };
        public static readonly char[] SubscripDelimitor = { '\\', '}', '_', '^', '/', ' ' };
        public static readonly char[] NotAllowedAtLastChar = { ',' };

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

using System.Collections.Generic;
using System.Linq;

namespace LatexConverter.Parsing
{
    public static class ParsingRules
    {
        public static readonly List<string> Operators = new List<string> { "/", "+", "*", "-", "=", ">", "<", ">=", "<=", "≤", "≥", "±", "∓" };
        public static readonly List<string> RelationalCommands = new List<string> { "ge", "le", "ne", "approx", "equiv", "ll", "gg", "pm", "mp", "in", "ni", "subset", "supset", "subseteq", "supseteq", "parallel", "perp", "vdash", "models", "mid", "sim", "simeq", "cong", "asymp", "propto", "bowtie", ">", "<" };
        public static readonly char[] SubscripDelimitor = { '\\', '}', '_', '^', '/', ' ', '`', '.', ')', '(', '+', '-', '*', '/', '=', ';', ':', '\r', '\n', '\'', '~', '?' };
        public static readonly char[] NotAllowedAtLastChar = { ',' };
        public static readonly List<string> NotValidVariableNames = new List<string> { "cm", "cm2", "cm3", "dB", "km", "kg", "sec", "mg",
            "min", "max" ,
            "how", "non", "but",
            "if",
            "kW",
            "MHz",
            "Hz",
            "is",
            "one",
            "on"
            };//, "m/s", "m/s2", "m/s3" };
        public static readonly List<string> NotValidForStartVariable = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "_"  };
        public static readonly List<string> NotValidForContainVariableName = new List<string>()
            {
                "{", "}", ")", "(",
                ",", ";", "'", "\"",
                "/", "\\", "=", "+",
                "-", "*", ".", "\"", "“", ">", "<",
                "–",
                "~"
            };

        public static readonly List<string> NotValidForAppendedOperandVariableName = new List<string>() {     "m/s" ,
                     "m/s2" ,
                     "m/s3" ,

                     "km/sec" ,
                     "km/s" ,
                     "km/h" ,

                     "kg/sec" ,
                     "kg/s" ,
                     "kg/h" ,

                     "m/sec" ,
                     "m/s" ,
                     "m/h" ,

                     "N/sec" ,
                     "N/s" ,
                     "N/h" ,
                     "N/m" ,

                     "rad/sec" ,
                     "rad/s" ,
                     "rad/h" ,
                     "rad/m" ,

                     "rev/sec" ,
                     "rev/s" ,
                     "rev/h" ,

                     "W/m" ,
                     "W/m2" ,
                     "W/m3" ,

                     "kW/m" ,
                     "kW/m2" ,
                     "kW/m3" ,

                     "mSv/y",
                     "g/mol",

                     "x-ray"};

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

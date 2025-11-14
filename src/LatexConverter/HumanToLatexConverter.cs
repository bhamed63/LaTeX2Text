using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    public class HumanToLatexConverter
    {
        public string Convert(string humanFriendlyText)
        {
            if (string.IsNullOrEmpty(humanFriendlyText)) return "";

            humanFriendlyText = PreProcessHumanLatexPatterns(humanFriendlyText);

            var sb = new StringBuilder();
            for (int i = 0; i < humanFriendlyText.Length; i++)
            {
                char c = humanFriendlyText[i];

                if (TryHandleIntegral(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleCombiningCharacters(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSuperscript(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSubscript(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleSpecialSymbols(c, sb, humanFriendlyText, ref i)) continue;
                if (TryHandleGeneralSymbol(c, sb, humanFriendlyText, i)) continue;
                if (TryHandleFontCharacter(c, sb, humanFriendlyText, i)) continue;

                sb.Append(c);
            }
            return PostProcessHumanLatex(sb.ToString());
        }

        private string PreProcessHumanLatexPatterns(string text)
        {
            text = text.Replace("sin⁻¹", CommandNames.Arcsin + " ");
            text = text.Replace("cos⁻¹", CommandNames.Arccos + " ");
            text = text.Replace("tan⁻¹", CommandNames.Arctan + " ");
            text = Regex.Replace(text, @"√\((.*?)\)", CommandNames.Sqrt + "{$1}");
            text = Regex.Replace(text, @"√([^ ])", CommandNames.Sqrt + "{$1}");
            text = Regex.Replace(text, @"\(([^ ]*?) ([^ ]*?)\)\n\(([^ ]*?) ([^ ]*?)\)", $@"{CommandNames.PmatrixBegin} $1 {CommandNames.MatrixColumnSeparator} $2 {CommandNames.MatrixRowSeparator} $3 {CommandNames.MatrixColumnSeparator} $4 {CommandNames.PmatrixEnd}");
            text = Regex.Replace(text, @"\(([^ ]*?) ([^ ]*?)\)", CommandNames.Binom + "{$1}{$2}");
            text = text.Replace("p̅", CommandNames.Overline + "{p}");
            text = text.Replace("xyz̅", CommandNames.Overline + "{xyz}");
            text = Regex.Replace(text, @"lim_{([^}]*)}", CommandNames.Lim + "_{$1}");
            return text;
        }

        private bool TryHandleIntegral(char c, StringBuilder sb, string text, ref int i)
        {
            if (c != '∫') return false;

            sb.Append(CommandNames.Int);
            i++;

            var sub_content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSubMap.ContainsKey(text[i]))
            {
                sub_content.Append(Dictionaries.ReverseSubMap[text[i]]);
                i++;
            }
            if (sub_content.Length > 0)
            {
                if (sub_content.Length > 1) sb.Append($"{CommandNames.SubscriptChar}{{{sub_content}}}"); else sb.Append($"{CommandNames.SubscriptChar}{sub_content}");
            }

            var sup_content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSupMap.ContainsKey(text[i]))
            {
                sup_content.Append(Dictionaries.ReverseSupMap[text[i]]);
                i++;
            }
            if (sup_content.Length > 0)
            {
                if (sup_content.Length > 1) sb.Append($"{CommandNames.SuperscriptChar}{{{sup_content}}}"); else sb.Append($"{CommandNames.SuperscriptChar}{sup_content}");
            }
            i--;
            return true;
        }

        private bool TryHandleCombiningCharacters(char c, StringBuilder sb, string text, ref int i)
        {
            if (i + 1 >= text.Length) return false;

            char next_c = text[i + 1];
            string? command = null;

            if (next_c == '\u20D7') command = CommandNames.Vec; // vec
            if (next_c == '\u0302') command = CommandNames.Hat; // hat

            if (command != null)
            {
                sb.Append($"{command}{{{c}}}");
                i++; // consume combining char
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private bool TryHandleSuperscript(char c, StringBuilder sb, string text, ref int i)
        {
            if (!Dictionaries.ReverseSupMap.ContainsKey(c)) return false;

            var content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSupMap.ContainsKey(text[i]))
            {
                content.Append(Dictionaries.ReverseSupMap[text[i]]);
                i++;
            }
            i--;
            if (content.Length > 1) sb.Append($"{CommandNames.SuperscriptChar}{{{content}}}"); else sb.Append($"{CommandNames.SuperscriptChar}{content}");
            return true;
        }

        private bool TryHandleSubscript(char c, StringBuilder sb, string text, ref int i)
        {
            if (!Dictionaries.ReverseSubMap.ContainsKey(c)) return false;

            var content = new StringBuilder();
            while (i < text.Length && Dictionaries.ReverseSubMap.ContainsKey(text[i]))
            {
                var base_char = Dictionaries.ReverseSubMap[text[i]];
                if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(base_char.ToString()))
                {
                    content.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[base_char.ToString()]}");
                }
                else { content.Append(base_char); }
                i++;
            }
            i--;
            if (content.Length > 1 && !content.ToString().Contains("\\")) sb.Append($"{CommandNames.SubscriptChar}{{{content}}}"); else sb.Append($"{CommandNames.SubscriptChar}{content}");
            return true;
        }

        private bool TryHandleSpecialSymbols(char c, StringBuilder sb, string text, ref int i)
        {
            string? command = c switch
            {
                '⇔' => CommandNames.BigLeftrightarrow,
                '⇒' => CommandNames.BigRightarrow,
                '°' => CommandNames.Circ,
                _ => null
            };

            if (command != null)
            {
                sb.Append(command).Append(" ");
                return true;
            }
            return false;
        }

        private bool TryHandleGeneralSymbol(char c, StringBuilder sb, string text, int i)
        {
            if (Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(c.ToString()) && c != ' ')
            {
                sb.Append($"\\{Dictionaries.ReverseHumanFriendlySymbolMap[c.ToString()]}");
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private bool TryHandleFontCharacter(char c, StringBuilder sb, string text, int i)
        {
            if (Dictionaries.ReverseMathFontMap.ContainsKey(c.ToString()))
            {
                sb.Append($"{Dictionaries.ReverseMathFontMap[c.ToString()]}");
                AppendSpaceIfNeeded(sb, text, i);
                return true;
            }
            return false;
        }

        private void AppendSpaceIfNeeded(StringBuilder sb, string text, int i)
        {
            if (i + 1 < text.Length)
            {
                char next_c = text[i + 1];
                if (!Dictionaries.ReverseSubMap.ContainsKey(next_c) && !Dictionaries.ReverseSupMap.ContainsKey(next_c))
                {
                    sb.Append(" ");
                }
            }
            else
            {
                sb.Append(" ");
            }
        }

        private string PostProcessHumanLatex(string text)
        {
            return Regex.Replace(text, @" ([,.])", "$1");
        }
    }
}

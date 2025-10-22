using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    public class HtmlConvertor
    {
        public string ConvertHTMLToOpenAIFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return " ";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", "\n");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, @"\s*&deg;\s*", " degrees");
            text = Regex.Replace(text, @"\s*&times;\s*", " times ");
            text = Regex.Replace(text, "&sdot;", " sdot ");
            text = Regex.Replace(text, "<sub>(.*?)</sub>", m =>
            {
                string content = m.Groups[1].Value;
                if (content.Length == 1) return $"_{content}";
                return $"_({content})";
            });
            text = Regex.Replace(text, "<sup>(.*?)</sup>", "^($1)");
            text = Regex.Replace(text, "&([a-zA-Z]+?);", "$1");
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"\s+", " ").Trim();
            }
            return string.Join("\n", lines);
        }

        public string ConvertHTMLToHumanFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return "";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", "\n");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, @"\s*&deg;\s*", "°");
            text = Regex.Replace(text, @"&([a-zA-Z]+);", m =>
            {
                string key = m.Value == "&times;" ? @"\times" : (m.Value == "&sdot;" ? @"\cdot" : $"\\{m.Groups[1].Value}");
                return Dictionaries.HumanFriendlySymbolMap.GetValueOrDefault(key, m.Groups[1].Value);
            });
            text = Regex.Replace(text, "<sup>(.*?)</sup>", m =>
            {
                var s = m.Groups[1].Value;
                var map = Dictionaries.SupMap;
                var sb = new StringBuilder();
                foreach (char c in s)
                {
                    if (map.TryGetValue(c, out char newChar)) sb.Append(newChar);
                    else sb.Append(c);
                }
                return sb.ToString();
            });
            text = Regex.Replace(text, "<sub>(.*?)</sub>", m =>
            {
                var s = m.Groups[1].Value;
                var map = Dictionaries.SubMap;
                var sb = new StringBuilder();
                foreach (char c in s)
                {
                    if (map.TryGetValue(c, out char newChar)) sb.Append(newChar);
                    else sb.Append(c);
                }
                return sb.ToString();
            });
            text = Regex.Replace(text, @"\s*([×])\s*", "$1");
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"\s+", " ").Trim();
            }
            return string.Join("\n", lines);
        }

        public string ConvertHTMLToScreenReaderFriendlyText(string html_input)
        {
            if (string.IsNullOrEmpty(html_input)) return "";
            if (string.IsNullOrWhiteSpace(html_input)) return " ";
            var text = html_input;
            text = Regex.Replace(text, @"<hr\s*/?>", "\n");
            text = text.Replace("&nbsp;", " ");
            text = Regex.Replace(text, @"<br\s*/?>", "\n");
            string previous;
            do
            {
                previous = text;
                text = Regex.Replace(text, @"<(i|b|strong|span|u|center)>(.*?)</\1>", "$2");
            } while (text != previous);
            text = Regex.Replace(text, @"\s*&deg;\s*", " degrees ");
            text = Regex.Replace(text, @"\s*&times;\s*", " times ");
            text = Regex.Replace(text, "&sdot;", " dot ");
            text = Regex.Replace(text, "<sub>(.*?)</sub>", " subscript $1 ");
            text = Regex.Replace(text, "<sup>(.*?)</sup>", m =>
            {
                string content = m.Groups[1].Value;
                if (content == "2") return " squared ";
                if (content == "3") return " cubed ";
                return $" to the power of {content} ";
            });
            text = Regex.Replace(text, "&([a-zA-Z]+?);", " $1 ");
            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Regex.Replace(lines[i], @"\s+", " ").Trim();
            }
            return string.Join("\n", lines);
        }
    }
}

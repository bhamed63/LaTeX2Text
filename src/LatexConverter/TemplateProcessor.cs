using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    public class TemplateProcessor
    {
        public string ProcessTemplateCommand(CommandNode node, IVisitor<string> visitor,
            IReadOnlyDictionary<string, string> templateMap, IReadOnlyDictionary<string, string> defaultMap = null)
        {
            var args = node.Args.Select(arg => arg.Accept(visitor)).ToArray();
            return ProcessTemplateCommand(node.Command, args, visitor, templateMap, defaultMap);
        }

        public string ProcessTemplateCommand(string command, string[] args, IVisitor<string> visitor,
            IReadOnlyDictionary<string, string> templateMap, IReadOnlyDictionary<string, string> defaultMap = null)
        {
            if (templateMap.TryGetValue(command, out var template))
            {
                args = removeUnnecessaryParanthesses(template, args);
                if (args.Length > 0)
                    return string.Format(template, args);
                return removeUnnecessaryParameters(template);
            } 
            return defaultMap != null ? defaultMap.GetValueOrDefault(command, command) : command;
        }

        public string ProcessTemplateCommandSubscript(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                if (node.Subscript != null)
                {
                    var subscriptText = node.Subscript.Accept(visitor);
                    subscriptText = Regex.Replace(subscriptText, @"\s+", "");
                    return string.Format(template, subscriptText);
                }
            }
            return node.Command;
        }

        public string ProcessTemplateCommandSubscript(LimNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                if (node.Subscript != null)
                {
                    var subscriptText = node.Subscript.Accept(visitor);
                    subscriptText = Regex.Replace(subscriptText, @"\s+", "");
                    return string.Format(template, subscriptText);
                }
            }
            return node.Command;
        }

        public string ToUnicodeProcessTemplateCommandSubscriptSuperscript(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap, Dictionary<char, string> unicodeMap = null)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var subScript = "";
                var superScript = "";
                if (node.Subscript != null)
                    subScript = ToUnicode(node.Subscript.Accept(visitor), false, node.Subscript);
                if (node.Superscript != null)
                    superScript = ToUnicode(node.Superscript.Accept(visitor), true, node.Superscript);
                return string.Format(template, subScript, superScript);
            }
            return node.Command;
        }

        private string[] removeUnnecessaryParanthesses(string template, string[] args)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                var arg = args[i];

                if (template.Contains("({" + i + "})"))
                {
                    if (arg.StartsWith("(") && arg.EndsWith(")"))
                        arg = arg.Substring(1, arg.Length - 2);
                }
                args[i] = arg;
            }
            return args;
        }

        private string removeUnnecessaryParameters(string template)
        {
            return Regex.Replace(template, @"\{\d+\}", "");
        }

        public string ToUnicodeProcessTemplateCommand(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap, Dictionary<char, string> unicodeMap = null)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => ToUnicode(arg.Accept(visitor), true, arg, unicodeMap)).ToArray();
                args = removeUnnecessaryParanthesses(template, args);
                return string.Format(template, args);
            }
            return node.Command;
        }

        public string ToUnicode(string s, bool? isSuperscript, AstNode originalNode, Dictionary<char, string> map = null)
        {
            var tempMap = isSuperscript.HasValue && isSuperscript.Value ? Dictionaries.SupMap : Dictionaries.SubMap;
            if (isSuperscript.HasValue && map == null) map = convertToMap(tempMap);

            string stripped_s = Regex.Replace(s, @"[\(\)]", "");
            if (map != null && !string.IsNullOrEmpty(stripped_s) && stripped_s.All(c => map.ContainsKey(c)))
            {
                var sb = new StringBuilder();
                foreach (char c in stripped_s) sb.Append(map[c]);
                return sb.ToString();
            }

            bool needsParentheses = !(originalNode is TextNode textNode && Regex.IsMatch(textNode.Text, @"^[a-zA-Z0-9]+$"));
            if (isSuperscript.HasValue)
            {
                string op = isSuperscript.Value ? "^" : "_";
                if (needsParentheses && !(s.StartsWith("(") || s.EndsWith(")")))
                {
                    return $"{op}({s})";
                }
                return $"{op}{s}";
            }
            return s;
        }

        private Dictionary<char, string>? convertToMap(Dictionary<char, char> tempMap)
        {
            if (tempMap == null)
                return null;

            var convertedMap = new Dictionary<char, string>();
            foreach (var key in tempMap.Keys)
            {
                convertedMap.Add(key, tempMap[key].ToString());
            }
            return convertedMap;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    /// <summary>
    /// An abstract base class for visitors.
    /// </summary>
    public abstract class BaseVisitor<T> : IVisitor<T>
    {
        public abstract T VisitText(TextNode node);
        public abstract T ExceptionalVisitText(TextNode node);
        public abstract T VisitCommand(CommandNode node);
        public abstract T ExceptionalVisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T ExceptionalVisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T ExceptionalVisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
        public abstract T ExceptionalVisitMatrix(MatrixNode node);

        internal static string ProcessTemplateCommand(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => arg.Accept(visitor)).ToArray();
                args = removeUnnecessaryParanthesses(template, args);
                return string.Format(template, args);
            }
            return node.Command;
        }

        private static string[] removeUnnecessaryParanthesses(string template, string[] args)
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

        internal static string ToUnicodeProcessTemplateCommand(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
        {
            var x = $"e{ToUnicode(node.Args[0].Accept(visitor), true, node.Args[0])}";
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => ToUnicode(arg.Accept(visitor), true, arg)).ToArray();
                args = removeUnnecessaryParanthesses(template, args);
                return string.Format(template, args);
            }
            return node.Command;
        }

        protected static string ToUnicode(string s, bool? isSuperscript, AstNode originalNode, Dictionary<char, string> map = null)
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

        private static Dictionary<char, string>? convertToMap(Dictionary<char, char> tempMap)
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

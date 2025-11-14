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
        public abstract T VisitCommand(CommandNode node);
        public abstract T VisitGroup(GroupNode node);
        public abstract T VisitScript(ScriptNode node);
        public abstract T VisitMatrix(MatrixNode node);
        public abstract T VisitSqrt(SqrtNode node);
        public abstract T VisitFrac(FracNode node);
        public abstract T VisitBinom(BinomNode node);

        public virtual T ExceptionalVisitText(TextNode node)
        {
            return VisitText(node);
        }
        public virtual T ExceptionalVisitCommand(CommandNode node)
        {
            return VisitCommand(node);
        }

        public virtual T ExceptionalVisitGroup(GroupNode node)
        {
            return VisitGroup(node);
        }

        public virtual T ExceptionalVisitScript(ScriptNode node)
        {
            return VisitScript(node);
        }

        public virtual T ExceptionalVisitMatrix(MatrixNode node)
        {
            return VisitMatrix(node);
        }

        public virtual T ExceptionalVisitSqrt(SqrtNode node)
        {
            return VisitSqrt(node);
        }

        public virtual T ExceptionalVisitFrac(FracNode node)
        {
            return VisitFrac(node);
        }

        public virtual T ExceptionalVisitBinom(BinomNode node)
        {
            return VisitBinom(node);
        }

        internal static string ProcessTemplateCommand(CommandNode node, IVisitor<string> visitor,
            IReadOnlyDictionary<string, string> templateMap, IReadOnlyDictionary<string, string> defaultMap = null)
        {
            var args = node.Args.Select(arg => arg.Accept(visitor)).ToArray();
            return ProcessTemplateCommand(node.Command, args, visitor, templateMap, defaultMap); 
        }

        internal static string ProcessTemplateCommand(string command, string[] args, IVisitor<string> visitor,
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

        internal static string ProcessTemplateCommandSubscript(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap)
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

        internal static string ToUnicodeProcessTemplateCommandSubscriptSuperscript(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap, Dictionary<char, string> unicodeMap = null)
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

        private static string removeUnnecessaryParameters(string template)
        {
            return Regex.Replace(template, @"\{\d+\}", "");
        }

        internal static string ToUnicodeProcessTemplateCommand(CommandNode node, IVisitor<string> visitor, IReadOnlyDictionary<string, string> templateMap, Dictionary<char, string> unicodeMap = null)
        {
            if (templateMap.TryGetValue(node.Command, out var template))
            {
                var args = node.Args.Select(arg => ToUnicode(arg.Accept(visitor), true, arg, unicodeMap)).ToArray();
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

        public virtual string GetPreProcessedResult(string text)
        {
            return text;
        }
    }
}

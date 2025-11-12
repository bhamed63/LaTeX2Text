using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexConverter
{
    public class ParenthesesCleaner
    {
        public static string RemoveExtraParentheses(string input)
        {
            var sb = new StringBuilder(input);
            bool changed;

            do
            {
                changed = false;

                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == '(')
                    {
                        int start = i;
                        int count = 1;

                        // Find matching closing parenthesis
                        int j = i + 1;
                        for (; j < sb.Length; j++)
                        {
                            if (sb[j] == '(') count++;
                            else if (sb[j] == ')') count--;

                            if (count == 0) break;
                        }

                        if (count != 0) break; // unmatched, stop

                        string inside = sb.ToString(start + 1, j - start - 1).Trim();

                        // Only remove parentheses if:
                        // 1. They wrap a single expression (no operators outside inner parentheses)
                        // 2. Not empty '()'
                        // 3. Not a lambda '=>'
                        if (!string.IsNullOrEmpty(inside) &&
                            !inside.Contains("=>") &&
                            !(inside.Contains("(") && inside.Contains(")") && !IsSingleParenthesesWrap(inside)))
                        {
                            // Check if inside is itself wrapped by parentheses: ((x+y)) -> (x+y)
                            if (inside.StartsWith("(") && inside.EndsWith(")") && IsSingleParenthesesWrap(inside))
                            {
                                sb.Remove(j, 1);
                                sb.Remove(start, 1);
                                changed = true;
                                break;
                            }
                        }
                    }
                }
            } while (changed);

            return sb.ToString();
        }

        static bool IsSingleParenthesesWrap(string s)
        {
            // Checks if the string is a single expression wrapped by parentheses
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(') count++;
                else if (s[i] == ')') count--;
                if (count < 0) return false;
            }
            return count == 0;
        }

    }
}

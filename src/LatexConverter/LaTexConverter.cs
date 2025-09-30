using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    public class LaTexConverter
    {
        #region Dictionaries

        private static readonly Dictionary<string, string> SymbolMap = new Dictionary<string, string>
        {
            // Greek Letters
            { @"\alpha", "alpha" }, { @"\beta", "beta" }, { @"\gamma", "gamma" }, { @"\delta", "delta" },
            { @"\epsilon", "epsilon" }, { @"\zeta", "zeta" }, { @"\eta", "eta" }, { @"\theta", "theta" },
            { @"\iota", "iota" }, { @"\kappa", "kappa" }, { @"\lambda", "lambda" }, { @"\mu", "mu" },
            { @"\nu", "nu" }, { @"\xi", "xi" }, { @"\omicron", "omicron" }, { @"\pi", "pi" },
            { @"\rho", "rho" }, { @"\sigma", "sigma" }, { @"\tau", "tau" }, { @"\upsilon", "upsilon" },
            { @"\phi", "phi" }, { @"\chi", "chi" }, { @"\psi", "psi" }, { @"\omega", "omega" },
            { @"\Gamma", "Gamma" }, { @"\Delta", "Delta" }, { @"\Theta", "Theta" }, { @"\Lambda", "Lambda" },
            { @"\Xi", "Xi" }, { @"\Pi", "Pi" }, { @"\Sigma", "Sigma" }, { @"\Upsilon", "Upsilon" },
            { @"\Phi", "Phi" }, { @"\Psi", "Psi" }, { @"\Omega", "Omega" },

            // Mathematical Symbols
            { @"\times", "times" }, { @"\div", "divided by" }, { @"\pm", "plus-minus" },
            { @"\mp", "minus-plus" }, { @"\cdot", "dot" }, { @"\circ", "circle" },
            { @"\bullet", "bullet" }, { @"\oplus", "oplus" }, { @"\ominus", "ominus" },
            { @"\otimes", "otimes" }, { @"\oslash", "oslash" }, { @"\odot", "odot" },
            { @"\leq", "less than or equal to" }, { @"\geq", "greater than or equal to" },
            { @"\neq", "not equal to" }, { @"\approx", "approximately equal to" },
            { @"\equiv", "equivalent to" }, { @"\propto", "proportional to" },
            { @"\infty", "infinity" }, { @"\nabla", "nabla" }, { @"\partial", "partial derivative" },
            { @"\int", "integral" }, { @"\sum", "summation" }, { @"\prod", "product" },

            // Physics Symbols
            { @"\hbar", "h-bar" }, { @"\ell", "ell" }, { @"\wp", "Weierstrass p" },
            { @"\Re", "Real part" }, { @"\Im", "Imaginary part" },
        };

        private static readonly Dictionary<string, string> HumanFriendlySymbolMap = new Dictionary<string, string>
        {
            // Greek Letters
            { @"\alpha", "α" }, { @"\beta", "β" }, { @"\gamma", "γ" }, { @"\delta", "δ" },
            { @"\epsilon", "ε" }, { @"\zeta", "ζ" }, { @"\eta", "η" }, { @"\theta", "θ" },
            { @"\iota", "ι" }, { @"\kappa", "κ" }, { @"\lambda", "λ" }, { @"\mu", "μ" },
            { @"\nu", "ν" }, { @"\xi", "ξ" }, { @"\omicron", "ο" }, { @"\pi", "π" },
            { @"\rho", "ρ" }, { @"\sigma", "σ" }, { @"\tau", "τ" }, { @"\upsilon", "υ" },
            { @"\phi", "φ" }, { @"\chi", "χ" }, { @"\psi", "ψ" }, { @"\omega", "ω" },
            { @"\Gamma", "Γ" }, { @"\Delta", "Δ" }, { @"\Theta", "Θ" }, { @"\Lambda", "Λ" },
            { @"\Xi", "Ξ" }, { @"\Pi", "Π" }, { @"\Sigma", "Σ" }, { @"\Upsilon", "Υ" },
            { @"\Phi", "Φ" }, { @"\Psi", "Ψ" }, { @"\Omega", "Ω" },

            // Mathematical Symbols
            { @"\times", "×" }, { @"\div", "÷" }, { @"\pm", "±" },
            { @"\mp", "∓" }, { @"\cdot", "·" }, { @"\circ", "∘" },
            { @"\bullet", "•" }, { @"\oplus", "⊕" }, { @"\ominus", "⊖" },
            { @"\otimes", "⊗" }, { @"\oslash", "⊘" }, { @"\odot", "⊙" },
            { @"\leq", "≤" }, { @"\geq", "≥" }, { @"\neq", "≠" }, { @"\approx", "≈" },
            { @"\equiv", "≡" }, { @"\propto", "∝" }, { @"\infty", "∞" },
            { @"\nabla", "∇" }, { @"\partial", "∂" },
            { @"\int", "∫" }, { @"\sum", "∑" }, { @"\prod", "∏" },

            // Physics Symbols
            { @"\hbar", "ħ" }, { @"\ell", "ℓ" },
        };

        private static readonly Dictionary<char, char> SupMap = new Dictionary<char, char>
        {
            { '0', '⁰' }, { '1', '¹' }, { '2', '²' }, { '3', '³' }, { '4', '⁴' },
            { '5', '⁵' }, { '6', '⁶' }, { '7', '⁷' }, { '8', '⁸' }, { '9', '⁹' }
        };

        private static readonly Dictionary<char, char> SubMap = new Dictionary<char, char>
        {
            { '0', '₀' }, { '1', '₁' }, { '2', '₂' }, { '3', '₃' }, { '4', '₄' },
            { '5', '₅' }, { '6', '₆' }, { '7', '₇' }, { '8', '₈' }, { '9', '₉' }
        };

        #endregion

        private string ProcessLatex(string latexInput, Func<string, string> pass)
        {
            string text = latexInput;
            string previous;
            do
            {
                previous = text;
                text = pass(text);
            } while (text != previous);
            return text;
        }

        #region OpenAI Conversion
        public string ConvertToOpenAIFriendlyText(string latex_input)
        {
            return ProcessLatex(latex_input, OpenAIPass);
        }

        private string OpenAIPass(string text)
        {
            text = Regex.Replace(text, @"\^\{(.+?)\}", "^($1)");
            text = Regex.Replace(text, @"_\{(.+?)\}", "_($1)");
            text = Regex.Replace(text, @"\^(\S)", "^($1)");
            text = Regex.Replace(text, @"_(\S)", "_($1)");

            text = Regex.Replace(text, @"\\frac\{(.+?)\}\{(.+?)\}", "($1)/($2)");
            text = Regex.Replace(text, @"\\sqrt\{(.+?)\}", "sqrt($1)");

            foreach (var pair in SymbolMap)
            {
                text = text.Replace(pair.Key, $"[{pair.Value}]");
            }
            return text;
        }
        #endregion

        #region Human-Friendly Conversion
        public string ConvertToHumanFriendlyText(string latex_input)
        {
            return ProcessLatex(latex_input, HumanFriendlyPass);
        }

        private string HumanFriendlyPass(string text)
        {
            text = Regex.Replace(text, @"\^\{(.+?)\}", m => Superscript(m.Groups[1].Value));
            text = Regex.Replace(text, @"\^([0-9])", m => Superscript(m.Groups[1].Value));
            text = Regex.Replace(text, @"_\{(.+?)\}", m => Subscript(m.Groups[1].Value));
            text = Regex.Replace(text, @"_([0-9])", m => Subscript(m.Groups[1].Value));

            text = Regex.Replace(text, @"\\frac\{(.+?)\}\{(.+?)\}", "$1/$2");
            text = Regex.Replace(text, @"\\sqrt\{(.+?)\}", "√($1)");

            foreach (var pair in HumanFriendlySymbolMap)
            {
                text = text.Replace(pair.Key, pair.Value);
            }

            text = text.Replace("{", "").Replace("}", "");
            return text;
        }

        private string Superscript(string s)
        {
            var result = "";
            foreach (var c in s)
            {
                result += SupMap.TryGetValue(c, out var sup) ? sup : c;
            }
            return result;
        }

        private string Subscript(string s)
        {
            var result = "";
            foreach (var c in s)
            {
                result += SubMap.TryGetValue(c, out var sub) ? sub : c;
            }
            return result;
        }
        #endregion

        #region Screen Reader Conversion
        public string ConvertToScreenReaderFriendlyText(string latex_input)
        {
            return ProcessLatex(latex_input, ScreenReaderPass);
        }

        private string ScreenReaderPass(string text)
        {
            text = Regex.Replace(text, @"\\sum_\{(.+?)\}\^\{(.+?)\}", " summation from $1 to $2 ");
            text = Regex.Replace(text, @"\\int_\{(.+?)\}\^\{(.+?)\}", " integral from $1 to $2 ");

            text = Regex.Replace(text, @"\\frac\{(.+?)\}\{(.+?)\}", " fraction with numerator $1 and denominator $2 ");
            text = Regex.Replace(text, @"\\sqrt\{(.+?)\}", " the square root of $1 ");

            text = Regex.Replace(text, @"\^\{(.+?)\}", m => PowerToText(m.Groups[1].Value));
            text = Regex.Replace(text, @"\^(\S)", m => PowerToText(m.Groups[1].Value));

            text = Regex.Replace(text, @"_\{(.+?)\}", " subscript $1 ");
            text = Regex.Replace(text, @"_(\S)", " subscript $1 ");

            foreach (var pair in SymbolMap)
            {
                text = text.Replace(pair.Key, $" {pair.Value} ");
            }

            text = text.Replace("{", "").Replace("}", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        private string PowerToText(string content)
        {
            if (content == "2") return " squared";
            if (content == "3") return " cubed";
            return $" to the power of {content} ";
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LatexConverter
{
    public class LaTexConverter
    {
        #region Dictionaries
        private static readonly Dictionary<string, string> SymbolMap = new Dictionary<string, string>
        {
            { @"\alpha", "alpha" }, { @"\beta", "beta" }, { @"\gamma", "gamma" }, { @"\delta", "delta" },
            { @"\epsilon", "epsilon" }, { @"\zeta", "zeta" }, { @"\eta", "eta" }, { @"\theta", "theta" },
            { @"\iota", "iota" }, { @"\kappa", "kappa" }, { @"\lambda", "lambda" }, { @"\mu", "mu" },
            { @"\nu", "nu" }, { @"\xi", "xi" }, { @"\omicron", "omicron" }, { @"\pi", "pi" },
            { @"\rho", "rho" }, { @"\sigma", "sigma" }, { @"\tau", "tau" }, { @"\upsilon", "upsilon" },
            { @"\phi", "phi" }, { @"\chi", "chi" }, { @"\psi", "psi" }, { @"\omega", "omega" },
            { @"\Gamma", "Gamma" }, { @"\Delta", "Delta" }, { @"\Theta", "Theta" }, { @"\Lambda", "Lambda" },
            { @"\Xi", "Xi" }, { @"\Pi", "Pi" }, { @"\Sigma", "Sigma" }, { @"\Upsilon", "Upsilon" },
            { @"\Phi", "Phi" }, { @"\Psi", "Psi" }, { @"\Omega", "Omega" },
            { @"\times", "times" }, { @"\div", "divided by" }, { @"\pm", "plus-minus" },
            { @"\mp", "minus-plus" }, { @"\cdot", "dot" }, { @"\circ", "circle" },
            { @"\bullet", "bullet" }, { @"\oplus", "oplus" }, { @"\ominus", "ominus" },
            { @"\otimes", "otimes" }, { @"\oslash", "oslash" }, { @"\odot", "odot" },
            { @"\leq", "less than or equal to" }, { @"\geq", "greater than or equal to" },
            { @"\neq", "not equal to" }, { @"\approx", "approximately equal to" },
            { @"\equiv", "equivalent to" }, { @"\propto", "proportional to" },
            { @"\infty", "infinity" }, { @"\nabla", "nabla" }, { @"\partial", "partial derivative" },
            { @"\int", "integral" }, { @"\sum", "summation" }, { @"\prod", "product" },
            { @"\hbar", "h-bar" }, { @"\ell", "ell" }, { @"\wp", "Weierstrass p" },
            { @"\Re", "Real part" }, { @"\Im", "Imaginary part" },
        };

        private static readonly Dictionary<string, string> HumanFriendlySymbolMap = new Dictionary<string, string>
        {
            { @"\alpha", "α" }, { @"\beta", "β" }, { @"\gamma", "γ" }, { @"\delta", "δ" },
            { @"\epsilon", "ε" }, { @"\zeta", "ζ" }, { @"\eta", "η" }, { @"\theta", "θ" },
            { @"\iota", "ι" }, { @"\kappa", "κ" }, { @"\lambda", "λ" }, { @"\mu", "μ" },
            { @"\nu", "ν" }, { @"\xi", "ξ" }, { @"\omicron", "ο" }, { @"\pi", "π" },
            { @"\rho", "ρ" }, { @"\sigma", "σ" }, { @"\tau", "τ" }, { @"\upsilon", "υ" },
            { @"\phi", "φ" }, { @"\chi", "χ" }, { @"\psi", "ψ" }, { @"\omega", "ω" },
            { @"\Gamma", "Γ" }, { @"\Delta", "Δ" }, { @"\Theta", "Θ" }, { @"\Lambda", "Λ" },
            { @"\Xi", "Ξ" }, { @"\Pi", "Π" }, { @"\Sigma", "Σ" }, { @"\Upsilon", "Υ" },
            { @"\Phi", "Φ" }, { @"\Psi", "Ψ" }, { @"\Omega", "Ω" },
            { @"\times", "×" }, { @"\div", "÷" }, { @"\pm", "±" },
            { @"\mp", "∓" }, { @"\cdot", "·" }, { @"\circ", "∘" },
            { @"\bullet", "•" }, { @"\oplus", "⊕" }, { @"\ominus", "⊖" },
            { @"\otimes", "⊗" }, { @"\oslash", "⊘" }, { @"\odot", "⊙" },
            { @"\leq", "≤" }, { @"\geq", "≥" }, { @"\neq", "≠" }, { @"\approx", "≈" },
            { @"\equiv", "≡" }, { @"\propto", "∝" }, { @"\infty", "∞" },
            { @"\nabla", "∇" }, { @"\partial", "∂" },
            { @"\int", "∫" }, { @"\sum", "∑" }, { @"\prod", "∏" },
            { @"\hbar", "ħ" }, { @"\ell", "ℓ" },
        };

        private static readonly Dictionary<char, char> SupMap = new Dictionary<char, char>
        {
            { '0', '⁰' }, { '1', '¹' }, { '2', '²' }, { '3', '³' }, { '4', '⁴' }, { '5', '⁵' }, { '6', '⁶' }, { '7', '⁷' }, { '8', '⁸' }, { '9', '⁹' },
            { 'a', 'ᵃ' }, { 'b', 'ᵇ' }, { 'c', 'ᶜ' }, { 'd', 'ᵈ' }, { 'e', 'ᵉ' }, { 'f', 'ᶠ' }, { 'g', 'ᵍ' }, { 'h', 'ʰ' }, { 'i', 'ⁱ' }, { 'j', 'ʲ' },
            { 'k', 'ᵏ' }, { 'l', 'ˡ' }, { 'm', 'ᵐ' }, { 'n', 'ⁿ' }, { 'o', 'ᵒ' }, { 'p', 'ᵖ' }, { 'r', 'ʳ' }, { 's', 'ˢ' }, { 't', 'ᵗ' }, { 'u', 'ᵘ' },
            { 'v', 'ᵛ' }, { 'w', 'ʷ' }, { 'x', 'ˣ' }, { 'y', 'ʸ' }, { 'z', 'ᶻ' }, { '+', '⁺' }, { '-', '⁻' }, { '=', '⁼' }, { '(', '⁽' }, { ')', '⁾' },
            { 'A', 'ᴬ' }, { 'B', 'ᴮ' }, { 'C', 'ᶜ' }, { 'D', 'ᴰ' }, { 'E', 'ᴱ' }, { 'G', 'ᴳ' }, { 'H', 'ᴴ' }, { 'I', 'ᴵ' }, { 'J', 'ᴶ' }, { 'K', 'ᴷ' }, { 'L', 'ᴸ' },
            { 'M', 'ᴹ' }, { 'N', 'ᴺ' }, { 'O', 'ᴼ' }, { 'P', 'ᴾ' }, { 'R', 'ᴿ' }, { 'T', 'ᵀ' }, { 'U', 'ᵁ' }, { 'V', 'ⱽ' }, { 'W', 'ᵂ' }, { 'Y', 'ʸ' }, { 'Z', 'ᶻ' }
        };

        private static readonly Dictionary<char, char> SubMap = new Dictionary<char, char>
        {
            { '0', '₀' }, { '1', '₁' }, { '2', '₂' }, { '3', '₃' }, { '4', '₄' }, { '5', '₅' }, { '6', '₆' }, { '7', '₇' }, { '8', '₈' }, { '9', '₉' },
            { 'a', 'ₐ' }, { 'e', 'ₑ' }, { 'h', 'ₕ' }, { 'i', 'ᵢ' }, { 'j', 'ⱼ' }, { 'k', 'ₖ' }, { 'l', 'ₗ' }, { 'm', 'ₘ' }, { 'n', 'ₙ' }, { 'o', 'ₒ' },
            { 'p', 'ₚ' }, { 'r', 'ᵣ' }, { 's', 'ₛ' }, { 't', 'ₜ' }, { 'u', 'ᵤ' }, { 'v', 'ᵥ' }, { 'x', 'ₓ' }, { '+', '₊' }, { '-', '₋' }, { '=', '₌' },
            { '(', '₍' }, { ')', '₎' }
        };
        #endregion

        #region Converters
        public string ConvertToOpenAIFriendlyText(string latex_input) => RecursiveConverter(latex_input, OpenAIPass);
        public string ConvertToHumanFriendlyText(string latex_input) => RecursiveConverter(latex_input, HumanFriendlyPass);
        public string ConvertToScreenReaderFriendlyText(string latex_input) => RecursiveConverter(latex_input, ScreenReaderPass);
        #endregion

        private string RecursiveConverter(string text, Func<string, string> pass)
        {
            string previous;
            int i = 0;
            const int maxIterations = 100;
            do
            {
                previous = text;
                text = pass(text);
                i++;
            } while (text != previous && i < maxIterations);
            return text;
        }

        #region Passes
        private string OpenAIPass(string text)
        {
            text = Regex.Replace(text, @"\\frac{([^{}]*?)}{([^{}]*?)}", m => $"({OpenAIPass(m.Groups[1].Value)})/({OpenAIPass(m.Groups[2].Value)})");
            text = Regex.Replace(text, @"\\sqrt{([^{}]*?)}", m => $"sqrt({OpenAIPass(m.Groups[1].Value)})");
            text = Regex.Replace(text, @"\\vec{([^{}]*?)}", m => $"vec({OpenAIPass(m.Groups[1].Value)})");
            text = Regex.Replace(text, @"\\text{([^{}]*?)}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\\mathrm{([^{}]*?)}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\^{([^{}]*?)}", m => $"^({OpenAIPass(m.Groups[1].Value)})");
            text = Regex.Replace(text, @"_{([^{}]*?)}", m => $"_({OpenAIPass(m.Groups[1].Value)})");
            text = Regex.Replace(text, @"\^([a-zA-Z0-9])", "^($1)");
            text = Regex.Replace(text, @"_([a-zA-Z0-9])", "_($1)");
            text = SymbolMap.Aggregate(text, (current, pair) => current.Replace(pair.Key, $"[{pair.Value}]"));
            return text;
        }

        private string HumanFriendlyPass(string text)
        {
            text = Regex.Replace(text, @"\\frac{([^{}]*?)}{([^{}]*?)}", m => $"({HumanFriendlyPass(m.Groups[1].Value)})/({HumanFriendlyPass(m.Groups[2].Value)})");
            text = Regex.Replace(text, @"\\sqrt{([^{}]*?)}", m => $"√({HumanFriendlyPass(m.Groups[1].Value)})");
            text = Regex.Replace(text, @"\\vec{([^{}]*?)}", m => $"{HumanFriendlyPass(m.Groups[1].Value)}\u20D7");
            text = Regex.Replace(text, @"\\text{([^{}]*?)}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\\mathrm{([^{}]*?)}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\^{([^{}]*?)}", m => ToUnicode(m.Groups[1].Value, true));
            text = Regex.Replace(text, @"_{([^{}]*?)}", m => ToUnicode(m.Groups[1].Value, false));
            text = Regex.Replace(text, @"\^([a-zA-Z0-9])", m => ToUnicode(m.Groups[1].Value, true));
            text = Regex.Replace(text, @"_([a-zA-Z0-9])", m => ToUnicode(m.Groups[1].Value, false));
            text = HumanFriendlySymbolMap.Aggregate(text, (current, pair) => current.Replace(pair.Key, pair.Value));
            return text;
        }

        private string ScreenReaderPass(string text)
        {
            text = Regex.Replace(text, @"\\sum_\{([^{}]*?)\}\^\{([^{}]*?)\}", m => $"summation from {ScreenReaderPass(m.Groups[1].Value)} to {ScreenReaderPass(m.Groups[2].Value)}");
            text = Regex.Replace(text, @"\\int_\{([^{}]*?)\}\^\{([^{}]*?)\}", m => $"integral from {ScreenReaderPass(m.Groups[1].Value)} to {ScreenReaderPass(m.Groups[2].Value)}");
            text = Regex.Replace(text, @"\\frac\{([^{}]*?)\}\{([^{}]*?)\}", m => $"fraction with numerator {ScreenReaderPass(m.Groups[1].Value)} and denominator {ScreenReaderPass(m.Groups[2].Value)}");
            text = Regex.Replace(text, @"\\sqrt\{([^{}]*?)\}", m => $"the square root of {ScreenReaderPass(m.Groups[1].Value)}");
            text = Regex.Replace(text, @"\\vec\{([^{}]*?)\}", m => $"vector {ScreenReaderPass(m.Groups[1].Value)}");
            text = Regex.Replace(text, @"\\text\{([^{}]*?)\}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\\mathrm\{([^{}]*?)\}", m => m.Groups[1].Value);
            text = Regex.Replace(text, @"\^\{([^{}]*?)\}", m => PowerToText(m.Groups[1].Value));
            text = Regex.Replace(text, @"_\{([^{}]*?)\}", m => $" subscript {ScreenReaderPass(m.Groups[1].Value)}");
            text = Regex.Replace(text, @"\^([a-zA-Z0-9])", m => PowerToText(m.Groups[1].Value));
            text = Regex.Replace(text, @"_([a-zA-Z0-9])", m => $" subscript {m.Groups[1].Value}");
            text = SymbolMap.Aggregate(text, (current, pair) => current.Replace(pair.Key, $" {pair.Value} "));
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }
        #endregion

        #region Unicode Helpers
        private string ToUnicode(string s, bool isSuperscript)
        {
            string processed = HumanFriendlyPass(s);
            var map = isSuperscript ? SupMap : SubMap;
            var sb = new StringBuilder();

            if (Regex.IsMatch(processed, @"[\(\)\√]"))
            {
                 return isSuperscript ? $"^({processed})" : $"_({processed})";
            }

            foreach (char c in processed)
            {
                if (!map.TryGetValue(c, out char newChar))
                {
                    return isSuperscript ? $"^({processed})" : $"_({processed})";
                }
                sb.Append(newChar);
            }
            return sb.ToString();
        }

        private string PowerToText(string content)
        {
            return $" to the power of {ScreenReaderPass(content)}";
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
    /// <summary>
    /// Provides centralized storage for symbol and character mappings used in LaTeX conversion.
    /// </summary>
    internal static class Dictionaries
    {
        /// <summary>
        /// A comprehensive map of LaTeX commands to their plain text representations.
        /// </summary>
        public static readonly Dictionary<string, string> SymbolMap = new() {
            { @"\alpha", "alpha" }, { @"\beta", "beta" }, { @"\gamma", "gamma" }, { @"\delta", "delta" },
            { @"\epsilon", "epsilon" }, { @"\varepsilon", "varepsilon" }, { @"\zeta", "zeta" }, { @"\eta", "eta" }, { @"\theta", "theta" },
            { @"\iota", "iota" }, { @"\kappa", "kappa" }, { @"\varkappa", "varkappa" }, { @"\lambda", "lambda" }, { @"\mu", "mu" },
            { @"\nu", "nu" }, { @"\xi", "xi" }, { @"\omicron", "omicron" }, { @"\pi", "pi" }, { @"\varpi", "varpi" },
            { @"\rho", "rho" }, { @"\varrho", "varrho" }, { @"\sigma", "sigma" }, { @"\varsigma", "varsigma" }, { @"\tau", "tau" }, { @"\upsilon", "upsilon" },
            { @"\phi", "phi" }, { @"\varphi", "varphi" }, { @"\chi", "chi" }, { @"\psi", "psi" }, { @"\omega", "omega" },
            { @"\Gamma", "Gamma" }, { @"\Delta", "Delta" }, { @"\Theta", "Theta" }, { @"\Lambda", "Lambda" },
            { @"\Xi", "Xi" }, { @"\Pi", "Pi" }, { @"\Sigma", "Sigma" }, { @"\Upsilon", "Upsilon" },
            { @"\Phi", "Phi" }, { @"\Psi", "Psi" }, { @"\Omega", "Omega" },
            { @"\times", "times" }, { @"\div", "div" }, { @"\pm", "pm" },
            { @"\mp", "mp" }, { @"\cdot", "cdot" }, { @"\circ", "circ" },
            { @"\bullet", "bullet" }, { @"\oplus", "oplus" }, { @"\ominus", "ominus" },
            { @"\otimes", "otimes" }, { @"\oslash", "oslash" }, { @"\odot", "odot" },
            { @"\parallel", "parallel" }, { @"\perp", "perp" },
            { @"\implies", "implies" }, { @"\iff", "iff" },
            { @"\rightarrow", "rightarrow" }, { @"\leftarrow", "leftarrow" }, { @"\uparrow", "uparrow" }, { @"\downarrow", "downarrow" },
            { @"\Rightarrow", "Rightarrow" }, { @"\Leftarrow", "Leftarrow" },
            { @"\leftrightarrow", "leftrightarrow" }, { @"\Leftrightarrow", "Leftrightarrow" },
            { @"\leq", "leq" }, { @"\geq", "geq" },
            { @"\neq", "neq" }, { @"\approx", "approx" }, { @"\equiv", "equiv" }, { @"\propto", "propto" },
            { @"\infty", "infty" }, { @"\nabla", "nabla" }, { @"\partial", "partial" },
            { @"\int", "integral" }, { @"\sum", "summation" }, { @"\prod", "product" }, { @"\lim", "limit" },
            { @"\hbar", "hbar" }, { @"\ell", "ell" }, { @"\wp", "wp" },
            { @"\Re", "Re" }, { @"\Im", "Im" },
            { @"\forall", "forall" }, { @"\exists", "exists" }, { @"\in", "in" }, { @"\to", "->" },
            { @"\arcsin", "arcsin" }, { @"\arccos", "arccos" }, { @"\arctan", "arctan" },
            { @"\cup", "cup" }, { @"\cap", "cap" }, { @"\subset", "subset" }, { @"\supset", "supset" },
            { @"\neg", "neg" }, { @"\land", "land" }, { @"\lor", "lor" }, { @"\prime", "prime" },
            { @"\blacksquare", "blacksquare" }, { @"\heartsuit", "heartsuit" }, { @"\clubsuit", "clubsuit" },
            { @"\diamondsuit", "diamondsuit" }, { @"\spadesuit", "spadesuit" }, { @"\natural", "natural" },
            { @"\sharp", "sharp" }, { @"\flat", "flat" }, { @"\triangle", "triangle" }, { @"\triangledown", "triangledown" },
            { @"\angle", "angle" }, { @"\measuredangle", "measuredangle" },
            { @"\dots", "..." }, { @"\cdots", "..." }, { @"\vdots", "..." }, { @"\ddots", "..." },
            { @"\,", " " }
        };
        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly representations.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderSymbolMap = new() {
            { @"\alpha", "alpha" }, { @"\beta", "beta" }, { @"\gamma", "gamma" }, { @"\delta", "delta" },
            { @"\epsilon", "epsilon" }, { @"\varepsilon", "epsilon" }, { @"\zeta", "zeta" }, { @"\eta", "eta" }, { @"\theta", "theta" },
            { @"\iota", "iota" }, { @"\kappa", "kappa" }, { @"\varkappa", "kappa" }, { @"\lambda", "lambda" }, { @"\mu", "mu" },
            { @"\nu", "nu" }, { @"\xi", "xi" }, { @"\omicron", "omicron" }, { @"\pi", "pi" }, { @"\varpi", "pi" },
            { @"\rho", "rho" }, { @"\varrho", "rho" }, { @"\sigma", "sigma" }, { @"\varsigma", "sigma" }, { @"\tau", "tau" }, { @"\upsilon", "upsilon" },
            { @"\phi", "phi" }, { @"\varphi", "phi" }, { @"\chi", "chi" }, { @"\psi", "psi" }, { @"\omega", "omega" },
            { @"\Gamma", "Gamma" }, { @"\Delta", "Delta" }, { @"\Theta", "Theta" }, { @"\Lambda", "Lambda" },
            { @"\Xi", "Xi" }, { @"\Pi", "Pi" }, { @"\Sigma", "Sigma" }, { @"\Upsilon", "Upsilon" },
            { @"\Phi", "Phi" }, { @"\Psi", "Psi" }, { @"\Omega", "Omega" },
            { @"\times", "times" }, { @"\div", "divided by" }, { @"\pm", "plus-minus" }, { @"\mp", "minus-plus" }, { @"\cdot", "dot" }, { @"\circ", "degrees" },
            { @"\otimes", "tensor product" }, { @"\odot", "circled dot" },
            { @"\parallel", "parallel to" }, { @"\perp", "perpendicular to" },
            { @"\implies", "implies" }, { @"\iff", "if and only if" },
            { @"\rightarrow", "right arrow" }, { @"\leftarrow", "left arrow" }, { @"\uparrow", "up arrow" }, { @"\downarrow", "down arrow" },
            { @"\Rightarrow", "rightwards double arrow" }, { @"\Leftarrow", "leftwards double arrow" },
            { @"\leftrightarrow", "left right arrow" }, { @"\Leftrightarrow", "left right double arrow" },
            { @"\leq", "less than or equal to" }, { @"\geq", "greater than or equal to" },
            { @"\neq", "not equal to" }, { @"\approx", "approximately equal to" }, { @"\equiv", "equivalent to" }, { @"\propto", "proportional to" },
            { @"\infty", "infinity" }, { @"\nabla", "nabla" }, { @"\partial", "partial derivative" }, { @"\hbar", "h-bar" }, { @"\wp", "Weierstrass p" },
            { @"\Re", "Real part" }, { @"\Im", "Imaginary part" },
            { @"\forall", "for all" }, { @"\exists", "exists" }, { @"\in", "in" }, { @"\to", "approaches" },
            { @"\arcsin", "arcsin" }, { @"\arccos", "arccos" }, { @"\arctan", "arctan" },
            { @"\cup", "union" }, { @"\cap", "intersection" }, { @"\subset", "subset of" }, { @"\supset", "superset of" },
            { @"\neg", "not" }, { @"\land", "and" }, { @"\lor", "or" }, { @"\prime", "prime" },
            { @"\blacksquare", "black square" }, { @"\heartsuit", "heart suit" }, { @"\clubsuit", "club suit" },
            { @"\diamondsuit", "diamond suit" }, { @"\spadesuit", "spade suit" }, { @"\natural", "natural" },
            { @"\sharp", "sharp" }, { @"\flat", "flat" }, { @"\triangle", "triangle" }, { @"\triangledown", "downward triangle" },
            { @"\angle", "angle" }, { @"\measuredangle", "measured angle" },
            { @"\dots", "dots" }, { @"\cdots", "centered dots" }, { @"\vdots", "vertical dots" }, { @"\ddots", "diagonal dots" }
        };
        /// <summary>
        /// A map of LaTeX commands to their human-friendly Unicode representations.
        /// </summary>
        public static readonly Dictionary<string, string> HumanFriendlySymbolMap = new() {
            { @"\alpha", "α" }, { @"\beta", "β" }, { @"\gamma", "γ" }, { @"\delta", "δ" },
            { @"\epsilon", "ε" }, { @"\zeta", "ζ" }, { @"\eta", "η" }, { @"\theta", "θ" },
            { @"\iota", "ι" }, { @"\kappa", "κ" }, { @"\varkappa", "ϰ" }, { @"\lambda", "λ" }, { @"\mu", "μ" },
            { @"\nu", "ν" }, { @"\xi", "ξ" }, { @"\omicron", "ο" }, { @"\pi", "π" }, { @"\varpi", "ϖ" },
            { @"\rho", "ρ" }, { @"\varrho", "ϱ" }, { @"\sigma", "σ" }, { @"\varsigma", "ς" }, { @"\tau", "τ" }, { @"\upsilon", "υ" },
            { @"\phi", "φ" }, { @"\varphi", "\u03D5" }, { @"\chi", "χ" }, { @"\psi", "ψ" }, { @"\omega", "ω" },
            { @"\Gamma", "Γ" }, { @"\Delta", "Δ" }, { @"\Theta", "Θ" }, { @"\Lambda", "Λ" },
            { @"\Xi", "Ξ" }, { @"\Pi", "Π" }, { @"\Sigma", "Σ" }, { @"\Upsilon", "Υ" },
            { @"\Phi", "Φ" }, { @"\Psi", "Ψ" }, { @"\Omega", "Ω" },
            { @"\times", "×" }, { @"\div", "÷" }, { @"\pm", "±" },
            { @"\mp", "∓" }, { @"\cdot", "·" }, { @"\circ", "∘" },
            { @"\bullet", "•" }, { @"\oplus", "⊕" }, { @"\ominus", "⊖" },
            { @"\otimes", "⊗" }, { @"\oslash", "⊘" }, { @"\odot", "⊙" },
            { @"\parallel", "∥" }, { @"\perp", "⊥" },
            { @"\implies", "⇒" }, { @"\iff", "⇔" },
            { @"\Re", "ℜ" }, { @"\Im", "ℑ" },
            { @"\rightarrow", "→" }, { @"\leftarrow", "←" }, { @"\uparrow", "↑" }, { @"\downarrow", "↓" },
            { @"\Rightarrow", "⇒" }, { @"\Leftarrow", "⇐" },
            { @"\leftrightarrow", "↔" }, { @"\Leftrightarrow", "⇔" },
            { @"\leq", "≤" }, { @"\geq", "≥" }, { @"\neq", "≠" }, { @"\approx", "≈" }, { @"\equiv", "≡" }, { @"\propto", "∝" }, { @"\infty", "∞" },
            { @"\nabla", "∇" }, { @"\partial", "∂" },
            { @"\int", "∫" }, { @"\sum", "∑" }, { @"\prod", "∏" }, { @"\lim", "lim" },
            { @"\hbar", "ħ" }, { @"\ell", "ℓ" },
            { @"\forall", "∀" }, { @"\exists", "∃" }, { @"\in", "∈" }, { @"\to", "→" },
            { @"\arcsin", "sin⁻¹" }, { @"\arccos", "cos⁻¹" }, { @"\arctan", "tan⁻¹" },
            { @"\cup", "∪" }, { @"\cap", "∩" }, { @"\subset", "⊂" }, { @"\supset", "⊃" },
            { @"\neg", "¬" }, { @"\land", "∧" }, { @"\lor", "∨" }, { @"\prime", "′" },
            { @"\blacksquare", "■" }, { @"\heartsuit", "♥" }, { @"\clubsuit", "♣" },
            { @"\diamondsuit", "♦" }, { @"\spadesuit", "♠" }, { @"\natural", "♮" },
            { @"\sharp", "♯" }, { @"\flat", "♭" }, { @"\triangle", "△" }, { @"\triangledown", "▽" },
            { @"\angle", "∠" }, { @"\measuredangle", "∡" },
            { @"\dots", "…" }, { @"\cdots", "⋯" }, { @"\vdots", "⋮" }, { @"\ddots", "⋱" },
            { @"\,", " " }
        };
        /// <summary>
        /// A map of Unicode symbols to their plain text representations.
        /// </summary>
        public static readonly Dictionary<string, string> ReverseHumanFriendlySymbolMap = HumanFriendlySymbolMap.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.First().Key.Substring(1));
        /// <summary>
        /// A list of LaTeX commands that should not be converted from plain text without a leading backslash.
        /// </summary>
        public static readonly List<string> DeniedConvertWithoutSlash = new List<string>() { @"\bullet", @"\in", @"\times", @"\sum", @"\exists", @"\to", @"\angle", @"\triangle", @"\natural", @"\sharp", @"\parallel", @"\prod", @"\flat", @"\lim" };

        /// <summary>
        /// A map of human-friendly Unicode symbols to their screen reader-friendly text representations.
        /// </summary>
        public static readonly Dictionary<string, string> HumanToScreenReaderMap = GetHumanToScreenReaderMap();

        private static Dictionary<string, string> GetHumanToScreenReaderMap()
        {
            var map = HumanFriendlySymbolMap
                .GroupBy(kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => ScreenReaderSymbolMap.GetValueOrDefault(g.First().Key, SymbolMap.GetValueOrDefault(g.First().Key, "")));
            map["="] = "equals";
            map["/"] = "divided by";
            map["*"] = "times";
            map["·"] = "cdot";
            map["×"] = "times";
            map["°"] = "degrees";
            map["√"] = "the square root of";
            map["ℝ"] = "the set of real numbers";
            map["ℰ"] = "calligraphic E";
            map["ℭ"] = "frak C";
            map["ℋ"] = "scr H";
            return map;
        }

        /// <summary>
        /// A map of characters to their superscript Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> SupMap = new() {
            { '0', '⁰' }, { '1', '¹' }, { '2', '²' }, { '3', '³' }, { '4', '⁴' }, { '5', '⁵' }, { '6', '⁶' }, { '7', '⁷' }, { '8', '⁸' }, { '9', '⁹' },
            { 'a', 'ᵃ' }, { 'b', 'ᵇ' }, { 'c', 'ᶜ' }, { 'd', 'ᵈ' }, { 'e', 'ᵉ' }, { 'f', 'ᶠ' }, { 'g', 'ᵍ' }, { 'h', 'ʰ' }, { 'i', 'ⁱ' }, { 'j', 'ʲ' },
            { 'k', 'ᵏ' }, { 'l', 'ˡ' }, { 'm', 'ᵐ' }, { 'n', 'ⁿ' }, { 'o', 'ᵒ' }, { 'p', 'ᵖ' }, { 'r', 'ʳ' }, { 's', 'ˢ' }, { 't', 'ᵗ' }, { 'u', 'ᵘ' },
            { 'v', 'ᵛ' }, { 'w', 'ʷ' }, { 'x', 'ˣ' }, { 'y', 'ʸ' }, { 'z', 'ᶻ' }, { '+', '⁺' }, { '-', '⁻' }, { '=', '⁼' }, { '(', '⁽' }, { ')', '⁾' },
            { 'A', 'ᴬ' }, { 'B', 'ᴮ' }, { 'D', 'ᴰ' }, { 'E', 'ᴱ' }, { 'G', 'ᴳ' }, { 'H', 'ᴴ' }, { 'I', 'ᴵ' }, { 'J', 'ᴶ' }, { 'K', 'ᴷ' }, { 'L', 'ᴸ' },
            { 'M', 'ᴹ' }, { 'N', 'ᴺ' }, { 'O', 'ᴼ' }, { 'P', 'ᴾ' }, { 'R', 'ᴿ' }, { 'T', 'ᵀ' }, { 'U', 'ᵁ' }, { 'V', 'ⱽ' }, { 'W', 'ᵂ' }
        };
        /// <summary>
        /// A map of characters to their subscript Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> SubMap = new() {
            { '0', '₀' }, { '1', '₁' }, { '2', '₂' }, { '3', '₃' }, { '4', '₄' }, { '5', '₅' }, { '6', '₆' }, { '7', '₇' }, { '8', '₈' }, { '9', '₉' },
            { 'a', 'ₐ' }, { 'e', 'ₑ' }, { 'h', 'ₕ' }, { 'i', 'ᵢ' }, { 'j', 'ⱼ' }, { 'k', 'ₖ' }, { 'l', 'ₗ' }, { 'm', 'ₘ' }, { 'n', 'ₙ' }, { 'o', 'ₒ' },
            { 'p', 'ₚ' }, { 'r', 'ᵣ' }, { 's', 'ₛ' }, { 't', 'ₜ' }, { 'u', 'ᵤ' }, { 'v', 'ᵥ' }, { 'x', 'ₓ' }, { '+', '₊' }, { '-', '₋' }, { '=', '₌' },
            { '(', '₍' }, { ')', '₎' }, { 'γ', 'ᵧ' }
        };

        /// <summary>
        /// A map of superscript Unicode characters to their base character representations.
        /// </summary>
        public static readonly Dictionary<char, char> ReverseSupMap = SupMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        /// <summary>
        /// A map of subscript Unicode characters to their base character representations.
        /// </summary>
        public static readonly Dictionary<char, char> ReverseSubMap = SubMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        /// <summary>
        /// A map of characters to their math blackboard bold Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathbbMap = new()
        {
            {'R', 'ℝ'}, {'C', 'ℂ'}, {'N', 'ℕ'}, {'Q', 'ℚ'}, {'Z', 'ℤ'}
        };
        /// <summary>
        /// A map of characters to their math calligraphic Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathcalMap = new()
        {
            {'E', 'ℰ'}, {'F', 'ℱ'}, {'H', 'ℋ'}, {'B', 'ℬ'}, {'I', 'ℐ'},
            {'R', 'ℛ'}, {'L', 'ℒ'}, {'M', 'ℳ'}
        };

        /// <summary>
        /// A map of characters to their math Fraktur Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathfrakMap = new()
        {
            {'C', 'ℭ'}, {'H', 'ℌ'}, {'I', 'ℑ'}, {'R', 'ℜ'}, {'Z', 'ℨ'}
        };

        /// <summary>
        /// A map of characters to their math script Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathscrMap = new()
        {
            {'E', 'ℰ'}, {'F', 'ℱ'}, {'H', 'ℋ'}, {'I', 'ℐ'}, {'L', 'ℒ'},
            {'M', 'ℳ'}, {'R', 'ℛ'}
        };
    }
}


using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
    public class SymbolDefinition
    {
        public string? PlainText { get; set; }
        public string? ScreenReader { get; set; }
        //public string? ScreenReaderTemplate { get; set; }
        public string? ExceptionalScreenReader { get; set; }
        public string? HumanFriendly { get; set; }
        public string? HumanFriendlyKey { get; set; }
        //public string? HumanFriendlyTemplate { get; set; }
        public string? OpenAI { get; set; }
    }

    public record FontCharacter(char BaseChar, string FontCommand, string UnicodeChar);
    public record ScriptCharacter(char BaseChar, char? Superscript, char? Subscript);
    public record OperatorMapping(string openAiFriendly, string humanFriendly, string screenReaderFriendly, string screenReaderFriendlySuperscript);

    /// <summary>
    /// Provides centralized storage for symbol and character mappings used in LaTeX conversion.
    /// </summary>
    internal static class Dictionaries
    {

        private static readonly List<FontCharacter> FontLibrary = new()
        {
            // mathbb
            new('A', @"\mathbb", "𝔸"),
            new('B', @"\mathbb", "𝔹"),
            new('C', @"\mathbb", "ℂ"),
            new('D', @"\mathbb", "𝔻"),
            new('E', @"\mathbb", "𝔼"),
            new('F', @"\mathbb", "𝔽"),
            new('G', @"\mathbb", "𝔾"),
            new('H', @"\mathbb", "ℍ"),
            new('I', @"\mathbb", "𝕀"),
            new('J', @"\mathbb", "𝕁"),
            new('K', @"\mathbb", "𝕂"),
            new('L', @"\mathbb", "𝕃"),
            new('M', @"\mathbb", "𝕄"),
            new('N', @"\mathbb", "ℕ"),
            new('O', @"\mathbb", "𝕆"),
            new('P', @"\mathbb", "ℙ"),
            new('Q', @"\mathbb", "ℚ"),
            new('R', @"\mathbb", "ℝ"),
            new('S', @"\mathbb", "𝕊"),
            new('T', @"\mathbb", "𝕋"),
            new('U', @"\mathbb", "𝕌"),
            new('V', @"\mathbb", "𝕍"),
            new('W', @"\mathbb", "𝕎"),
            new('X', @"\mathbb", "𝕏"),
            new('Y', @"\mathbb", "𝕐"),
            new('Z', @"\mathbb", "ℤ"),
            new('a', @"\mathbb", "𝕒"),
            new('b', @"\mathbb", "𝕓"),
            new('c', @"\mathbb", "𝕔"),
            new('d', @"\mathbb", "𝕕"),
            new('e', @"\mathbb", "𝕖"),
            new('f', @"\mathbb", "𝕗"),
            new('g', @"\mathbb", "𝕘"),
            new('h', @"\mathbb", "𝕙"),
            new('i', @"\mathbb", "𝕚"),
            new('j', @"\mathbb", "𝕛"),
            new('k', @"\mathbb", "𝕜"),
            new('l', @"\mathbb", "𝕝"),
            new('m', @"\mathbb", "𝕞"),
            new('n', @"\mathbb", "𝕟"),
            new('o', @"\mathbb", "𝕠"),
            new('p', @"\mathbb", "𝕡"),
            new('q', @"\mathbb", "𝕢"),
            new('r', @"\mathbb", "𝕣"),
            new('s', @"\mathbb", "𝕤"),
            new('t', @"\mathbb", "𝕥"),
            new('u', @"\mathbb", "𝕦"),
            new('v', @"\mathbb", "𝕧"),
            new('w', @"\mathbb", "𝕨"),
            new('x', @"\mathbb", "𝕩"),
            new('y', @"\mathbb", "𝕪"),
            new('z', @"\mathbb", "𝕫"),

            // mathcal
            new('A', @"\mathcal", "𝒜"),
            new('B', @"\mathcal", "ℬ"),
            new('C', @"\mathcal", "𝒞"),
            new('D', @"\mathcal", "𝒟"),
            new('E', @"\mathcal", "ℰ"),
            new('F', @"\mathcal", "ℱ"),
            new('G', @"\mathcal", "𝒢"),
            new('H', @"\mathcal", "ℋ"),
            new('I', @"\mathcal", "ℐ"),
            new('J', @"\mathcal", "𝒥"),
            new('K', @"\mathcal", "𝒦"),
            new('L', @"\mathcal", "ℒ"),
            new('M', @"\mathcal", "ℳ"),
            new('N', @"\mathcal", "𝒩"),
            new('O', @"\mathcal", "𝒪"),
            new('P', @"\mathcal", "𝒫"),
            new('Q', @"\mathcal", "𝒬"),
            new('R', @"\mathcal", "ℛ"),
            new('S', @"\mathcal", "𝒮"),
            new('T', @"\mathcal", "𝒯"),
            new('U', @"\mathcal", "𝒰"),

            new('V', @"\mathcal", "𝒱"),
            new('W', @"\mathcal", "𝒲"),
            new('X', @"\mathcal", "𝒳"),
            new('Y', @"\mathcal", "𝒴"),
            new('Z', @"\mathcal", "𝒵"),

            // mathfrak
            new('A', @"\mathfrak", "𝔄"),
            new('B', @"\mathfrak", "𝔅"),
            new('C', @"\mathfrak", "ℭ"),
            new('D', @"\mathfrak", "𝔇"),
            new('E', @"\mathfrak", "𝔈"),
            new('F', @"\mathfrak", "𝔉"),

            new('G', @"\mathfrak", "𝔊"),
            new('H', @"\mathfrak", "ℌ"),
            new('I', @"\mathfrak", "ℑ"),
            new('J', @"\mathfrak", "𝔍"),
            new('K', @"\mathfrak", "𝔎"),

            new('L', @"\mathfrak", "𝔏"),
            new('M', @"\mathfrak", "𝔐"),
            new('N', @"\mathfrak", "𝔑"),
            new('O', @"\mathfrak", "𝔒"),
            new('P', @"\mathfrak", "𝔓"),

            new('Q', @"\mathfrak", "𝔔"),
            new('R', @"\mathfrak", "ℜ"),
            new('S', @"\mathfrak", "𝔖"),
            new('T', @"\mathfrak", "𝔗"),
            new('U', @"\mathfrak", "𝔘"),

            new('V', @"\mathfrak", "𝔙"),
            new('W', @"\mathfrak", "𝔚"),
            new('X', @"\mathfrak", "𝔛"),
            new('Y', @"\mathfrak", "𝔜"),
            new('Z', @"\mathfrak", "ℨ"),
            new('a', @"\mathfrak", "𝔞"),
            new('b', @"\mathfrak", "𝔟"),
            new('c', @"\mathfrak", "𝔠"),
            new('d', @"\mathfrak", "𝔡"),
            new('e', @"\mathfrak", "𝔢"),
            new('f', @"\mathfrak", "𝔣"),

            new('g', @"\mathfrak", "𝔤"),
            new('h', @"\mathfrak", "𝔥"),
            new('i', @"\mathfrak", "𝔦"),
            new('j', @"\mathfrak", "𝔧"),
            new('k', @"\mathfrak", "𝔨"),

            new('l', @"\mathfrak", "𝔩"),
            new('m', @"\mathfrak", "𝔪"),
            new('n', @"\mathfrak", "𝔫"),
            new('o', @"\mathfrak", "𝔬"),
            new('p', @"\mathfrak", "𝔭"),

            new('q', @"\mathfrak", "𝔮"),
            new('r', @"\mathfrak", "𝔯"),
            new('s', @"\mathfrak", "𝔰"),
            new('t', @"\mathfrak", "𝔱"),
            new('u', @"\mathfrak", "𝔲"),

            new('v', @"\mathfrak", "𝔳"),
            new('w', @"\mathfrak", "𝔴"),
            new('x', @"\mathfrak", "𝔵"),
            new('y', @"\mathfrak", "𝔶"),
            new('z', @"\mathfrak", "𝔷"),

            // mathscr
            new('A', @"\mathscr", "𝒜"),
            new('B', @"\mathscr", "ℬ"),
            new('C', @"\mathscr", "𝒞"),
            new('D', @"\mathscr", "𝒟"),
            new('E', @"\mathscr", "ℰ"),
            new('F', @"\mathscr", "ℱ"),
            new('G', @"\mathscr", "𝒢"),
            new('H', @"\mathscr", "ℋ"),
            new('I', @"\mathscr", "ℐ"),
            new('J', @"\mathscr", "𝒥"),
            new('K', @"\mathscr", "𝒦"),
            new('L', @"\mathscr", "ℒ"),
            new('M', @"\mathscr", "ℳ"),
            new('N', @"\mathscr", "𝒩"),
            new('O', @"\mathscr", "𝒪"),
            new('P', @"\mathscr", "𝒫"),
            new('Q', @"\mathscr", "𝒬"),
            new('R', @"\mathscr", "ℛ"),
            new('S', @"\mathscr", "𝒮"),
            new('T', @"\mathscr", "𝒯"),
            new('U', @"\mathscr", "𝒰"),
            new('V', @"\mathscr", "𝒱"),
            new('W', @"\mathscr", "𝒲"),
            new('X', @"\mathscr", "𝒳"),
            new('Y', @"\mathscr", "𝒴"),
            new('Z', @"\mathscr", "𝒵")
        };
        private static readonly List<ScriptCharacter> ScriptLibrary = new()
        {
            new('0', '⁰', '₀'), new('1', '¹', '₁'), new('2', '²', '₂'), new('3', '³', '₃'), new('4', '⁴', '₄'),
            new('5', '⁵', '₅'), new('6', '⁶', '₆'), new('7', '⁷', '₇'), new('8', '⁸', '₈'), new('9', '⁹', '₉'),
            new('a', 'ᵃ', 'ₐ'), new('b', 'ᵇ', null), new('c', 'ᶜ', null), new('d', 'ᵈ', null), new('e', 'ᵉ', 'ₑ'),
            new('f', 'ᶠ', null), new('g', 'ᵍ', null), new('h', 'ʰ', 'ₕ'), new('i', 'ⁱ', 'ᵢ'), new('j', 'ʲ', 'ⱼ'),
            new('k', 'ᵏ', 'ₖ'), new('l', 'ˡ', 'ₗ'), new('m', 'ᵐ', 'ₘ'), new('n', 'ⁿ', 'ₙ'), new('o', 'ᵒ', 'ₒ'),
            new('p', 'ᵖ', 'ₚ'), new('r', 'ʳ', 'ᵣ'), new('s', 'ˢ', 'ₛ'), new('t', 'ᵗ', 'ₜ'), new('u', 'ᵘ', 'ᵤ'),
            new('v', 'ᵛ', 'ᵥ'), new('w', 'ʷ', null), new('x', 'ˣ', 'ₓ'), new('y', 'ʸ', null), new('z', 'ᶻ', null),
            new('+', '⁺', '₊'), new('-', '⁻', '₋'), new('=', '⁼', '₌'), new('(', '⁽', '₍'), new(')', '⁾', '₎'),
            new('A', 'ᴬ', null), new('B', 'ᴮ', null), new('D', 'ᴰ', null), new('E', 'ᴱ', null), new('G', 'ᴳ', null),
            new('H', 'ᴴ', null), new('I', 'ᴵ', null), new('J', 'ᴶ', null), new('K', 'ᴷ', null), new('L', 'ᴸ', null),
            new('M', 'ᴹ', null), new('N', 'ᴺ', null), new('O', 'ᴼ', null), new('P', 'ᴾ', null), new('R', 'ᴿ', null),
            new('T', 'ᵀ', null), new('U', 'ᵁ', null), new('V', 'ⱽ', null), new('W', 'ᵂ', null), new('γ', null, 'ᵧ')
        };
        private static readonly Dictionary<string, SymbolDefinition> SymbolLibrary = new()
        {
            { @"\alpha", new SymbolDefinition { PlainText = "alpha", ScreenReader = "alpha", HumanFriendly = "α" } },
            { @"\beta", new SymbolDefinition { PlainText = "beta", ScreenReader = "beta", HumanFriendly = "β" } },
            { @"\gamma", new SymbolDefinition { PlainText = "gamma", ScreenReader = "gamma", HumanFriendly = "γ" } },
            { @"\delta", new SymbolDefinition { PlainText = "delta", ScreenReader = "delta", HumanFriendly = "δ" } },
            { @"\epsilon", new SymbolDefinition { PlainText = "epsilon", ScreenReader = "epsilon", HumanFriendly = "ε" } },
            { @"\varepsilon", new SymbolDefinition { PlainText = "varepsilon", ScreenReader = "varepsilon", HumanFriendly = "ε" } },
            { @"\zeta", new SymbolDefinition { PlainText = "zeta", ScreenReader = "zeta", HumanFriendly = "ζ" } },
            { @"\eta", new SymbolDefinition { PlainText = "eta", ScreenReader = "eta", HumanFriendly = "η" } },
            { @"\theta", new SymbolDefinition { PlainText = "theta", ScreenReader = "theta", HumanFriendly = "θ" } },
            { @"\iota", new SymbolDefinition { PlainText = "iota", ScreenReader = "iota", HumanFriendly = "ι" } },
            { @"\kappa", new SymbolDefinition { PlainText = "kappa", ScreenReader = "kappa", HumanFriendly = "κ" } },
            { @"\varkappa", new SymbolDefinition { PlainText = "varkappa", ScreenReader = "varkappa", HumanFriendly = "ϰ" } },
            { @"\lambda", new SymbolDefinition { PlainText = "lambda", ScreenReader = "lambda", HumanFriendly = "λ" } },
            { @"\mu", new SymbolDefinition { PlainText = "mu", ScreenReader = "mu", HumanFriendly = "μ" } },
            { @"\nu", new SymbolDefinition { PlainText = "nu", ScreenReader = "nu", HumanFriendly = "ν" } },
            { @"\xi", new SymbolDefinition { PlainText = "xi", ScreenReader = "xi", HumanFriendly = "ξ" } },
            { @"\omicron", new SymbolDefinition { PlainText = "omicron", ScreenReader = "omicron", HumanFriendly = "ο" } },
            { @"\pi", new SymbolDefinition { PlainText = "pi", ScreenReader = "pi", HumanFriendly = "π" } },
            { @"\varpi", new SymbolDefinition { PlainText = "varpi", ScreenReader = "varpi", HumanFriendly = "ϖ" } },
            { @"\rho", new SymbolDefinition { PlainText = "rho", ScreenReader = "rho", HumanFriendly = "ρ" } },
            { @"\varrho", new SymbolDefinition { PlainText = "varrho", ScreenReader = "varrho", HumanFriendly = "ϱ" } },
            { @"\sigma", new SymbolDefinition { PlainText = "sigma", ScreenReader = "sigma", HumanFriendly = "σ" } },
            { @"\varsigma", new SymbolDefinition { PlainText = "varsigma", ScreenReader = "varsigma", HumanFriendly = "ς" } },
            { @"\tau", new SymbolDefinition { PlainText = "tau", ScreenReader = "tau", HumanFriendly = "τ" } },
            { @"\upsilon", new SymbolDefinition { PlainText = "upsilon", ScreenReader = "upsilon", HumanFriendly = "υ" } },
            { @"\phi", new SymbolDefinition { PlainText = "phi", ScreenReader = "phi", HumanFriendly = "φ" } },
            { @"\varphi", new SymbolDefinition { PlainText = "varphi", ScreenReader = "varphi", HumanFriendly = "\u03D5" } },
            { @"\chi", new SymbolDefinition { PlainText = "chi", ScreenReader = "chi", HumanFriendly = "χ" } },
            { @"\psi", new SymbolDefinition { PlainText = "psi", ScreenReader = "psi", HumanFriendly = "ψ" } },
            { @"\omega", new SymbolDefinition { PlainText = "omega", ScreenReader = "omega", HumanFriendly = "ω" } },
            { @"\Gamma", new SymbolDefinition { PlainText = "Gamma", ScreenReader = "Gamma", HumanFriendly = "Γ" } },
            { @"\Delta", new SymbolDefinition { PlainText = "Delta", ScreenReader = "Delta", HumanFriendly = "Δ" } },
            { @"\Theta", new SymbolDefinition { PlainText = "Theta", ScreenReader = "Theta", HumanFriendly = "Θ" } },
            { @"\Lambda", new SymbolDefinition { PlainText = "Lambda", ScreenReader = "Lambda", HumanFriendly = "Λ" } },
            { @"\Xi", new SymbolDefinition { PlainText = "Xi", ScreenReader = "Xi", HumanFriendly = "Ξ" } },
            { @"\Pi", new SymbolDefinition { PlainText = "Pi", ScreenReader = "Pi", HumanFriendly = "Π" } },
            { @"\Sigma", new SymbolDefinition { PlainText = "Sigma", ScreenReader = "Sigma", HumanFriendly = "Σ" } },
            { @"\Upsilon", new SymbolDefinition { PlainText = "Upsilon", ScreenReader = "Upsilon", HumanFriendly = "Υ" } },
            { @"\Phi", new SymbolDefinition { PlainText = "Phi", ScreenReader = "Phi", HumanFriendly = "Φ" } },
            { @"\Psi", new SymbolDefinition { PlainText = "Psi", ScreenReader = "Psi", HumanFriendly = "Ψ" } },
            { @"\Omega", new SymbolDefinition { PlainText = "Omega", ScreenReader = "Omega", HumanFriendly = "Ω" } },
            { @"\times", new SymbolDefinition { PlainText = "times", ScreenReader = "times", HumanFriendly = "×" } },
            { @"\div", new SymbolDefinition { PlainText = "div", ScreenReader = "divided by", HumanFriendly = "÷" } },
            { @"\pm", new SymbolDefinition { PlainText = "pm", ScreenReader = "plus-minus", HumanFriendly = "±" } },
            { @"\mp", new SymbolDefinition { PlainText = "mp", ScreenReader = "minus-plus", HumanFriendly = "∓" } },
            { @"\cdot", new SymbolDefinition { PlainText = "cdot", ScreenReader = "cdot", HumanFriendly = "·" } },
            { @"\circ", new SymbolDefinition { PlainText = "circ", ScreenReader = "{0} degrees", HumanFriendly = "{0}°" , HumanFriendlyKey = "°" } },
            { @"\bullet", new SymbolDefinition { PlainText = "bullet", ScreenReader = "bullet", HumanFriendly = "•" } },
            { @"\oplus", new SymbolDefinition { PlainText = "oplus", ScreenReader = "oplus", HumanFriendly = "⊕" } },
            { @"\ominus", new SymbolDefinition { PlainText = "ominus", ScreenReader = "ominus", HumanFriendly = "⊖" } },
            { @"\otimes", new SymbolDefinition { PlainText = "otimes", ScreenReader = "tensor product", HumanFriendly = "⊗" } },
            { @"\oslash", new SymbolDefinition { PlainText = "oslash", ScreenReader = "oslash", HumanFriendly = "⊘" } },
            { @"\odot", new SymbolDefinition { PlainText = "odot", ScreenReader = "circled dot", HumanFriendly = "⊙" } },
            { @"\parallel", new SymbolDefinition { PlainText = "parallel", ScreenReader = "parallel to", HumanFriendly = "∥" } },
            { @"\perp", new SymbolDefinition { PlainText = "perp", ScreenReader = "perpendicular to", HumanFriendly = "⊥" } },
            { @"\implies", new SymbolDefinition { PlainText = "implies", ScreenReader = "implies", HumanFriendly = "⇒" } },
            { @"\iff", new SymbolDefinition { PlainText = "iff", ScreenReader = "if and only if", HumanFriendly = "⇔" } },
            { @"\rightarrow", new SymbolDefinition { PlainText = "rightarrow", ScreenReader = "right arrow", ExceptionalScreenReader = "approaches", HumanFriendly = "→" } },
            { @"\leftarrow", new SymbolDefinition { PlainText = "leftarrow", ScreenReader = "left arrow", HumanFriendly = "←" } },
            { @"\uparrow", new SymbolDefinition { PlainText = "uparrow", ScreenReader = "up arrow", HumanFriendly = "↑" } },
            { @"\downarrow", new SymbolDefinition { PlainText = "downarrow", ScreenReader = "down arrow", HumanFriendly = "↓" } },
            { @"\Rightarrow", new SymbolDefinition { PlainText = "Rightarrow", ScreenReader = "right double arrow", HumanFriendly = "⇒" } },
            { @"\Leftarrow", new SymbolDefinition { PlainText = "Leftarrow", ScreenReader = "left double arrow", HumanFriendly = "⇐" } },
            { @"\leftrightarrow", new SymbolDefinition { PlainText = "leftrightarrow", ScreenReader = "left right arrow", HumanFriendly = "↔" } },
            { @"\Leftrightarrow", new SymbolDefinition { PlainText = "Leftrightarrow", ScreenReader = "if and only if", HumanFriendly = "⇔" } },
            { @"\leq", new SymbolDefinition { PlainText = "leq", ScreenReader = "less than or equal to", HumanFriendly = "≤" } },
            { @"\geq", new SymbolDefinition { PlainText = "geq", ScreenReader = "greater than or equal to", HumanFriendly = "≥" } },
            { @"\neq", new SymbolDefinition { PlainText = "neq", ScreenReader = "not equal to", HumanFriendly = "≠" } },
            { @"\approx", new SymbolDefinition { PlainText = "approx", ScreenReader = "approximately equal to", HumanFriendly = "≈" } },
            { @"\equiv", new SymbolDefinition { PlainText = "equiv", ScreenReader = "congruent to", HumanFriendly = "≡" } },
            { @"\propto", new SymbolDefinition { PlainText = "propto", ScreenReader = "proportional to", HumanFriendly = "∝" } },
            { @"\infty", new SymbolDefinition { PlainText = "infty", ScreenReader = "infinity", HumanFriendly = "∞" } },
            { @"\nabla", new SymbolDefinition { PlainText = "nabla", ScreenReader = "nabla", HumanFriendly = "∇" } },
            { @"\partial", new SymbolDefinition { PlainText = "partial", ScreenReader = "partial derivative", HumanFriendly = "∂" } },
            { @"\int", new SymbolDefinition { PlainText = "integral", ScreenReader = "integral from {0} to {1}", HumanFriendly = "∫{0}{1}" , HumanFriendlyKey = "∫"} },
            { @"\sum", new SymbolDefinition { PlainText = "summation", ScreenReader = "summation from {0} to {1}", HumanFriendly = "∑{0}{1}" , HumanFriendlyKey = "∑"} },
            { @"\prod", new SymbolDefinition { PlainText = "product", ScreenReader = "prod from {0} to {1}", HumanFriendly = "∏{0}{1}" , HumanFriendlyKey = "∏"} },
            { @"\lim", new SymbolDefinition { PlainText = "limit", ScreenReader = "limit as {0} of", HumanFriendly = "lim_{{{0}}}" , HumanFriendlyKey = "lim"} },
            { @"\sqrt", new SymbolDefinition { PlainText = "sqrt", ScreenReader = "the square root of {0}", HumanFriendly ="√({0})", OpenAI ="sqrt({0})" } },
            { @"\frac", new SymbolDefinition { PlainText = "fraction with numerator", OpenAI = "{0}/{1}" , HumanFriendly  = "{0} / {1}", ScreenReader = "fraction with numerator {0} and denominator {1}"  } },
            { @"\binom", new SymbolDefinition { PlainText = "binom", OpenAI = "binom({0},{1})" , HumanFriendly = "({0} {1})" , ScreenReader = "{0} choose {1}" } },
            { @"\hbar", new SymbolDefinition { PlainText = "hbar", ScreenReader = "h bar", HumanFriendly = "ħ" } },
            { @"\ell", new SymbolDefinition { PlainText = "ell", ScreenReader = "ell", HumanFriendly = "ℓ" } },
            { @"\wp", new SymbolDefinition { PlainText = "wp", ScreenReader = "Weierstrass p", HumanFriendly = "wp" } },
            { @"\Re", new SymbolDefinition { PlainText = "Re", ScreenReader = "Real part", HumanFriendly = "ℜ" } },
            { @"\Im", new SymbolDefinition { PlainText = "Im", ScreenReader = "Imaginary part", HumanFriendly = "ℑ" } },
            { @"\forall", new SymbolDefinition { PlainText = "forall", ScreenReader = "for all", HumanFriendly = "∀" } },
            { @"\exists", new SymbolDefinition { PlainText = "exists", ScreenReader = "there exists", HumanFriendly = "∃" } },
            { @"\in", new SymbolDefinition { PlainText = "in", ScreenReader = "in", HumanFriendly = "∈" } },
            { @"\to", new SymbolDefinition { PlainText = "->", ScreenReader = "approaches", HumanFriendly = "→" } },
            { @"\vec", new SymbolDefinition { PlainText = "vector", ScreenReader = "vector {0}", HumanFriendly = "{0}⃗", OpenAI = "{0}" } },
            { @"\hat", new SymbolDefinition { PlainText = "hat", ScreenReader = "{0} hat", HumanFriendly = "{0}̂", OpenAI = "hat {0}" } },
            { @"\overline", new SymbolDefinition { PlainText = "bar", ScreenReader = "{0} bar", HumanFriendly = "{0}̅", OpenAI = "overline({0})" } },
            { @"\sin", new SymbolDefinition { PlainText = "sine of", ScreenReader ="sine of {0}",  HumanFriendly ="sin({0})" ,  OpenAI ="sin({0})" } },
            { @"\cos", new SymbolDefinition { PlainText = "cosine of", ScreenReader ="cosine of {0}", HumanFriendly ="cos({0})" ,  OpenAI ="cos({0})"} },
            { @"\tan", new SymbolDefinition { PlainText = "tangent of", ScreenReader ="tangent of {0}", HumanFriendly ="tan({0})" ,  OpenAI ="tan({0})"} },
            { @"\log", new SymbolDefinition { PlainText = "logarithm of", ScreenReader ="logarithm of {0}",HumanFriendly ="log({0})" ,   OpenAI ="log({0})"} },
            { @"\ln", new SymbolDefinition { PlainText = "natural logarithm of", ScreenReader ="natural logarithm of {0}", HumanFriendly ="ln({0})" ,  OpenAI ="ln({0})"} },
            { @"\exp", new SymbolDefinition { PlainText = "e to the power of", ScreenReader ="e to the power of {0}", HumanFriendly ="e{0}" ,  OpenAI ="exp({0})" } },
            { @"\det", new SymbolDefinition { PlainText = "determinant of", ScreenReader ="determinant of {0}", HumanFriendly ="det({0})" ,  OpenAI ="det({0})" } },
            { @"\arcsin", new SymbolDefinition { PlainText = "arcsin", ScreenReader = "arcsin", HumanFriendly = "sin⁻¹" } },
            { @"\arccos", new SymbolDefinition { PlainText = "arccos", ScreenReader = "arccos", HumanFriendly = "cos⁻¹" } },
            { @"\arctan", new SymbolDefinition { PlainText = "arctan", ScreenReader = "arctan", HumanFriendly = "tan⁻¹" } },
            { @"\cup", new SymbolDefinition { PlainText = "cup", ScreenReader = "union", HumanFriendly = "∪" } },
            { @"\cap", new SymbolDefinition { PlainText = "cap", ScreenReader = "intersection", HumanFriendly = "∩" } },
            { @"\subset", new SymbolDefinition { PlainText = "subset", ScreenReader = "subset of", HumanFriendly = "⊂" } },
            { @"\supset", new SymbolDefinition { PlainText = "supset", ScreenReader = "superset of", HumanFriendly = "⊃" } },
            { @"\neg", new SymbolDefinition { PlainText = "neg", ScreenReader = "not", HumanFriendly = "¬" } },
            { @"\land", new SymbolDefinition { PlainText = "land", ScreenReader = "and", HumanFriendly = "∧" } },
            { @"\lor", new SymbolDefinition { PlainText = "lor", ScreenReader = "or", HumanFriendly = "∨" } },
            { @"\prime", new SymbolDefinition { PlainText = "prime", ScreenReader = "{0} prime", HumanFriendly = "{0}′", HumanFriendlyKey="′" } },
            { @"\blacksquare", new SymbolDefinition { PlainText = "blacksquare", ScreenReader = "black square", HumanFriendly = "■" } },
            { @"\heartsuit", new SymbolDefinition { PlainText = "heartsuit", ScreenReader = "heart suit", HumanFriendly = "♥" } },
            { @"\clubsuit", new SymbolDefinition { PlainText = "clubsuit", ScreenReader = "club suit", HumanFriendly = "♣" } },
            { @"\diamondsuit", new SymbolDefinition { PlainText = "diamondsuit", ScreenReader = "diamond suit", HumanFriendly = "♦" } },
            { @"\spadesuit", new SymbolDefinition { PlainText = "spadesuit", ScreenReader = "spade suit", HumanFriendly = "♠" } },
            { @"\natural", new SymbolDefinition { PlainText = "natural", ScreenReader = "natural", HumanFriendly = "♮" } },
            { @"\sharp", new SymbolDefinition { PlainText = "sharp", ScreenReader = "sharp", HumanFriendly = "♯" } },
            { @"\flat", new SymbolDefinition { PlainText = "flat", ScreenReader = "flat", HumanFriendly = "♭" } },
            { @"\triangle", new SymbolDefinition { PlainText = "triangle", ScreenReader = "triangle", HumanFriendly = "△" } },
            { @"\triangledown", new SymbolDefinition { PlainText = "triangledown", ScreenReader = "downward triangle", HumanFriendly = "▽" } },
            { @"\angle", new SymbolDefinition { PlainText = "angle", ScreenReader = "angle", HumanFriendly = "∠" } },
            { @"\measuredangle", new SymbolDefinition { PlainText = "measuredangle", ScreenReader = "measured angle", HumanFriendly = "∡" } },
            { @"\dots", new SymbolDefinition { PlainText = "...", ScreenReader = "dots", HumanFriendly = "…" } },
            { @"\cdots", new SymbolDefinition { PlainText = "...", ScreenReader = "centered dots", HumanFriendly = "⋯" } },
            { @"\vdots", new SymbolDefinition { PlainText = "...", ScreenReader = "vertical dots", HumanFriendly = "⋮" } },
            { @"\ddots", new SymbolDefinition { PlainText = "...", ScreenReader = "diagonal dots", HumanFriendly = "⋱" } },
            { @"\,", new SymbolDefinition { PlainText = " ", ScreenReader = " ", HumanFriendly = " " } },


            { @"\mathcal", new SymbolDefinition { PlainText = "mathcal", ScreenReader = "{0}}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathbb", new SymbolDefinition { PlainText = "mathbb", ScreenReader = "the set of real numbers", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathfrak", new SymbolDefinition { PlainText = "mathfrak", ScreenReader = "frak {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathscr", new SymbolDefinition { PlainText = "mathscr", ScreenReader = "scr {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },

            { @"\text", new SymbolDefinition { PlainText = "text", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\textrm", new SymbolDefinition { PlainText = "textrm", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathbf", new SymbolDefinition { PlainText = "mathbf", ScreenReader = "bf {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathit", new SymbolDefinition { PlainText = "mathit", ScreenReader = "it {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathsf", new SymbolDefinition { PlainText = "mathsf", ScreenReader = "sf {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathrm", new SymbolDefinition { PlainText = "mathrm", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { @"\mathtt", new SymbolDefinition { PlainText = "mathtt", ScreenReader = "tt {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },

            { @"\for_superscript", new SymbolDefinition { PlainText = "for_superscript", ScreenReader = "{0} to the power of {1}", HumanFriendly = "{0}^{1}}" , OpenAI = "{0}" } },
            { @"\for_subscript", new SymbolDefinition { PlainText = "for_subscript", ScreenReader = "{0} subscript {1}", HumanFriendly = "{0}_{1}" , OpenAI = "{0}" } },
            { @"\for_matrix", new SymbolDefinition { PlainText = "for_matrix", ScreenReader = "a {0}x{1} matrix with rows {2}", HumanFriendly = "{0}_{1}" , OpenAI = "{0}" } },

           };

        static Dictionaries()
        {
            SymbolMap = SymbolLibrary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.PlainText!);
            ScreenReaderSymbolMap = SymbolLibrary
                .Where(kvp => kvp.Value.ScreenReader != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReader!);
            ExceptionalScreenReaderSymbolMap = SymbolLibrary
                .Where(kvp => kvp.Value.ExceptionalScreenReader != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ExceptionalScreenReader!);
            HumanFriendlySymbolMap = SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendly!);
            //ReverseHumanFriendlySymbolMap = HumanFriendlySymbolMap.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.First().Key.Substring(1));


            ReverseHumanFriendlySymbolMap = SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null)
                .GroupBy(kvp => (kvp.Value.HumanFriendlyKey ?? kvp.Value.HumanFriendly)!)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.First().Key.Substring(1));

            ScreenReaderTemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.ScreenReader != null && kvp.Value.ScreenReader.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReader!);

            HumanFriendlyTemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null && kvp.Value.HumanFriendly.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendly!);

            OpenAITemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.OpenAI != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OpenAI!);

            ScreenReaderOperatorMap = OperatorMap
                .Where(kvp => kvp.Value.screenReaderFriendly != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.screenReaderFriendly!);

            ScreenReaderOperatorSuperscriptTemplateMap = OperatorMap
                .Where(kvp => kvp.Value.screenReaderFriendlySuperscript != null && kvp.Value.screenReaderFriendlySuperscript.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.screenReaderFriendlySuperscript!);

            MathbbMap = FontLibrary
                .Where(fc => fc.FontCommand == @"\mathbb")
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathcalMap = FontLibrary
                .Where(fc => fc.FontCommand == @"\mathcal")
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathfrakMap = FontLibrary
                .Where(fc => fc.FontCommand == @"\mathfrak")
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathscrMap = FontLibrary
                .Where(fc => fc.FontCommand == @"\mathscr")
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            ReverseMathFontMap = FontLibrary
                .GroupBy(fc => fc.UnicodeChar)
                .ToDictionary(g => g.Key, g => $"{g.First().FontCommand}{{{g.First().BaseChar}}}");

            SupMap = ScriptLibrary
                .Where(sc => sc.Superscript.HasValue)
                .ToDictionary(sc => sc.BaseChar, sc => sc.Superscript!.Value);

            SubMap = ScriptLibrary
                .Where(sc => sc.Subscript.HasValue)
                .ToDictionary(sc => sc.BaseChar, sc => sc.Subscript!.Value);

            ReverseSupMap = ScriptLibrary
                .Where(sc => sc.Superscript.HasValue)
                .ToDictionary(sc => sc.Superscript!.Value, sc => sc.BaseChar);

            ReverseSubMap = ScriptLibrary
                .Where(sc => sc.Subscript.HasValue)
                .ToDictionary(sc => sc.Subscript!.Value, sc => sc.BaseChar);
        }


        /// <summary>
        /// A comprehensive map of operators to their plain text representations.
        /// </summary>
        public static readonly Dictionary<string, OperatorMapping> OperatorMap = new()
        {
            { "+", new OperatorMapping("+", "+", "plus","{0} plus") },
            { "-", new OperatorMapping("-", "-", "minus","{0} minus") },
            { "=", new OperatorMapping("=", "=", "equals","{0} equals") },
            { "*", new OperatorMapping("*", "*", "times","{0} star") },
            { "/", new OperatorMapping("/", "/", "divided by","{0} divided by") },
            { "′", new OperatorMapping("′", "′", "prime","{0} prime") },
            { "2", new OperatorMapping("2", "2", "squared","{0} squared") },
            { "3", new OperatorMapping("3", "3", "cubed","{0} cubed") },
            { "°", new OperatorMapping("°", "°", "degrees","{0} degrees") },
        };

        /// <summary>
        /// A comprehensive map of LaTeX commands to their plain text representations.
        /// </summary>
        public static readonly Dictionary<string, string> SymbolMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly representations.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderSymbolMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly representations.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderOperatorMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly template strings.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderOperatorSuperscriptTemplateMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly representations.
        /// </summary>
        public static readonly Dictionary<string, string> ExceptionalScreenReaderSymbolMap;

        /// <summary>
        /// A map of LaTeX commands to their human-friendly Unicode representations.
        /// </summary>
        public static readonly Dictionary<string, string> HumanFriendlySymbolMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly template strings.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderTemplateMap;

        /// <summary>
        /// A map of LaTeX commands to their human-friendly template strings.
        /// </summary>
        public static readonly Dictionary<string, string> HumanFriendlyTemplateMap;

        /// <summary>
        /// A map of LaTeX commands to their OpenAI-friendly template strings.
        /// </summary>
        public static readonly Dictionary<string, string> OpenAITemplateMap;

        /// <summary>
        /// A map of Unicode symbols to their plain text representations.
        /// </summary>
        public static readonly Dictionary<string, string> ReverseHumanFriendlySymbolMap;
        /// <summary>
        /// A list of LaTeX commands that should not be converted from plain text without a leading backslash.
        /// </summary>
        public static readonly List<string> DeniedConvertWithoutSlash = new List<string>() { @"\bullet", @"\in", @"\times", @"\sum", @"\exists", @"\to", @"\angle", @"\triangle", @"\natural", @"\sharp", @"\parallel", @"\prod", @"\flat", @"\lim" };

        /// <summary>
        /// A map of characters to their superscript Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> SupMap;
        /// <summary>
        /// A map of characters to their subscript Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> SubMap;

        /// <summary>
        /// A map of superscript Unicode characters to their base character representations.
        /// </summary>
        public static readonly Dictionary<char, char> ReverseSupMap;

        /// <summary>
        /// A map of subscript Unicode characters to their base character representations.
        /// </summary>
        public static readonly Dictionary<char, char> ReverseSubMap;
        /// <summary>
        /// A map of characters to their math blackboard bold Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathbbMap;
        /// <summary>
        /// A map of characters to their math calligraphic Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathcalMap;

        /// <summary>
        /// A map of characters to their math Fraktur Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathfrakMap;

        /// <summary>
        /// A map of characters to their math script Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathscrMap;

        public static readonly Dictionary<string, string> ReverseMathFontMap;
    }
}

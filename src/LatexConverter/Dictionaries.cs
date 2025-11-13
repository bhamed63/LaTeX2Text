
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
            new('A', CommandNames.Mathbb, "𝔸"),
            new('B', CommandNames.Mathbb, "𝔹"),
            new('C', CommandNames.Mathbb, "ℂ"),
            new('D', CommandNames.Mathbb, "𝔻"),
            new('E', CommandNames.Mathbb, "𝔼"),
            new('F', CommandNames.Mathbb, "𝔽"),
            new('G', CommandNames.Mathbb, "𝔾"),
            new('H', CommandNames.Mathbb, "ℍ"),
            new('I', CommandNames.Mathbb, "𝕀"),
            new('J', CommandNames.Mathbb, "𝕁"),
            new('K', CommandNames.Mathbb, "𝕂"),
            new('L', CommandNames.Mathbb, "𝕃"),
            new('M', CommandNames.Mathbb, "𝕄"),
            new('N', CommandNames.Mathbb, "ℕ"),
            new('O', CommandNames.Mathbb, "𝕆"),
            new('P', CommandNames.Mathbb, "ℙ"),
            new('Q', CommandNames.Mathbb, "ℚ"),
            new('R', CommandNames.Mathbb, "ℝ"),
            new('S', CommandNames.Mathbb, "𝕊"),
            new('T', CommandNames.Mathbb, "𝕋"),
            new('U', CommandNames.Mathbb, "𝕌"),
            new('V', CommandNames.Mathbb, "𝕍"),
            new('W', CommandNames.Mathbb, "𝕎"),
            new('X', CommandNames.Mathbb, "𝕏"),
            new('Y', CommandNames.Mathbb, "𝕐"),
            new('Z', CommandNames.Mathbb, "ℤ"),
            new('a', CommandNames.Mathbb, "𝕒"),
            new('b', CommandNames.Mathbb, "𝕓"),
            new('c', CommandNames.Mathbb, "𝕔"),
            new('d', CommandNames.Mathbb, "𝕕"),
            new('e', CommandNames.Mathbb, "𝕖"),
            new('f', CommandNames.Mathbb, "𝕗"),
            new('g', CommandNames.Mathbb, "𝕘"),
            new('h', CommandNames.Mathbb, "𝕙"),
            new('i', CommandNames.Mathbb, "𝕚"),
            new('j', CommandNames.Mathbb, "𝕛"),
            new('k', CommandNames.Mathbb, "𝕜"),
            new('l', CommandNames.Mathbb, "𝕝"),
            new('m', CommandNames.Mathbb, "𝕞"),
            new('n', CommandNames.Mathbb, "𝕟"),
            new('o', CommandNames.Mathbb, "𝕠"),
            new('p', CommandNames.Mathbb, "𝕡"),
            new('q', CommandNames.Mathbb, "𝕢"),
            new('r', CommandNames.Mathbb, "𝕣"),
            new('s', CommandNames.Mathbb, "𝕤"),
            new('t', CommandNames.Mathbb, "𝕥"),
            new('u', CommandNames.Mathbb, "𝕦"),
            new('v', CommandNames.Mathbb, "𝕧"),
            new('w', CommandNames.Mathbb, "𝕨"),
            new('x', CommandNames.Mathbb, "𝕩"),
            new('y', CommandNames.Mathbb, "𝕪"),
            new('z', CommandNames.Mathbb, "𝕫"),

            // mathcal
            new('A', CommandNames.Mathcal, "𝒜"),
            new('B', CommandNames.Mathcal, "ℬ"),
            new('C', CommandNames.Mathcal, "𝒞"),
            new('D', CommandNames.Mathcal, "𝒟"),
            new('E', CommandNames.Mathcal, "ℰ"),
            new('F', CommandNames.Mathcal, "ℱ"),
            new('G', CommandNames.Mathcal, "𝒢"),
            new('H', CommandNames.Mathcal, "ℋ"),
            new('I', CommandNames.Mathcal, "ℐ"),
            new('J', CommandNames.Mathcal, "𝒥"),
            new('K', CommandNames.Mathcal, "𝒦"),
            new('L', CommandNames.Mathcal, "ℒ"),
            new('M', CommandNames.Mathcal, "ℳ"),
            new('N', CommandNames.Mathcal, "𝒩"),
            new('O', CommandNames.Mathcal, "𝒪"),
            new('P', CommandNames.Mathcal, "𝒫"),
            new('Q', CommandNames.Mathcal, "𝒬"),
            new('R', CommandNames.Mathcal, "ℛ"),
            new('S', CommandNames.Mathcal, "𝒮"),
            new('T', CommandNames.Mathcal, "𝒯"),
            new('U', CommandNames.Mathcal, "𝒰"),

            new('V', CommandNames.Mathcal, "𝒱"),
            new('W', CommandNames.Mathcal, "𝒲"),
            new('X', CommandNames.Mathcal, "𝒳"),
            new('Y', CommandNames.Mathcal, "𝒴"),
            new('Z', CommandNames.Mathcal, "𝒵"),

            // mathfrak
            new('A', CommandNames.Mathfrak, "𝔄"),
            new('B', CommandNames.Mathfrak, "𝔅"),
            new('C', CommandNames.Mathfrak, "ℭ"),
            new('D', CommandNames.Mathfrak, "𝔇"),
            new('E', CommandNames.Mathfrak, "𝔈"),
            new('F', CommandNames.Mathfrak, "𝔉"),

            new('G', CommandNames.Mathfrak, "𝔊"),
            new('H', CommandNames.Mathfrak, "ℌ"),
            new('I', CommandNames.Mathfrak, "ℑ"),
            new('J', CommandNames.Mathfrak, "𝔍"),
            new('K', CommandNames.Mathfrak, "𝔎"),

            new('L', CommandNames.Mathfrak, "𝔏"),
            new('M', CommandNames.Mathfrak, "𝔐"),
            new('N', CommandNames.Mathfrak, "𝔑"),
            new('O', CommandNames.Mathfrak, "𝔒"),
            new('P', CommandNames.Mathfrak, "𝔓"),

            new('Q', CommandNames.Mathfrak, "𝔔"),
            new('R', CommandNames.Mathfrak, "ℜ"),
            new('S', CommandNames.Mathfrak, "𝔖"),
            new('T', CommandNames.Mathfrak, "𝔗"),
            new('U', CommandNames.Mathfrak, "𝔘"),

            new('V', CommandNames.Mathfrak, "𝔙"),
            new('W', CommandNames.Mathfrak, "𝔚"),
            new('X', CommandNames.Mathfrak, "𝔛"),
            new('Y', CommandNames.Mathfrak, "𝔜"),
            new('Z', CommandNames.Mathfrak, "ℨ"),
            new('a', CommandNames.Mathfrak, "𝔞"),
            new('b', CommandNames.Mathfrak, "𝔟"),
            new('c', CommandNames.Mathfrak, "𝔠"),
            new('d', CommandNames.Mathfrak, "𝔡"),
            new('e', CommandNames.Mathfrak, "𝔢"),
            new('f', CommandNames.Mathfrak, "𝔣"),

            new('g', CommandNames.Mathfrak, "𝔤"),
            new('h', CommandNames.Mathfrak, "𝔥"),
            new('i', CommandNames.Mathfrak, "𝔦"),
            new('j', CommandNames.Mathfrak, "𝔧"),
            new('k', CommandNames.Mathfrak, "𝔨"),

            new('l', CommandNames.Mathfrak, "𝔩"),
            new('m', CommandNames.Mathfrak, "𝔪"),
            new('n', CommandNames.Mathfrak, "𝔫"),
            new('o', CommandNames.Mathfrak, "𝔬"),
            new('p', CommandNames.Mathfrak, "𝔭"),

            new('q', CommandNames.Mathfrak, "𝔮"),
            new('r', CommandNames.Mathfrak, "𝔯"),
            new('s', CommandNames.Mathfrak, "𝔰"),
            new('t', CommandNames.Mathfrak, "𝔱"),
            new('u', CommandNames.Mathfrak, "𝔲"),

            new('v', CommandNames.Mathfrak, "𝔳"),
            new('w', CommandNames.Mathfrak, "𝔴"),
            new('x', CommandNames.Mathfrak, "𝔵"),
            new('y', CommandNames.Mathfrak, "𝔶"),
            new('z', CommandNames.Mathfrak, "𝔷"),

            // mathscr
            new('A', CommandNames.Mathscr, "𝒜"),
            new('B', CommandNames.Mathscr, "ℬ"),
            new('C', CommandNames.Mathscr, "𝒞"),
            new('D', CommandNames.Mathscr, "𝒟"),
            new('E', CommandNames.Mathscr, "ℰ"),
            new('F', CommandNames.Mathscr, "ℱ"),
            new('G', CommandNames.Mathscr, "𝒢"),
            new('H', CommandNames.Mathscr, "ℋ"),
            new('I', CommandNames.Mathscr, "ℐ"),
            new('J', CommandNames.Mathscr, "𝒥"),
            new('K', CommandNames.Mathscr, "𝒦"),
            new('L', CommandNames.Mathscr, "ℒ"),
            new('M', CommandNames.Mathscr, "ℳ"),
            new('N', CommandNames.Mathscr, "𝒩"),
            new('O', CommandNames.Mathscr, "𝒪"),
            new('P', CommandNames.Mathscr, "𝒫"),
            new('Q', CommandNames.Mathscr, "𝒬"),
            new('R', CommandNames.Mathscr, "ℛ"),
            new('S', CommandNames.Mathscr, "𝒮"),
            new('T', CommandNames.Mathscr, "𝒯"),
            new('U', CommandNames.Mathscr, "𝒰"),
            new('V', CommandNames.Mathscr, "𝒱"),
            new('W', CommandNames.Mathscr, "𝒲"),
            new('X', CommandNames.Mathscr, "𝒳"),
            new('Y', CommandNames.Mathscr, "𝒴"),
            new('Z', CommandNames.Mathscr, "𝒵")
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
            { CommandNames.Alpha, new SymbolDefinition { PlainText = "alpha", ScreenReader = "alpha", HumanFriendly = "α" } },
            { CommandNames.Beta, new SymbolDefinition { PlainText = "beta", ScreenReader = "beta", HumanFriendly = "β" } },
            { CommandNames.Gamma, new SymbolDefinition { PlainText = "gamma", ScreenReader = "gamma", HumanFriendly = "γ" } },
            { CommandNames.Delta, new SymbolDefinition { PlainText = "delta", ScreenReader = "delta", HumanFriendly = "δ" } },
            { CommandNames.Epsilon, new SymbolDefinition { PlainText = "epsilon", ScreenReader = "epsilon", HumanFriendly = "ε" } },
            { CommandNames.Varepsilon, new SymbolDefinition { PlainText = "varepsilon", ScreenReader = "varepsilon", HumanFriendly = "ε" } },
            { CommandNames.Zeta, new SymbolDefinition { PlainText = "zeta", ScreenReader = "zeta", HumanFriendly = "ζ" } },
            { CommandNames.Eta, new SymbolDefinition { PlainText = "eta", ScreenReader = "eta", HumanFriendly = "η" } },
            { CommandNames.Theta, new SymbolDefinition { PlainText = "theta", ScreenReader = "theta", HumanFriendly = "θ" } },
            { CommandNames.Iota, new SymbolDefinition { PlainText = "iota", ScreenReader = "iota", HumanFriendly = "ι" } },
            { CommandNames.Kappa, new SymbolDefinition { PlainText = "kappa", ScreenReader = "kappa", HumanFriendly = "κ" } },
            { CommandNames.Varkappa, new SymbolDefinition { PlainText = "varkappa", ScreenReader = "varkappa", HumanFriendly = "ϰ" } },
            { CommandNames.Lambda, new SymbolDefinition { PlainText = "lambda", ScreenReader = "lambda", HumanFriendly = "λ" } },
            { CommandNames.Mu, new SymbolDefinition { PlainText = "mu", ScreenReader = "mu", HumanFriendly = "μ" } },
            { CommandNames.Nu, new SymbolDefinition { PlainText = "nu", ScreenReader = "nu", HumanFriendly = "ν" } },
            { CommandNames.Xi, new SymbolDefinition { PlainText = "xi", ScreenReader = "xi", HumanFriendly = "ξ" } },
            { CommandNames.Omicron, new SymbolDefinition { PlainText = "omicron", ScreenReader = "omicron", HumanFriendly = "ο" } },
            { CommandNames.Pi, new SymbolDefinition { PlainText = "pi", ScreenReader = "pi", HumanFriendly = "π" } },
            { CommandNames.Varpi, new SymbolDefinition { PlainText = "varpi", ScreenReader = "varpi", HumanFriendly = "ϖ" } },
            { CommandNames.Rho, new SymbolDefinition { PlainText = "rho", ScreenReader = "rho", HumanFriendly = "ρ" } },
            { CommandNames.Varrho, new SymbolDefinition { PlainText = "varrho", ScreenReader = "varrho", HumanFriendly = "ϱ" } },
            { CommandNames.Sigma, new SymbolDefinition { PlainText = "sigma", ScreenReader = "sigma", HumanFriendly = "σ" } },
            { CommandNames.Varsigma, new SymbolDefinition { PlainText = "varsigma", ScreenReader = "varsigma", HumanFriendly = "ς" } },
            { CommandNames.Tau, new SymbolDefinition { PlainText = "tau", ScreenReader = "tau", HumanFriendly = "τ" } },
            { CommandNames.Upsilon, new SymbolDefinition { PlainText = "upsilon", ScreenReader = "upsilon", HumanFriendly = "υ" } },
            { CommandNames.Phi, new SymbolDefinition { PlainText = "phi", ScreenReader = "phi", HumanFriendly = "φ" } },
            { CommandNames.Varphi, new SymbolDefinition { PlainText = "varphi", ScreenReader = "varphi", HumanFriendly = "\u03D5" } },
            { CommandNames.Chi, new SymbolDefinition { PlainText = "chi", ScreenReader = "chi", HumanFriendly = "χ" } },
            { CommandNames.Psi, new SymbolDefinition { PlainText = "psi", ScreenReader = "psi", HumanFriendly = "ψ" } },
            { CommandNames.Omega, new SymbolDefinition { PlainText = "omega", ScreenReader = "omega", HumanFriendly = "ω" } },
            { CommandNames.BigGamma, new SymbolDefinition { PlainText = "Gamma", ScreenReader = "Gamma", HumanFriendly = "Γ" } },
            { CommandNames.BigDelta, new SymbolDefinition { PlainText = "Delta", ScreenReader = "Delta", HumanFriendly = "Δ" } },
            { CommandNames.BigTheta, new SymbolDefinition { PlainText = "Theta", ScreenReader = "Theta", HumanFriendly = "Θ" } },
            { CommandNames.BigLambda, new SymbolDefinition { PlainText = "Lambda", ScreenReader = "Lambda", HumanFriendly = "Λ" } },
            { CommandNames.BigXi, new SymbolDefinition { PlainText = "Xi", ScreenReader = "Xi", HumanFriendly = "Ξ" } },
            { CommandNames.BigPi, new SymbolDefinition { PlainText = "Pi", ScreenReader = "Pi", HumanFriendly = "Π" } },
            { CommandNames.BigSigma, new SymbolDefinition { PlainText = "Sigma", ScreenReader = "Sigma", HumanFriendly = "Σ" } },
            { CommandNames.BigUpsilon, new SymbolDefinition { PlainText = "Upsilon", ScreenReader = "Upsilon", HumanFriendly = "Υ" } },
            { CommandNames.BigPhi, new SymbolDefinition { PlainText = "Phi", ScreenReader = "Phi", HumanFriendly = "Φ" } },
            { CommandNames.BigPsi, new SymbolDefinition { PlainText = "Psi", ScreenReader = "Psi", HumanFriendly = "Ψ" } },
            { CommandNames.BigOmega, new SymbolDefinition { PlainText = "Omega", ScreenReader = "Omega", HumanFriendly = "Ω" } },
            { CommandNames.Times, new SymbolDefinition { PlainText = "times", ScreenReader = "times", HumanFriendly = "×" } },
            { CommandNames.Div, new SymbolDefinition { PlainText = "div", ScreenReader = "divided by", HumanFriendly = "÷" } },
            { CommandNames.Pm, new SymbolDefinition { PlainText = "pm", ScreenReader = "plus-minus", HumanFriendly = "±" } },
            { CommandNames.Mp, new SymbolDefinition { PlainText = "mp", ScreenReader = "minus-plus", HumanFriendly = "∓" } },
            { CommandNames.Cdot, new SymbolDefinition { PlainText = "cdot", ScreenReader = "cdot", HumanFriendly = "·" } },
            { CommandNames.Circ, new SymbolDefinition { PlainText = "circ", ScreenReader = "{0} degrees", HumanFriendly = "{0}°" , HumanFriendlyKey = "°", OpenAI ="{0} degrees" } },
            { CommandNames.Bullet, new SymbolDefinition { PlainText = "bullet", ScreenReader = "bullet", HumanFriendly = "•" } },
            { CommandNames.Oplus, new SymbolDefinition { PlainText = "oplus", ScreenReader = "oplus", HumanFriendly = "⊕" } },
            { CommandNames.Ominus, new SymbolDefinition { PlainText = "ominus", ScreenReader = "ominus", HumanFriendly = "⊖" } },
            { CommandNames.Otimes, new SymbolDefinition { PlainText = "otimes", ScreenReader = "tensor product", HumanFriendly = "⊗" } },
            { CommandNames.Oslash, new SymbolDefinition { PlainText = "oslash", ScreenReader = "oslash", HumanFriendly = "⊘" } },
            { CommandNames.Odot, new SymbolDefinition { PlainText = "odot", ScreenReader = "circled dot", HumanFriendly = "⊙" } },
            { CommandNames.Parallel, new SymbolDefinition { PlainText = "parallel", ScreenReader = "parallel to", HumanFriendly = "∥" } },
            { CommandNames.Perp, new SymbolDefinition { PlainText = "perp", ScreenReader = "perpendicular to", HumanFriendly = "⊥" } },
            { CommandNames.Implies, new SymbolDefinition { PlainText = "implies", ScreenReader = "implies", HumanFriendly = "⇒" } },
            { CommandNames.Iff, new SymbolDefinition { PlainText = "iff", ScreenReader = "if and only if", HumanFriendly = "⇔" } },
            { CommandNames.Rightarrow, new SymbolDefinition { PlainText = "rightarrow", ScreenReader = "right arrow", ExceptionalScreenReader = "approaches", HumanFriendly = "→" } },
            { CommandNames.Leftarrow, new SymbolDefinition { PlainText = "leftarrow", ScreenReader = "left arrow", HumanFriendly = "←" } },
            { CommandNames.Uparrow, new SymbolDefinition { PlainText = "uparrow", ScreenReader = "up arrow", HumanFriendly = "↑" } },
            { CommandNames.Downarrow, new SymbolDefinition { PlainText = "downarrow", ScreenReader = "down arrow", HumanFriendly = "↓" } },
            { CommandNames.BigRightarrow, new SymbolDefinition { PlainText = "Rightarrow", ScreenReader = "right double arrow", HumanFriendly = "⇒" } },
            { CommandNames.BigLeftarrow, new SymbolDefinition { PlainText = "Leftarrow", ScreenReader = "left double arrow", HumanFriendly = "⇐" } },
            { CommandNames.Leftrightarrow, new SymbolDefinition { PlainText = "leftrightarrow", ScreenReader = "left right arrow", HumanFriendly = "↔" } },
            { CommandNames.BigLeftrightarrow, new SymbolDefinition { PlainText = "Leftrightarrow", ScreenReader = "if and only if", HumanFriendly = "⇔" } },
            { CommandNames.Leq, new SymbolDefinition { PlainText = "leq", ScreenReader = "less than or equal to", HumanFriendly = "≤" } },
            { CommandNames.Geq, new SymbolDefinition { PlainText = "geq", ScreenReader = "greater than or equal to", HumanFriendly = "≥" } },
            { CommandNames.Neq, new SymbolDefinition { PlainText = "neq", ScreenReader = "not equal to", HumanFriendly = "≠" } },
            { CommandNames.Approx, new SymbolDefinition { PlainText = "approx", ScreenReader = "approximately equal to", HumanFriendly = "≈" } },
            { CommandNames.Equiv, new SymbolDefinition { PlainText = "equiv", ScreenReader = "congruent to", HumanFriendly = "≡" } },
            { CommandNames.Propto, new SymbolDefinition { PlainText = "propto", ScreenReader = "proportional to", HumanFriendly = "∝" } },
            { CommandNames.Infty, new SymbolDefinition { PlainText = "infty", ScreenReader = "infinity", HumanFriendly = "∞" } },
            { CommandNames.Nabla, new SymbolDefinition { PlainText = "nabla", ScreenReader = "nabla", HumanFriendly = "∇" } },
            { CommandNames.Partial, new SymbolDefinition { PlainText = "partial", ScreenReader = "partial derivative", HumanFriendly = "∂" } },
            { CommandNames.Int, new SymbolDefinition { PlainText = "integral", ScreenReader = "integral from {0} to {1}", HumanFriendly = "∫{0}{1}" , HumanFriendlyKey = "∫"} },
            { CommandNames.Sum, new SymbolDefinition { PlainText = "summation", ScreenReader = "summation from {0} to {1}", HumanFriendly = "∑{0}{1}" , HumanFriendlyKey = "∑"} },
            { CommandNames.Prod, new SymbolDefinition { PlainText = "product", ScreenReader = "prod from {0} to {1}", HumanFriendly = "∏{0}{1}" , HumanFriendlyKey = "∏"} },
            { CommandNames.Lim, new SymbolDefinition { PlainText = "limit", ScreenReader = "limit as {0} of", HumanFriendly = "lim_{{{0}}}" , HumanFriendlyKey = "lim"} },
            { CommandNames.Sqrt, new SymbolDefinition { PlainText = "sqrt", ScreenReader = "the square root of {0}", HumanFriendly ="√({0})", OpenAI ="sqrt({0})" } },
            { CommandNames.Frac, new SymbolDefinition { PlainText = "fraction with numerator", OpenAI = "{0}/{1}" , HumanFriendly  = "{0} / {1}", ScreenReader = "fraction with numerator {0} and denominator {1}"  } },
            { CommandNames.Binom, new SymbolDefinition { PlainText = "binom", OpenAI = "binom({0},{1})" , HumanFriendly = "({0} {1})" , ScreenReader = "{0} choose {1}" } },
            { CommandNames.Hbar, new SymbolDefinition { PlainText = "hbar", ScreenReader = "h bar", HumanFriendly = "ħ" } },
            { CommandNames.Ell, new SymbolDefinition { PlainText = "ell", ScreenReader = "ell", HumanFriendly = "ℓ" } },
            { CommandNames.Wp, new SymbolDefinition { PlainText = "wp", ScreenReader = "Weierstrass p", HumanFriendly = "wp" } },
            { CommandNames.Re, new SymbolDefinition { PlainText = "Re", ScreenReader = "Real part", HumanFriendly = "ℜ" } },
            { CommandNames.Im, new SymbolDefinition { PlainText = "Im", ScreenReader = "Imaginary part", HumanFriendly = "ℑ" } },
            { CommandNames.Forall, new SymbolDefinition { PlainText = "forall", ScreenReader = "for all", HumanFriendly = "∀" } },
            { CommandNames.Exists, new SymbolDefinition { PlainText = "exists", ScreenReader = "there exists", HumanFriendly = "∃" } },
            { CommandNames.In, new SymbolDefinition { PlainText = "in", ScreenReader = "in", HumanFriendly = "∈" } },
            { CommandNames.To, new SymbolDefinition { PlainText = "->", ScreenReader = "approaches", HumanFriendly = "→" } },
            { CommandNames.Vec, new SymbolDefinition { PlainText = "vector", ScreenReader = "vector {0}", HumanFriendly = "{0}⃗", OpenAI = "{0}" } },
            { CommandNames.Hat, new SymbolDefinition { PlainText = "hat", ScreenReader = "{0} hat", HumanFriendly = "{0}̂", OpenAI = "hat {0}" } },
            { CommandNames.Overline, new SymbolDefinition { PlainText = "bar", ScreenReader = "{0} bar", HumanFriendly = "{0}̅", OpenAI = "overline({0})" } },
            { CommandNames.Sin, new SymbolDefinition { PlainText = "sine of", ScreenReader ="sine of {0}",  HumanFriendly ="sin({0})" ,  OpenAI ="sin({0})" } },
            { CommandNames.Cos, new SymbolDefinition { PlainText = "cosine of", ScreenReader ="cosine of {0}", HumanFriendly ="cos({0})" ,  OpenAI ="cos({0})"} },
            { CommandNames.Tan, new SymbolDefinition { PlainText = "tangent of", ScreenReader ="tangent of {0}", HumanFriendly ="tan({0})" ,  OpenAI ="tan({0})"} },
            { CommandNames.Log, new SymbolDefinition { PlainText = "logarithm of", ScreenReader ="logarithm of {0}",HumanFriendly ="log({0})" ,   OpenAI ="log({0})"} },
            { CommandNames.Ln, new SymbolDefinition { PlainText = "natural logarithm of", ScreenReader ="natural logarithm of {0}", HumanFriendly ="ln({0})" ,  OpenAI ="ln({0})"} },
            { CommandNames.Exp, new SymbolDefinition { PlainText = "e to the power of", ScreenReader ="e to the power of {0}", HumanFriendly ="e{0}" ,  OpenAI ="exp({0})" } },
            { CommandNames.Det, new SymbolDefinition { PlainText = "determinant of", ScreenReader ="determinant of {0}", HumanFriendly ="det({0})" ,  OpenAI ="det({0})" } },
            { CommandNames.Arcsin, new SymbolDefinition { PlainText = "arcsin", ScreenReader = "arcsin", HumanFriendly = "sin⁻¹" } },
            { CommandNames.Arccos, new SymbolDefinition { PlainText = "arccos", ScreenReader = "arccos", HumanFriendly = "cos⁻¹" } },
            { CommandNames.Arctan, new SymbolDefinition { PlainText = "arctan", ScreenReader = "arctan", HumanFriendly = "tan⁻¹" } },
            { CommandNames.Cup, new SymbolDefinition { PlainText = "cup", ScreenReader = "union", HumanFriendly = "∪" } },
            { CommandNames.Cap, new SymbolDefinition { PlainText = "cap", ScreenReader = "intersection", HumanFriendly = "∩" } },
            { CommandNames.Subset, new SymbolDefinition { PlainText = "subset", ScreenReader = "subset of", HumanFriendly = "⊂" } },
            { CommandNames.Supset, new SymbolDefinition { PlainText = "supset", ScreenReader = "superset of", HumanFriendly = "⊃" } },
            { CommandNames.Neg, new SymbolDefinition { PlainText = "neg", ScreenReader = "not", HumanFriendly = "¬" } },
            { CommandNames.Land, new SymbolDefinition { PlainText = "land", ScreenReader = "and", HumanFriendly = "∧" } },
            { CommandNames.Lor, new SymbolDefinition { PlainText = "lor", ScreenReader = "or", HumanFriendly = "∨" } },
            { CommandNames.Prime, new SymbolDefinition { PlainText = "prime", ScreenReader = "{0} prime", HumanFriendly = "{0}′", HumanFriendlyKey="′", OpenAI="{0} prime" } },
            { CommandNames.Blacksquare, new SymbolDefinition { PlainText = "blacksquare", ScreenReader = "black square", HumanFriendly = "■" } },
            { CommandNames.Heartsuit, new SymbolDefinition { PlainText = "heartsuit", ScreenReader = "heart suit", HumanFriendly = "♥" } },
            { CommandNames.Clubsuit, new SymbolDefinition { PlainText = "clubsuit", ScreenReader = "club suit", HumanFriendly = "♣" } },
            { CommandNames.Diamondsuit, new SymbolDefinition { PlainText = "diamondsuit", ScreenReader = "diamond suit", HumanFriendly = "♦" } },
            { CommandNames.Spadesuit, new SymbolDefinition { PlainText = "spadesuit", ScreenReader = "spade suit", HumanFriendly = "♠" } },
            { CommandNames.Natural, new SymbolDefinition { PlainText = "natural", ScreenReader = "natural", HumanFriendly = "♮" } },
            { CommandNames.Sharp, new SymbolDefinition { PlainText = "sharp", ScreenReader = "sharp", HumanFriendly = "♯" } },
            { CommandNames.Flat, new SymbolDefinition { PlainText = "flat", ScreenReader = "flat", HumanFriendly = "♭" } },
            { CommandNames.Triangle, new SymbolDefinition { PlainText = "triangle", ScreenReader = "triangle", HumanFriendly = "△" } },
            { CommandNames.Triangledown, new SymbolDefinition { PlainText = "triangledown", ScreenReader = "downward triangle", HumanFriendly = "▽" } },
            { CommandNames.Angle, new SymbolDefinition { PlainText = "angle", ScreenReader = "angle", HumanFriendly = "∠" } },
            { CommandNames.Measuredangle, new SymbolDefinition { PlainText = "measuredangle", ScreenReader = "measured angle", HumanFriendly = "∡" } },
            { CommandNames.Dots, new SymbolDefinition { PlainText = "...", ScreenReader = "dots", HumanFriendly = "…" } },
            { CommandNames.Cdots, new SymbolDefinition { PlainText = "...", ScreenReader = "centered dots", HumanFriendly = "⋯" } },
            { CommandNames.Vdots, new SymbolDefinition { PlainText = "...", ScreenReader = "vertical dots", HumanFriendly = "⋮" } },
            { CommandNames.Ddots, new SymbolDefinition { PlainText = "...", ScreenReader = "diagonal dots", HumanFriendly = "⋱" } },
            { CommandNames.Thinspace, new SymbolDefinition { PlainText = " ", ScreenReader = " ", HumanFriendly = " " } },


            { CommandNames.Mathcal, new SymbolDefinition { PlainText = "mathcal", ScreenReader = "{0}}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathbb, new SymbolDefinition { PlainText = "mathbb", ScreenReader = "the set of real numbers", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathfrak, new SymbolDefinition { PlainText = "mathfrak", ScreenReader = "frak {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathscr, new SymbolDefinition { PlainText = "mathscr", ScreenReader = "scr {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },

            { CommandNames.Text, new SymbolDefinition { PlainText = "text", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Textrm, new SymbolDefinition { PlainText = "textrm", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathbf, new SymbolDefinition { PlainText = "mathbf", ScreenReader = "bf {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathit, new SymbolDefinition { PlainText = "mathit", ScreenReader = "it {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathsf, new SymbolDefinition { PlainText = "mathsf", ScreenReader = "sf {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathrm, new SymbolDefinition { PlainText = "mathrm", ScreenReader = "{0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },
            { CommandNames.Mathtt, new SymbolDefinition { PlainText = "mathtt", ScreenReader = "tt {0}", HumanFriendly = "{0}" , OpenAI = "{0}" } },

            { CommandNames.Superscript, new SymbolDefinition { PlainText = "for_superscript", ScreenReader = "{0} to the power of {1}", HumanFriendly = "{0}^{1}}" , OpenAI ="{0}^{1}" } },
            { CommandNames.Subscript, new SymbolDefinition { PlainText = "for_subscript", ScreenReader = "{0} subscript {1}", HumanFriendly = "{0}_{1}" , OpenAI =  "{0}_{1}" } },
            { CommandNames.Matrix, new SymbolDefinition { PlainText = "for_matrix", ScreenReader = "a {0}x{1} matrix with rows {2}", HumanFriendly = "matrix[{0}]" , OpenAI = "{0}" } },

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
                .Where(fc => fc.FontCommand == CommandNames.Mathbb)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathcalMap = FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathcal)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathfrakMap = FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathfrak)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathscrMap = FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathscr)
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

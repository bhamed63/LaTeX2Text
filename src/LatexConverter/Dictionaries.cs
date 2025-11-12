
using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
    public class SymbolDefinition
    {
        public string? PlainText { get; set; }
        public string? ScreenReader { get; set; }
        public string? ScreenReaderTemplate { get; set; }
        public string? ExceptionalScreenReader { get; set; }
        public string? HumanFriendly { get; set; }
        public string? HumanFriendlyTemplate { get; set; }
        public string? OpenAITemplate { get; set; }
    }

    public record FontCharacter(char BaseChar, string FontCommand, char UnicodeChar);
    public record ScriptCharacter(char BaseChar, char? Superscript, char? Subscript);

    /// <summary>
    /// Provides centralized storage for symbol and character mappings used in LaTeX conversion.
    /// </summary>
    internal static class Dictionaries
    {
        private static readonly List<FontCharacter> FontLibrary = new()
        {
            new('R', @"\mathbb", 'ℝ'), new('C', @"\mathbb", 'ℂ'), new('N', @"\mathbb", 'ℕ'), new('Q', @"\mathbb", 'ℚ'), new('Z', @"\mathbb", 'ℤ'),
            new('E', @"\mathcal", 'ℰ'), new('F', @"\mathcal", 'ℱ'), new('H', @"\mathcal", 'ℋ'), new('B', @"\mathcal", 'ℬ'), new('I', @"\mathcal", 'ℐ'),
            new('R', @"\mathcal", 'ℛ'), new('L', @"\mathcal", 'ℒ'), new('M', @"\mathcal", 'ℳ'),
            new('C', @"\mathfrak", 'ℭ'), new('H', @"\mathfrak", 'ℌ'), new('I', @"\mathfrak", 'ℑ'), new('R', @"\mathfrak", 'ℜ'), new('Z', @"\mathfrak", 'ℨ'),
            new('E', @"\mathscr", 'ℰ'), new('F', @"\mathscr", 'ℱ'), new('H', @"\mathscr", 'ℋ'), new('I', @"\mathscr", 'ℐ'), new('L', @"\mathscr", 'ℒ'),
            new('M', @"\mathscr", 'ℳ'), new('R', @"\mathscr", 'ℛ')
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
            { @"\circ", new SymbolDefinition { PlainText = "circ", ScreenReader = " degrees ", HumanFriendly = "∘" } },
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
            { @"\int", new SymbolDefinition { PlainText = "integral", ScreenReader = "integral", HumanFriendly = "∫" } },
            { @"\sum", new SymbolDefinition { PlainText = "summation", ScreenReader = "summation", HumanFriendly = "∑" } },
            { @"\prod", new SymbolDefinition { PlainText = "product", ScreenReader = "product", HumanFriendly = "∏" } },
            { @"\lim", new SymbolDefinition { PlainText = "limit", ScreenReader = "limit as", HumanFriendly = "lim" } },
            { @"\sqrt", new SymbolDefinition { PlainText = "sqrt" } },
            { @"\frac", new SymbolDefinition { PlainText = "fraction with numerator" } },
            { @"\hbar", new SymbolDefinition { PlainText = "hbar", ScreenReader = "h bar", HumanFriendly = "ħ" } },
            { @"\ell", new SymbolDefinition { PlainText = "ell", ScreenReader = "ell", HumanFriendly = "ℓ" } },
            { @"\wp", new SymbolDefinition { PlainText = "wp", ScreenReader = "Weierstrass p", HumanFriendly = "wp" } },
            { @"\Re", new SymbolDefinition { PlainText = "Re", ScreenReader = "Real part", HumanFriendly = "ℜ" } },
            { @"\Im", new SymbolDefinition { PlainText = "Im", ScreenReader = "Imaginary part", HumanFriendly = "ℑ" } },
            { @"\forall", new SymbolDefinition { PlainText = "forall", ScreenReader = "for all", HumanFriendly = "∀" } },
            { @"\exists", new SymbolDefinition { PlainText = "exists", ScreenReader = "there exists", HumanFriendly = "∃" } },
            { @"\in", new SymbolDefinition { PlainText = "in", ScreenReader = "in", HumanFriendly = "∈" } },
            { @"\to", new SymbolDefinition { PlainText = "->", ScreenReader = "approaches", HumanFriendly = "→" } },
            { @"\vec", new SymbolDefinition { PlainText = "vector", ScreenReaderTemplate = "vector {0}", HumanFriendlyTemplate = "{0}⃗", OpenAITemplate = "{0}" } },
            { @"\hat", new SymbolDefinition { PlainText = "hat", ScreenReaderTemplate = "{0} hat", HumanFriendlyTemplate = "{0}̂", OpenAITemplate = "hat {0}" } },
            { @"\mathcal", new SymbolDefinition { PlainText = "calligraphic", ScreenReader = "calligraphic" } },
            { @"\mathbb", new SymbolDefinition { PlainText = "the set of real numbers", ScreenReader = "the set of real numbers" } },
            { @"\overline", new SymbolDefinition { PlainText = "bar", ScreenReaderTemplate = "{0} bar", HumanFriendlyTemplate = "{0}̅", OpenAITemplate = "overline({0})" } },
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
            { @"\prime", new SymbolDefinition { PlainText = "prime", ScreenReader = "prime", HumanFriendly = "′" } },
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
        { @"\binom", new SymbolDefinition { PlainText = "binom", ScreenReaderTemplate = "{0} choose {1}", HumanFriendlyTemplate = "({0} {1})", OpenAITemplate = "binom({0},{1})" } },
        { @"\cos", new SymbolDefinition { PlainText = "cosine of", ScreenReaderTemplate = "cosine of {0}", HumanFriendlyTemplate = "cos({0})", OpenAITemplate = "cos({0})" } },
        { @"\sin", new SymbolDefinition { PlainText = "sine of", ScreenReaderTemplate = "sine of {0}", HumanFriendlyTemplate = "sin({0})", OpenAITemplate = "sin({0})" } },
        { @"\tan", new SymbolDefinition { PlainText = "tangent of", ScreenReaderTemplate = "tangent of {0}", HumanFriendlyTemplate = "tan({0})", OpenAITemplate = "tan({0})" } },
        { @"\log", new SymbolDefinition { PlainText = "logarithm of", ScreenReaderTemplate = "logarithm of {0}", HumanFriendlyTemplate = "log({0})", OpenAITemplate = "log({0})" } },
        { @"\ln", new SymbolDefinition { PlainText = "natural logarithm of", ScreenReaderTemplate = "natural logarithm of {0}", HumanFriendlyTemplate = "ln({0})", OpenAITemplate = "ln({0})" } },
        { @"\det", new SymbolDefinition { PlainText = "determinant of", ScreenReaderTemplate = "determinant of {0}", HumanFriendlyTemplate = "det({0})", OpenAITemplate = "det({0})" } },
        { @"\exp", new SymbolDefinition { PlainText = "e to the power of", ScreenReaderTemplate = "e to the power of {0}", OpenAITemplate = "exp({0})" } }
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
            ReverseHumanFriendlySymbolMap = HumanFriendlySymbolMap.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.First().Key.Substring(1));

            ScreenReaderTemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.ScreenReaderTemplate != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReaderTemplate!);

            HumanFriendlyTemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendlyTemplate != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendlyTemplate!);

            OpenAITemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.OpenAITemplate != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OpenAITemplate!);

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
        public static readonly Dictionary<char, char> MathbbMap;
        /// <summary>
        /// A map of characters to their math calligraphic Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathcalMap;

        /// <summary>
        /// A map of characters to their math Fraktur Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathfrakMap;

        /// <summary>
        /// A map of characters to their math script Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathscrMap;

        public static readonly Dictionary<char, string> ReverseMathFontMap;
    }
}

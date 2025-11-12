using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexConverter
{
<<<<<<< HEAD
    public class SymbolDefinition
    {
        public string? PlainText { get; set; }
        public string? ScreenReader { get; set; }
        //public string? ScreenReaderTemplate { get; set; }
        public string? ExceptionalScreenReader { get; set; }
        public string? HumanFriendly { get; set; }
        //public string? HumanFriendlyTemplate { get; set; }
        public string? OpenAI { get; set; }
    }

    public record FontCharacter(char BaseChar, string FontCommand, char UnicodeChar);
    public record ScriptCharacter(char BaseChar, char? Superscript, char? Subscript);

=======
>>>>>>> f8f458d83751f9c4184ce84440258a4a568cb450
    /// <summary>
    /// Provides centralized storage for symbol and character mappings used in LaTeX conversion.
    /// </summary>
    internal static class Dictionaries
    {
<<<<<<< HEAD
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
            { @"\sqrt", new SymbolDefinition { PlainText = "sqrt", HumanFriendly ="√({0})", OpenAI ="sqrt({0})" } },
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


            //{ @"\mathcal", new SymbolDefinition { PlainText = "calligraphic", ScreenReaderTemplate = "calligraphic {0}", OpenAITemplate = "{0}" } },
            //{ @"\mathbb", new SymbolDefinition { PlainText = "the set of real numbers", ScreenReader = "the set of real numbers"  } },
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
                .Where(kvp => kvp.Value.ScreenReader != null && kvp.Value.ScreenReader.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReader!);

            HumanFriendlyTemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null && kvp.Value.HumanFriendly.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendly!);

            OpenAITemplateMap = SymbolLibrary
                .Where(kvp => kvp.Value.OpenAI != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OpenAI!);

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


=======
>>>>>>> f8f458d83751f9c4184ce84440258a4a568cb450
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
            { @"\div", "divided by" }, { @"\pm", "plus-minus" }, { @"\mp", "minus-plus" },
            { @"\circ", " degrees " }, { @"\otimes", "tensor product" }, { @"\odot", "circled dot" },
            { @"\parallel", "parallel to" }, { @"\perp", "perpendicular to" },
            { @"\implies", "implies" }, { @"\iff", "if and only if" },
            { @"\rightarrow", "right arrow" }, { @"\leftarrow", "left arrow" }, { @"\uparrow", "up arrow" }, { @"\downarrow", "down arrow" },
            { @"\Rightarrow", "right double arrow" }, { @"\Leftarrow", "left double arrow" },
            { @"\leftrightarrow", "left right arrow" }, { @"\Leftrightarrow", "if and only if" },
            { @"\leq", "less than or equal to" }, { @"\geq", "greater than or equal to" },
            { @"\neq", "not equal to" }, { @"\approx", "approximately equal to" }, { @"\equiv", "congruent to" }, { @"\propto", "proportional to" },
            { @"\infty", "infinity" }, { @"\partial", "partial derivative" }, { @"\hbar", "h bar" }, { @"\wp", "Weierstrass p" },
            { @"\Re", "Real part" }, { @"\Im", "Imaginary part" },
            { @"\forall", "for all" }, { @"\to", "approaches" },
            { @"\arcsin", "arcsin" }, { @"\arccos", "arccos" }, { @"\arctan", "arctan" },
            { @"\cup", "union" }, { @"\cap", "intersection" }, { @"\subset", "subset of" }, { @"\supset", "superset of" },
            { @"\neg", "not" }, { @"\land", "and" }, { @"\lor", "or" }, { @"\prime", "prime" },
            { @"\blacksquare", "black square" }, { @"\heartsuit", "heart suit" }, { @"\clubsuit", "club suit" },
            { @"\diamondsuit", "diamond suit" }, { @"\spadesuit", "spade suit" }, { @"\natural", "natural" },
            { @"\sharp", "sharp" }, { @"\flat", "flat" }, { @"\triangle", "triangle" }, { @"\triangledown", "downward triangle" },
            { @"\angle", "angle" }, { @"\measuredangle", "measured angle" },
            { @"\dots", "dots" }, { @"\cdots", "centered dots" }, { @"\vdots", "vertical dots" }, { @"\ddots", "diagonal dots" },
            { @"\exists", "there exists" }
        };
        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly representations.
        /// </summary>
        public static readonly Dictionary<string, string> ExceptionalScreenReaderSymbolMap = new() {
            { @"\rightarrow", "approaches" }
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
<<<<<<< HEAD
        public static readonly Dictionary<char, char> MathbbMap;
        /// <summary>
        /// A map of characters to their math calligraphic Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, char> MathcalMap;
=======
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
>>>>>>> f8f458d83751f9c4184ce84440258a4a568cb450

        /// <summary>
        /// A map of characters to their math Fraktur Unicode representations.
        /// </summary>
<<<<<<< HEAD
        public static readonly Dictionary<char, char> MathfrakMap;
=======
        public static readonly Dictionary<char, char> MathfrakMap = new()
        {
            {'C', 'ℭ'}, {'H', 'ℌ'}, {'I', 'ℑ'}, {'R', 'ℜ'}, {'Z', 'ℨ'}
        };
>>>>>>> f8f458d83751f9c4184ce84440258a4a568cb450

        /// <summary>
        /// A map of characters to their math script Unicode representations.
        /// </summary>
<<<<<<< HEAD
        public static readonly Dictionary<char, char> MathscrMap;

        public static readonly Dictionary<char, string> ReverseMathFontMap;
=======
        public static readonly Dictionary<char, char> MathscrMap = new()
        {
            {'E', 'ℰ'}, {'F', 'ℱ'}, {'H', 'ℋ'}, {'I', 'ℐ'}, {'L', 'ℒ'},
            {'M', 'ℳ'}, {'R', 'ℛ'}
        };

        public static readonly Dictionary<char, string> ReverseMathFontMap = new()
        {
            {'ℝ', @"\mathbb{R}"}, {'ℂ', @"\mathbb{C}"}, {'ℕ', @"\mathbb{N}"}, {'ℚ', @"\mathbb{Q}"}, {'ℤ', @"\mathbb{Z}"},
            {'ℰ', @"\mathcal{E}"}, {'ℱ', @"\mathcal{F}"}, {'ℋ', @"\mathcal{H}"}, {'ℬ', @"\mathcal{B}"}, {'ℐ', @"\mathcal{I}"},
            {'ℛ', @"\mathcal{R}"}, {'ℒ', @"\mathcal{L}"}, {'ℳ', @"\mathcal{M}"},
            {'ℭ', @"\mathfrak{C}"}, {'ℌ', @"\mathfrak{H}"}, {'ℑ', @"\mathfrak{I}"}, {'ℜ', @"\mathfrak{R}"}, {'ℨ', @"\mathfrak{Z}"}
        };
>>>>>>> f8f458d83751f9c4184ce84440258a4a568cb450
    }
}

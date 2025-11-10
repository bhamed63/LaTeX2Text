using Xunit;

namespace LatexConverter.Tests
{
    public class ConvertHumanFriendlyToScreenFriendlyTextTests
    {
        private readonly LaTexConverter _converter = new LaTexConverter();

        [Theory]
        //[InlineData("α", "alpha")]
        //[InlineData("β", "beta")]
        //[InlineData("γ", "gamma")]
        //[InlineData("x²", "x squared")]
        //[InlineData("H₂O", "H subscript 2 O")]
        //[InlineData("43°", "43 degrees")]
        //[InlineData("a ≠ b", "a not equal to b")]
        //[InlineData("a ≥ b", "a greater than or equal to b")]
        //[InlineData("a ≤ b", "a less than or equal to b")]
        //[InlineData("∀ x", "for all x")]
        //[InlineData("∃ y", "there exists y")]
        //[InlineData("z ∈ S", "z in S")]
        //[InlineData("a ± b", "a plus-minus b")]
        //[InlineData("a ∓ b", "a minus-plus b")]
        //[InlineData("a ≡ b", "a congruent to b")]
        //[InlineData("∫ₐᵇ f(x) dx", "integral from a to b of f of x dx")]
        [InlineData("lim_{x→∞} f(x)", "limit as x approaches infinity of f of x")]
        //[InlineData("eˣ", "e to the power of x")]
        //[InlineData("(n k)", "n choose k")]
        //[InlineData("∪", "union")]
        //[InlineData("∩", "intersection")]
        //[InlineData("⊂", "subset of")]
        //[InlineData("⊃", "superset of")]
        //[InlineData("¬A", "not A")]
        //[InlineData("A ∧ B", "A and B")]
        //[InlineData("A ∨ B", "A or B")]
        //[InlineData("ħ", "h bar")]
        //[InlineData("ℓ", "ell")]
        //[InlineData("⊗", "tensor product")]
        //[InlineData("∥", "parallel to")]
        //[InlineData("⊥", "perpendicular to")]
        //[InlineData("⇒", "right double arrow")]
        //[InlineData("⇔", "if and only if")]
        //[InlineData("≈", "approximately equal to")]
        //[InlineData("→", "right arrow")]
        //[InlineData("←", "left arrow")]
        public void ConvertHumanFriendlyToScreenFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _converter.ConvertHumanFriendlyToScreenFriendlyText(input);
            Assert.Equal(expected, result);
        }
    }
}

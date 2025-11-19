using Xunit;

namespace LatexConverter.Tests
{
    public class ConvertHumanFriendlyToScreenFriendlyTextTests
    {
        private readonly LaTexConverter _converter = new LaTexConverter();

        [Theory]
        [InlineData("α", "alpha")]
        [InlineData("β", "beta")]
        [InlineData("γ", "gamma")]
        [InlineData("x²", "x squared")]
        [InlineData("H₂O", "H subscript 2 O")]
        [InlineData("43°", "43 degrees")]
        [InlineData("a ≠ b", "a not equal to b")]
        [InlineData("a ≥ b", "a greater than or equal to b")]
        [InlineData("a ≤ b", "a less than or equal to b")]
        [InlineData("∀ x", "for all x")]
        [InlineData("∃ y", "there exists y")]
        [InlineData("z ∈ S", "z in S")]
        [InlineData("a ± b", "a plus-minus b")]
        [InlineData("a ∓ b", "a minus-plus b")]
        [InlineData("a ≡ b", "a congruent to b")]
        [InlineData("∫ₐᵇ f(x) dx", "integral from a to b f(x) dx")]
        [InlineData("lim_{x→∞} f(x)", "limit as x approaches infinity of f(x)")]
        [InlineData("eˣ", "e to the power of x")]
        [InlineData("(n k)", "n choose k")]
        [InlineData("∪", "union")]
        [InlineData("∩", "intersection")]
        [InlineData("⊂", "subset of")]
        [InlineData("⊃", "superset of")]
        [InlineData("¬A", "not A")]
        [InlineData("A ∧ B", "A and B")]
        [InlineData("A ∨ B", "A or B")]
        [InlineData("ħ", "h bar")]
        [InlineData("ℓ", "ell")]
        [InlineData("⊗", "tensor product")]
        [InlineData("∥", "parallel to")]
        [InlineData("⊥", "perpendicular to")]
        [InlineData("⇒", "right double arrow")]
        [InlineData("⇔", "if and only if")]
        [InlineData("≈", "approximately equal to")]
        [InlineData("→", "right arrow")]
        [InlineData("←", "left arrow")]
        // [InlineData("ℰ₁", "calligraphic E subscript 1")]
        [InlineData("v⃗₀", "vector v subscript 0")]
        [InlineData("∑ᵢ₌₁ⁿ i²", "summation from i equals 1 to n i squared")]
        [InlineData("λ₁", "lambda subscript 1")]
        [InlineData("λ₁ + λ₂ = 1", "lambda subscript 1 plus lambda subscript 2 equals 1")]
        [InlineData("√(ω₁ / ω₂)", "square root of omega subscript 1 divided by omega subscript 2")]
        [InlineData("α² + β² = γ²", "alpha squared plus beta squared equals gamma squared")]
        [InlineData("F⃗ᵢ", "vector F subscript i")]
        [InlineData("This is the sum: ∑ᵢ₌₁ⁿ i.", "This is the sum: summation from i equals 1 to n i.")]
        public void ConvertHumanFriendlyToScreenFriendlyText_ConvertsCorrectly(string input, string expected)
        {
            var result = _converter.ConvertHumanFriendlyToScreenFriendlyText(input);
            Assert.Equal(expected, result);
        }
    }
}

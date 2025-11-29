using Xunit;
using System.Linq;

namespace LatexConverter.Tests
{
    public class ComponentExtractionVisitorTests
    {
        // private readonly LaTexConverter _converter = new LaTexConverter();

        // [Fact]
        // public void ExtractComponents_WithSimpleExpression_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"\sqrt{x * \frac{2}{m}}";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(2, result.Commands.Count);
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sqrt");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\frac");

        //     Assert.Single(result.Operators);
        //     Assert.Contains("*", result.Operators);

        //     Assert.Equal(2, result.Variables.Count);
        //     Assert.Contains("x", result.Variables);
        //     Assert.Contains("m", result.Variables);

        //     Assert.Single(result.Numbers);
        //     Assert.Contains("2", result.Numbers);
        // }

        // [Fact]
        // public void ExtractComponents_WithNestedCommands_ShouldReturnAllCommands()
        // {
        //     var latex = @"\frac{\sqrt{\sin{x}}}{2}";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(3, result.Commands.Count);
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\frac");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sqrt");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sin");

        //     Assert.Single(result.Variables);
        //     Assert.Contains("x", result.Variables);

        //     Assert.Single(result.Numbers);
        //     Assert.Contains("2", result.Numbers);
        // }

        // [Fact]
        // public void ExtractComponents_WithMultipleOperatorsAndVariables_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"a + b - c = d";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(3, result.Operators.Count);
        //     Assert.Contains("+", result.Operators);
        //     Assert.Contains("-", result.Operators);
        //     Assert.Contains("=", result.Operators);

        //     Assert.Equal(4, result.Variables.Count);
        //     Assert.Contains("a", result.Variables);
        //     Assert.Contains("b", result.Variables);
        //     Assert.Contains("c", result.Variables);
        //     Assert.Contains("d", result.Variables);
        // }

        // [Fact]
        // public void ExtractComponents_WithEmptyInput_ShouldReturnEmptyLists()
        // {
        //     var latex = "";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Empty(result.Commands);
        //     Assert.Empty(result.Operators);
        //     Assert.Empty(result.Variables);
        //     Assert.Empty(result.Numbers);
        // }

        // [Fact]
        // public void ExtractComponents_WithNoSpaces_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"a+b-c=d";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(3, result.Operators.Count);
        //     Assert.Contains("+", result.Operators);
        //     Assert.Contains("-", result.Operators);
        //     Assert.Contains("=", result.Operators);

        //     Assert.Equal(4, result.Variables.Count);
        //     Assert.Contains("a", result.Variables);
        //     Assert.Contains("b", result.Variables);
        //     Assert.Contains("c", result.Variables);
        //     Assert.Contains("d", result.Variables);
        // }

        // [Fact]
        // public void ExtractComponents_WithSimpleExpression_Include_Operator_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"\sqrt{x * \frac{2}{m + 4}}";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(2, result.Commands.Count);
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sqrt");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\frac");

        //     Assert.Equal(2, result.Operators.Count);
        //     Assert.Contains("*", result.Operators);
        //     Assert.Contains("+", result.Operators);

        //     Assert.Equal(2, result.Variables.Count);
        //     Assert.Contains("x", result.Variables);
        //     Assert.Contains("m", result.Variables);

        //     Assert.Equal(2, result.Numbers.Count);
        //     Assert.Contains("2", result.Numbers);
        //     Assert.Contains("4", result.Numbers);
        // }


        // [Fact]
        // public void ExtractComponents_Without_Operator_Without_Space_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"abcd";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Empty(result.Commands);
        //     Assert.Empty(result.Operators);
        //     Assert.Empty(result.Variables);
        //     Assert.Empty(result.Numbers);
        // }

        // [Fact]
        // public void ExtractComponents_Without_Operator_With_Space_ShouldReturnCorrectComponents()
        // {
        //     var latex = @"a b c d";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Empty(result.Commands);
        //     Assert.Empty(result.Operators);
        //     Assert.Empty(result.Variables);
        //     Assert.Empty(result.Numbers);
        // }

        // [Fact]
        // public void ExtractComponents_WithNestedCommands_and_Independent_Variable_ShouldReturnAllCommands()
        // {
        //     var latex = @"a +b is a sample of \frac{\sqrt{\sin{x}}}{2}";
        //     var result = _converter.ExtractComponents(latex);

        //     Assert.Equal(3, result.Commands.Count);
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\frac");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sqrt");
        //     Assert.Contains(result.Commands, c => c.Item2 == @"\sin");

        //     Assert.Equal(3, result.Variables.Count);
        //     Assert.Contains("x", result.Variables);
        //     Assert.Contains("a", result.Variables);
        //     Assert.Contains("b", result.Variables);

        //     Assert.Single(result.Numbers);
        //     Assert.Contains("2", result.Numbers);

        //     Assert.Single(result.Operators);
        //     Assert.Contains("+", result.Operators);
        // }
    }
}

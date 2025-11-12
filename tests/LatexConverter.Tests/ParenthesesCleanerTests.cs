using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LatexConverter.Tests
{
    public class ParenthesesCleanerTests
    {
        [Theory]
        [InlineData("((a+b))", "(a+b)")]
        [InlineData("(()=(a+b))", "()=(a+b)")]
        //[InlineData("(()=a+b))", "()=a+b)")]
        [InlineData("(((x+y)))", "(x+y)")]
        [InlineData("sqrt((a^2+b^2))/2", "sqrt(a^2+b^2)/2")]
        [InlineData("arccos (sqrt((I / I_0))) ]", "arccos (sqrt(I / I_0)) ]")]
        [InlineData("(a+b)*(c+d)", "(a+b)*(c+d)")] 
        [InlineData("() => arccos", "() => arccos")]                       // lambda with empty parentheses
        [InlineData("(a+9) => arccos", "(a+9) => arccos")]                // lambda with meaningful parentheses
        [InlineData("((x+y))", "(x+y)")]                                   // double parentheses around expression
        [InlineData("(((a)))", "(a)")]                                     // triple nested parentheses
        [InlineData("(x) + (y)", "(x) + (y)")]                             // parentheses around individual terms (keep)
        //[InlineData("((x+y)*(a+b))", "((x+y)*(a+b))")]                     // meaningful parentheses with operators (keep)
        [InlineData("(((((z)))))", "(z)")]                                  // deep nested parentheses
        [InlineData("foo((bar((x+y))))", "foo(bar(x+y))")]                 // nested function calls with redundant parentheses

        public void Test_RemoveExtraParentheses(string input, string expected)
        {
            string result = ParenthesesCleaner.RemoveExtraParentheses(input);
            Assert.Equal(expected, result);
        }
    }
}

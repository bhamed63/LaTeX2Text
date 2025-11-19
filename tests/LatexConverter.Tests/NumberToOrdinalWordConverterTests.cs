using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LatexConverter.Tests
{
    public class NumberToOrdinalWordConverterTests
    {
        [Theory] 
        // Tens with units
        [InlineData(21, "twenty first")]
        [InlineData(22, "twenty second")]
        [InlineData(31, "thirty first")]

        // Numbers > 31 (fallback)
        [InlineData(32, "thirty second")]
        [InlineData(100, "one hundredth")]
         
        // Special powers
        [InlineData(2, "square")]
        [InlineData(3, "cube")]

        // Single digits
        [InlineData(1, "first")]
        [InlineData(4, "fourth")]
        [InlineData(5, "fifth")]
        [InlineData(9, "ninth")]

        // Teens
        [InlineData(10, "tenth")]
        [InlineData(11, "eleventh")]
        [InlineData(12, "twelfth")]
        [InlineData(13, "thirteenth")]
        [InlineData(19, "nineteenth")]

        // Exact tens
        [InlineData(20, "twentieth")]
        [InlineData(30, "thirtieth")]
        [InlineData(40, "fortieth")]

        // Tens with units 
        [InlineData(33, "thirty third")]
        [InlineData(45, "forty fifth")]

        // Hundreds 
        [InlineData(101, "one hundred first")]
        [InlineData(342, "three hundred forty second")]

        // Thousands
        [InlineData(1000, "one thousandth")]
        [InlineData(2345, "two thousand three hundred forty fifth")]
        [InlineData(999999, "nine hundred ninety nine thousand nine hundred ninety ninth")]

        // Million
        [InlineData(1000000, "one millionth")]
        public void NumberToOrdinalWord_ShouldReturnCorrectWord(int number, string expected)
        {
            // Act
            var result = NumberToOrdinalWordConverter.NumberToOrdinalWord(number);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}

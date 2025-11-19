namespace LatexConverter
{
    public class NumberToOrdinalWordConverter
    {
        private static readonly string[] UnitsMap = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        private static readonly string[] UnitsOrdinal = { "", "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth" };
        private static readonly string[] TeensMap = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen",
                                                  "sixteen", "seventeen", "eighteen", "nineteen" };
        private static readonly string[] TeensOrdinal = { "tenth", "eleventh", "twelfth", "thirteenth", "fourteenth",
                                                       "fifteenth", "sixteenth", "seventeenth", "eighteenth", "nineteenth" };
        private static readonly string[] TensMap = { "", "", "twenty", "thirty", "forty", "fifty", "sixty",
                                                 "seventy", "eighty", "ninety" };
        private static readonly string[] TensOrdinal = { "", "", "twentieth", "thirtieth", "fortieth", "fiftieth",
                                                     "sixtieth", "seventieth", "eightieth", "ninetieth" };

        public static string NumberToOrdinalWord(int number)
        {
            if (number < 0 || number > 1000000) return number.ToString(); // fallback
            if (number == 0) return "zeroth";
            if (number == 2) return "square"; // special case
            if (number == 3) return "cube";   // special case
            if (number == 1000000) return "one millionth";

            return ConvertNumberToWords(number, true).Trim();
        }

        private static string ConvertNumberToWords(int number, bool isFinal)
        {
            if (number < 10)
                return isFinal ? UnitsOrdinal[number] : UnitsMap[number];

            if (number < 20)
                return isFinal ? TeensOrdinal[number - 10] : TeensMap[number - 10];

            if (number < 100)
            {
                int tens = number / 10;
                int units = number % 10;

                if (units == 0)
                    return isFinal ? TensOrdinal[tens] : TensMap[tens];

                return TensMap[tens] + " " + (isFinal ? UnitsOrdinal[units] : UnitsMap[units]);
            }

            if (number < 1000)
            {
                int hundreds = number / 100;
                int remainder = number % 100;

                string hundredsWord = UnitsMap[hundreds] + " hundred";

                if (remainder == 0)
                    return isFinal ? hundredsWord + "th" : hundredsWord;

                return hundredsWord + " " + ConvertNumberToWords(remainder, isFinal);
            }

            if (number < 1000000)
            {
                int thousands = number / 1000;
                int remainder = number % 1000;

                // Convert thousands group always as cardinal (isFinal=false)
                string thousandsWord = ConvertNumberToWords(thousands, false) + " thousand";

                if (remainder == 0)
                    return thousandsWord + "th";

                return thousandsWord + " " + ConvertNumberToWords(remainder, isFinal);
            }

            return number.ToString();
        }

    }
}
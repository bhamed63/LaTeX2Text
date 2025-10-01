import unittest
from src.latex_converter import LaTexConverter

class TestLaTexConverter(unittest.TestCase):

    def setUp(self):
        self.converter = LaTexConverter()

    def test_convert_to_openai_friendly_text(self):
        # Test basic symbols
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'\alpha'), '[alpha]')
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'\beta'), '[beta]')
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'\hbar'), '[h-bar]')

        # Test mixed text and symbols
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'The value is $\alpha$'), 'The value is [alpha]')

        # Test fractions
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'$\frac{1}{2}$'), '(1)/(2)')

        # Test nested fractions
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'$\frac{\alpha}{\beta}$'), '([alpha])/([beta])')
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'$\frac{1}{\frac{a}{b}}$'), '(1)/((a)/(b))')

        # Test square roots
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'$\sqrt{2}$'), 'sqrt(2)')
        self.assertEqual(self.converter.ConvertToOpenAIFriendlyText(r'$\sqrt{\alpha}$'), 'sqrt([alpha])')

        # Test complex expression
        self.assertEqual(
            self.converter.ConvertToOpenAIFriendlyText(r'$E = mc^2 + \frac{1}{2}mv^2$'),
            'E = mc^2 + (1)/(2)mv^2'
        )

    def test_convert_to_human_friendly_text(self):
        # Test basic symbols
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'\alpha'), 'α')
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'\beta'), 'β')
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'\hbar'), 'ħ')

        # Test mixed text and symbols
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'The value is $\alpha$'), 'The value is α')

        # Test fractions
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'$\frac{1}{2}$'), '1/2')

        # Test nested fractions
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'$\frac{\alpha}{\beta}$'), 'α/β')

        # Test square roots
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'$\sqrt{2}$'), '√(2)')
        self.assertEqual(self.converter.ConvertToHumanFriendlyText(r'$\sqrt{\alpha}$'), '√(α)')

        # Test complex expression
        self.assertEqual(
            self.converter.ConvertToHumanFriendlyText(r'$E = mc^2 + \frac{1}{2}mv^2$'),
            'E = mc² + 1/2mv²'
        )

    def test_convert_to_screen_reader_friendly_text(self):
        # Test basic symbols
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'\alpha'), 'alpha')
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'\beta'), 'beta')
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'\hbar'), 'h-bar')

        # Test mixed text and symbols
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'The value is $\alpha$'), 'The value is alpha')

        # Test fractions
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'$\frac{1}{2}$'), 'fraction with numerator 1 and denominator 2')

        # Test nested fractions
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'$\frac{\alpha}{\beta}$'), 'fraction with numerator alpha and denominator beta')

        # Test square roots
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'$\sqrt{2}$'), 'the square root of 2')
        self.assertEqual(self.converter.ConvertToScreenReaderFriendlyText(r'$\sqrt{\alpha}$'), 'the square root of alpha')

        # Test complex expression
        self.assertEqual(
            self.converter.ConvertToScreenReaderFriendlyText(r'$E = mc^2 + \frac{1}{2}mv^2$'),
            'E = mc square + fraction with numerator 1 and denominator 2 mv square'
        )
        # Test other powers
        self.assertEqual(
            self.converter.ConvertToScreenReaderFriendlyText(r'$x^3$'),
            'x cubed'
        )
        self.assertEqual(
            self.converter.ConvertToScreenReaderFriendlyText(r'$x^n$'),
            'x to the power of n'
        )


if __name__ == '__main__':
    unittest.main()
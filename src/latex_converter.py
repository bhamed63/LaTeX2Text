import re
from pylatexenc.latex2text import LatexNodes2Text, get_default_latex_context_db, MacroTextSpec

class LaTexConverter:

    def _get_symbol_map(self):
        return {
            # Greek Letters
            r'\alpha': 'alpha', r'\beta': 'beta', r'\gamma': 'gamma', r'\delta': 'delta',
            r'\epsilon': 'epsilon', r'\zeta': 'zeta', r'\eta': 'eta', r'\theta': 'theta',
            r'\iota': 'iota', r'\kappa': 'kappa', r'\lambda': 'lambda', r'\mu': 'mu',
            r'\nu': 'nu', r'\xi': 'xi', r'\omicron': 'omicron', r'\pi': 'pi',
            r'\rho': 'rho', r'\sigma': 'sigma', r'\tau': 'tau', r'\upsilon': 'upsilon',
            r'\phi': 'phi', r'\chi': 'chi', r'\psi': 'psi', r'\omega': 'omega',
            r'\Gamma': 'Gamma', r'\Delta': 'Delta', r'\Theta': 'Theta', r'\Lambda': 'Lambda',
            r'\Xi': 'Xi', r'\Pi': 'Pi', r'\Sigma': 'Sigma', r'\Upsilon': 'Upsilon',
            r'\Phi': 'Phi', r'\Psi': 'Psi', r'\Omega': 'Omega',

            # Mathematical Symbols
            r'\times': 'times', r'\div': 'divided by', r'\pm': 'plus-minus',
            r'\mp': 'minus-plus', r'\cdot': 'dot', r'\circ': 'circle',
            r'\bullet': 'bullet', r'\oplus': 'oplus', r'\ominus': 'ominus',
            r'\otimes': 'otimes', r'\oslash': 'oslash', r'\odot': 'odot',
            r'\leq': 'less than or equal to', r'\geq': 'greater than or equal to',
            r'\neq': 'not equal to', r'\approx': 'approximately equal to',
            r'\equiv': 'equivalent to', r'\propto': 'proportional to',
            r'\infty': 'infinity', r'\nabla': 'nabla', r'\partial': 'partial derivative',
            r'\int': 'integral', r'\sum': 'summation', r'\prod': 'product',

            # Physics Symbols
            r'\hbar': 'h-bar', r'\ell': 'ell', r'\wp': 'Weierstrass p',
            r'\Re': 'Real part', r'\Im': 'Imaginary part',

            # Other
            r'\%': '%', r'\&': '&', r'\_': '_',
        }

    def _create_db(self, style):
        db = get_default_latex_context_db()

        macros = []
        symbol_map = self._get_symbol_map()
        for k, v in symbol_map.items():
            macroname = k.strip('\\')
            if style == 'openai':
                macros.append(MacroTextSpec(macroname, simplify_repl=f'[{v}]'))
            elif style == 'screen_reader':
                macros.append(MacroTextSpec(macroname, simplify_repl=f' {v} '))

        if style == 'openai':
            macros.append(MacroTextSpec('frac', simplify_repl='(%(1)s)/(%(2)s)'))
            macros.append(MacroTextSpec('sqrt', simplify_repl='sqrt(%(2)s)'))
        elif style == 'screen_reader':
            macros.append(MacroTextSpec('frac', simplify_repl='fraction with numerator %(1)s and denominator %(2)s '))
            macros.append(MacroTextSpec('sqrt', simplify_repl='the square root of %(2)s'))

        db.add_context_category('custom-style', macros=macros, prepend=True)
        return db

    def ConvertToOpenAIFriendlyText(self, latex_input: str) -> str:
        db = self._create_db('openai')
        l2t = LatexNodes2Text(latex_context=db)
        text = l2t.latex_to_text(latex_input)
        text = re.sub(r'\^\((.+?)\)', r'^\1', text)
        text = re.sub(r'_\((.+?)\)', r'_\1', text)
        return text

    def ConvertToHumanFriendlyText(self, latex_input: str) -> str:
        l2t = LatexNodes2Text()
        text = l2t.latex_to_text(latex_input)

        # Post-processing for superscripts
        sup_map = {'0': '⁰', '1': '¹', '2': '²', '3': '³', '4': '⁴', '5': '⁵', '6': '⁶', '7': '⁷', '8': '⁸', '9': '⁹'}
        def sup_repl(m):
            return "".join(sup_map.get(c, c) for c in m.group(1))

        text = re.sub(r'\^\{(.*?)\}', sup_repl, text)
        text = re.sub(r'\^(\d)', lambda m: sup_map.get(m.group(1), m.group(1)), text)

        return text

    def ConvertToScreenReaderFriendlyText(self, latex_input: str) -> str:
        db = self._create_db('screen_reader')
        l2t = LatexNodes2Text(latex_context=db)
        text = l2t.latex_to_text(latex_input)

        # Post-processing for superscripts
        def sup_repl(m):
            content = m.group(1)
            if content == '2':
                return ' square'
            if content == '3':
                return ' cubed'
            return f' to the power of {content}'

        text = re.sub(r'\^\{(.*?)\}', sup_repl, text)
        text = re.sub(r'\^(\S)', sup_repl, text)

        return re.sub(r'\s+', ' ', text).strip()
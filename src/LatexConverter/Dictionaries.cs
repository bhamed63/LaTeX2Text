
using System;
using System.Collections.Generic;
using System.Linq;
using LatexConverter.Data;

namespace LatexConverter
{
    public class SymbolDefinition
    {
        public string? PlainText { get; set; }
        public string? ScreenReader { get; set; }
        //public string? ScreenReaderTemplate { get; set; }
        public string? ExceptionalScreenReader { get; set; }
        public string? HumanFriendly { get; set; }
        public string? HumanFriendlyKey { get; set; }
        //public string? HumanFriendlyTemplate { get; set; }
        public string? OpenAI { get; set; }
    }

    public record FontCharacter(char BaseChar, string FontCommand, string UnicodeChar);
    public record ScriptCharacter(char BaseChar, char? Superscript, char? Subscript);
    public record OperatorMapping(string openAiFriendly, string humanFriendly, string screenReaderFriendly, string screenReaderFriendlySuperscript);

    /// <summary>
    /// Provides centralized storage for symbol and character mappings used in LaTeX conversion.
    /// </summary>
    public static class Dictionaries
    {
        static Dictionaries()
        {
            SymbolMap = Data.RawData.SymbolLibrary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.PlainText!);
            ScreenReaderSymbolMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.ScreenReader != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReader!);
            ExceptionalScreenReaderSymbolMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.ExceptionalScreenReader != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ExceptionalScreenReader!);
            HumanFriendlySymbolMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendly!);
            //ReverseHumanFriendlySymbolMap = HumanFriendlySymbolMap.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.First().Key.Substring(1));


            ReverseHumanFriendlySymbolMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null)
                .GroupBy(kvp => (kvp.Value.HumanFriendlyKey ?? kvp.Value.HumanFriendly)!)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.First().Key.Substring(1));

            ScreenReaderTemplateMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.ScreenReader != null && kvp.Value.ScreenReader.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ScreenReader!);

            HumanFriendlyTemplateMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.HumanFriendly != null && kvp.Value.HumanFriendly.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HumanFriendly!);

            OpenAITemplateMap = Data.RawData.SymbolLibrary
                .Where(kvp => kvp.Value.OpenAI != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OpenAI!);

            ScreenReaderOperatorMap = Data.RawData.OperatorMap
                .Where(kvp => kvp.Value.screenReaderFriendly != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.screenReaderFriendly!);

            ScreenReaderOperatorSuperscriptTemplateMap = Data.RawData.OperatorMap
                .Where(kvp => kvp.Value.screenReaderFriendlySuperscript != null && kvp.Value.screenReaderFriendlySuperscript.Contains("{0}"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.screenReaderFriendlySuperscript!);

            MathbbMap = Data.RawData.FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathbb)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathcalMap = Data.RawData.FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathcal)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathfrakMap = Data.RawData.FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathfrak)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            MathscrMap = Data.RawData.FontLibrary
                .Where(fc => fc.FontCommand == CommandNames.Mathscr)
                .ToDictionary(fc => fc.BaseChar, fc => fc.UnicodeChar);

            ReverseMathFontMap = Data.RawData.FontLibrary
                .GroupBy(fc => fc.UnicodeChar)
                .ToDictionary(g => g.Key, g => $"{g.First().FontCommand}{{{g.First().BaseChar}}}");

            SupMap = Data.RawData.ScriptLibrary
                .Where(sc => sc.Superscript.HasValue)
                .ToDictionary(sc => sc.BaseChar, sc => sc.Superscript!.Value);

            SubMap = Data.RawData.ScriptLibrary
                .Where(sc => sc.Subscript.HasValue)
                .ToDictionary(sc => sc.BaseChar, sc => sc.Subscript!.Value);

            ReverseSupMap = Data.RawData.ScriptLibrary
                .Where(sc => sc.Superscript.HasValue)
                .ToDictionary(sc => sc.Superscript!.Value, sc => sc.BaseChar);

            ReverseSubMap = Data.RawData.ScriptLibrary
                .Where(sc => sc.Subscript.HasValue)
                .ToDictionary(sc => sc.Subscript!.Value, sc => sc.BaseChar);

            DeniedConvertWithoutSlash = Data.RawData.DeniedConvertWithoutSlash;
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
        public static readonly Dictionary<string, string> ScreenReaderOperatorMap;

        /// <summary>
        /// A map of LaTeX commands to their screen reader-friendly template strings.
        /// </summary>
        public static readonly Dictionary<string, string> ScreenReaderOperatorSuperscriptTemplateMap;

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
        public static readonly List<string> DeniedConvertWithoutSlash;

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
        public static readonly Dictionary<char, string> MathbbMap;
        /// <summary>
        /// A map of characters to their math calligraphic Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathcalMap;

        /// <summary>
        /// A map of characters to their math Fraktur Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathfrakMap;

        /// <summary>
        /// A map of characters to their math script Unicode representations.
        /// </summary>
        public static readonly Dictionary<char, string> MathscrMap;

        public static readonly Dictionary<string, string> ReverseMathFontMap;
    }
}

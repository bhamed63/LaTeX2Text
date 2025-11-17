using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LatexConverter.Data
{
    internal static class DataLoader
    {
        public static void LoadDataFromExcel(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // As per requirements, fail silently if the file doesn't exist.
                return;
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    LoadFontLibrary(package.Workbook.Worksheets["FontLibrary"]);
                    LoadScriptLibrary(package.Workbook.Worksheets["ScriptLibrary"]);
                    LoadSymbolLibrary(package.Workbook.Worksheets["SymbolLibrary"]);
                    LoadOperatorMap(package.Workbook.Worksheets["OperatorMap"]);
                    LoadDeniedConvertWithoutSlash(package.Workbook.Worksheets["DeniedConvertWithoutSlash"]);
                }
            }
            catch (Exception ex)
            {
                // As per requirements, throw a specific exception for format errors.
                throw new InvalidDataException("The provided Excel file is not in the correct format.", ex);
            }
        }

        private static void LoadFontLibrary(ExcelWorksheet worksheet)
        {
            if (worksheet == null) return;

            var fontLibrary = RawData.FontLibrary.ToDictionary(f => (f.BaseChar, f.FontCommand));

            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var character = worksheet.Cells[row, 1].Value?.ToString()?[0] ?? default;
                var command = worksheet.Cells[row, 2].Value?.ToString();
                var unicodeCharacter = worksheet.Cells[row, 3].Value?.ToString();

                if (character == default || string.IsNullOrEmpty(command) || string.IsNullOrEmpty(unicodeCharacter))
                {
                    continue;
                }

                var key = (character, command);
                var newFont = new FontCharacter(character, command, unicodeCharacter);

                if (fontLibrary.ContainsKey(key))
                {
                    fontLibrary[key] = newFont;
                }
                else
                {
                    fontLibrary.Add(key, newFont);
                }
            }
            RawData.FontLibrary.Clear();
            RawData.FontLibrary.AddRange(fontLibrary.Values);
        }

        private static void LoadScriptLibrary(ExcelWorksheet worksheet)
        {
            if (worksheet == null) return;

            var scriptLibrary = RawData.ScriptLibrary.ToDictionary(s => s.BaseChar);

            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var character = worksheet.Cells[row, 1].Value?.ToString()?[0] ?? default;
                var superscriptStr = worksheet.Cells[row, 2].Value?.ToString();
                var subscriptStr = worksheet.Cells[row, 3].Value?.ToString();

                if (character == default)
                {
                    continue;
                }

                var newScript = new ScriptCharacter(
                    character,
                    string.IsNullOrEmpty(superscriptStr) ? null : superscriptStr[0],
                    string.IsNullOrEmpty(subscriptStr) ? null : subscriptStr[0]
                );

                if (scriptLibrary.ContainsKey(character))
                {
                    scriptLibrary[character] = newScript;
                }
                else
                {
                    scriptLibrary.Add(character, newScript);
                }
            }
            RawData.ScriptLibrary.Clear();
            RawData.ScriptLibrary.AddRange(scriptLibrary.Values);
        }

        private static void LoadSymbolLibrary(ExcelWorksheet worksheet)
        {
            if (worksheet == null) return;

            var symbolLibrary = RawData.SymbolLibrary;

            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var key = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrEmpty(key)) continue;

                var typeString = worksheet.Cells[row, 8].Value?.ToString();
                var sanitizedType = string.IsNullOrEmpty(typeString) ? "Unknown" : Utilities.SanitizeCommandType(typeString);

                var symbol = new SymbolDefinition
                {
                    PlainText = worksheet.Cells[row, 2].Value?.ToString(),
                    ScreenReader = worksheet.Cells[row, 3].Value?.ToString(),
                    HumanFriendly = worksheet.Cells[row, 4].Value?.ToString(),
                    HumanFriendlyKey = worksheet.Cells[row, 5].Value?.ToString(),
                    OpenAI = worksheet.Cells[row, 6].Value?.ToString(),
                    ExceptionalScreenReader = worksheet.Cells[row, 7].Value?.ToString(),
                    CommandType = Enum.TryParse<CommandType>(sanitizedType, true, out var type) ? type : CommandType.Unknown,
                    ArgsNumber = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var num) ? num : 0
                };

                if (symbolLibrary.ContainsKey(key))
                {
                    symbolLibrary[key] = symbol;
                }
                else
                {
                    symbolLibrary.Add(key, symbol);
                }
            }
        }

        private static void LoadOperatorMap(ExcelWorksheet worksheet)
        {
            if (worksheet == null) return;

            var operatorMap = RawData.OperatorMap;

            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var key = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrEmpty(key)) continue;

                var op = new OperatorMapping(
                    openAiFriendly: worksheet.Cells[row, 2].Value?.ToString() ?? "",
                    humanFriendly: worksheet.Cells[row, 3].Value?.ToString() ?? "",
                    screenReaderFriendly: worksheet.Cells[row, 4].Value?.ToString() ?? "",
                    screenReaderFriendlySuperscript: worksheet.Cells[row, 5].Value?.ToString() ?? ""
                );

                if (operatorMap.ContainsKey(key))
                {
                    operatorMap[key] = op;
                }
                else
                {
                    operatorMap.Add(key, op);
                }
            }
        }

        private static void LoadDeniedConvertWithoutSlash(ExcelWorksheet worksheet)
        {
            if (worksheet == null) return;

            var deniedList = RawData.DeniedConvertWithoutSlash.ToHashSet();

            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var command = worksheet.Cells[row, 1].Value?.ToString();
                if (string.IsNullOrEmpty(command)) continue;

                deniedList.Add(command);
            }
            RawData.DeniedConvertWithoutSlash.Clear();
            RawData.DeniedConvertWithoutSlash.AddRange(deniedList);
        }
    }
}

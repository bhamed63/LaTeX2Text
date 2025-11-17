using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExcelDataReader;
using LatexConverter;
using LatexConverter.Data;
using OfficeOpenXml;

class Program
{
    private static readonly string OutputDirectory = "sample-xml-files";
    private static readonly string XmlFilePath = "G:\\Minihub\\ContentBackup2025124_155117\\AllProbSol";
    private static readonly string SecondXmlFilePath = "G:\\Minihub\\ContentBackup2025124_155117\\AllProbs";
    private static readonly string ExcelFilePath = "G:\\Minihub\\Projects\\open-ai-project-backend\\test_project\\FilesCreated\\f004093e-0158-4ce8-a0a0-fe944ef531f0.xlsx";
    private static readonly Regex LatexRegex = new Regex(@"\\([a-zA-Z]+|.)");

    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--export")
        {
            ExportDataToExcel();
        }
        else
        {
            ProcessXmlFiles();
        }
    }

    private static void ExportDataToExcel()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputFileName = $"latex_data_{timestamp}.xlsx";
        var outputPath = Path.Combine(OutputDirectory, outputFileName);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var descriptionSheet = package.Workbook.Worksheets.Add("Description");
            descriptionSheet.Cells["A1"].Value = "Sheet";
            descriptionSheet.Cells["B1"].Value = "Description";

            descriptionSheet.Cells["A2"].Value = "FontLibrary";
            descriptionSheet.Cells["B2"].Value = "Contains mappings for different font styles like mathbb, mathcal, etc.";
            descriptionSheet.Cells["A3"].Value = "ScriptLibrary";
            descriptionSheet.Cells["B3"].Value = "Contains mappings for superscript and subscript characters.";
            descriptionSheet.Cells["A4"].Value = "SymbolLibrary";
            descriptionSheet.Cells["B4"].Value = "Contains definitions for LaTeX symbols and their conversions.";
            descriptionSheet.Cells["A5"].Value = "OperatorMap";
            descriptionSheet.Cells["B5"].Value = "Contains mappings for operators like +, -, etc.";
            descriptionSheet.Cells["A6"].Value = "DeniedConvertWithoutSlash";
            descriptionSheet.Cells["B6"].Value = "Contains a list of commands that are not converted from plain text to LaTeX without a leading slash.";

            var fontLibrarySheet = package.Workbook.Worksheets.Add("FontLibrary");
            fontLibrarySheet.Cells["A1"].Value = "BaseChar";
            fontLibrarySheet.Cells["B1"].Value = "FontCommand";
            fontLibrarySheet.Cells["C1"].Value = "UnicodeChar";
            var row = 2;
            foreach (var font in RawData.FontLibrary)
            {
                fontLibrarySheet.Cells[$"A{row}"].Value = font.BaseChar;
                fontLibrarySheet.Cells[$"B{row}"].Value = font.FontCommand;
                fontLibrarySheet.Cells[$"C{row}"].Value = font.UnicodeChar;
                row++;
            }

            var scriptLibrarySheet = package.Workbook.Worksheets.Add("ScriptLibrary");
            scriptLibrarySheet.Cells["A1"].Value = "BaseChar";
            scriptLibrarySheet.Cells["B1"].Value = "Superscript";
            scriptLibrarySheet.Cells["C1"].Value = "Subscript";
            row = 2;
            foreach (var script in RawData.ScriptLibrary)
            {
                scriptLibrarySheet.Cells[$"A{row}"].Value = script.BaseChar;
                scriptLibrarySheet.Cells[$"B{row}"].Value = script.Superscript;
                scriptLibrarySheet.Cells[$"C{row}"].Value = script.Subscript;
                row++;
            }

            var symbolLibrarySheet = package.Workbook.Worksheets.Add("SymbolLibrary");
            symbolLibrarySheet.Cells["A1"].Value = "Key";
            symbolLibrarySheet.Cells["B1"].Value = "PlainText";
            symbolLibrarySheet.Cells["C1"].Value = "ScreenReader";
            symbolLibrarySheet.Cells["D1"].Value = "HumanFriendly";
            symbolLibrarySheet.Cells["E1"].Value = "HumanFriendlyKey";
            symbolLibrarySheet.Cells["F1"].Value = "OpenAI";
            symbolLibrarySheet.Cells["G1"].Value = "ExceptionalScreenReader";
            symbolLibrarySheet.Cells["H1"].Value = "CommandType";
            symbolLibrarySheet.Cells["I1"].Value = "ArgsNumber";

            row = 2;
            foreach (var symbol in RawData.SymbolLibrary)
            {
                symbolLibrarySheet.Cells[$"A{row}"].Value = symbol.Key;
                symbolLibrarySheet.Cells[$"B{row}"].Value = symbol.Value.PlainText;
                symbolLibrarySheet.Cells[$"C{row}"].Value = symbol.Value.ScreenReader;
                symbolLibrarySheet.Cells[$"D{row}"].Value = symbol.Value.HumanFriendly;
                symbolLibrarySheet.Cells[$"E{row}"].Value = symbol.Value.HumanFriendlyKey;
                symbolLibrarySheet.Cells[$"F{row}"].Value = symbol.Value.OpenAI;
                symbolLibrarySheet.Cells[$"G{row}"].Value = symbol.Value.ExceptionalScreenReader;
                symbolLibrarySheet.Cells[$"H{row}"].Value = symbol.Value.CommandType.ToString();
                symbolLibrarySheet.Cells[$"I{row}"].Value = symbol.Value.ArgsNumber;
                row++;
            }

            var operatorMapSheet = package.Workbook.Worksheets.Add("OperatorMap");
            operatorMapSheet.Cells["A1"].Value = "Key";
            operatorMapSheet.Cells["B1"].Value = "OpenAIFriendly";
            operatorMapSheet.Cells["C1"].Value = "HumanFriendly";
            operatorMapSheet.Cells["D1"].Value = "ScreenReaderFriendly";
            operatorMapSheet.Cells["E1"].Value = "ScreenReaderFriendlySuperscript";
            row = 2;
            foreach (var op in RawData.OperatorMap)
            {
                operatorMapSheet.Cells[$"A{row}"].Value = op.Key;
                operatorMapSheet.Cells[$"B{row}"].Value = op.Value.openAiFriendly;
                operatorMapSheet.Cells[$"C{row}"].Value = op.Value.humanFriendly;
                operatorMapSheet.Cells[$"D{row}"].Value = op.Value.screenReaderFriendly;
                operatorMapSheet.Cells[$"E{row}"].Value = op.Value.screenReaderFriendlySuperscript;
                row++;
            }

            var deniedConvertSheet = package.Workbook.Worksheets.Add("DeniedConvertWithoutSlash");
            deniedConvertSheet.Cells["A1"].Value = "Command";
            row = 2;
            foreach (var command in RawData.DeniedConvertWithoutSlash)
            {
                deniedConvertSheet.Cells[$"A{row}"].Value = command;
                row++;
            }

            package.SaveAs(new FileInfo(outputPath));
        }
        Console.WriteLine($"\nData exported to {Path.GetFullPath(outputPath)}");
    }

    private static void ProcessXmlFiles()
    {
        if (!Directory.Exists(XmlFilePath))
        {
            Console.WriteLine($"Directory not found: {Path.GetFullPath(XmlFilePath)}");
            return;
        }

        if (!Directory.Exists(SecondXmlFilePath))
        {
            Console.WriteLine($"Directory not found: {Path.GetFullPath(SecondXmlFilePath)}");
            return;
        }

        Console.WriteLine("Found unique LaTeX commands from XML files:");

        try
        {
            var filePaths = Directory.GetFiles(XmlFilePath, "*.xml", SearchOption.AllDirectories);
            var foundCommands = new HashSet<string>();

            foreach (var filePath in filePaths)
            {
                var text = File.ReadAllText(filePath);
                ProcessText(new List<string> { text }, foundCommands);
            }

            filePaths = Directory.GetFiles(SecondXmlFilePath, "*.xml", SearchOption.AllDirectories);
            foundCommands = new HashSet<string>();

            foreach (var filePath in filePaths)
            {
                var text = File.ReadAllText(filePath);
                ProcessText(new List<string> { text }, foundCommands);
            }

            SaveReport(foundCommands);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing files: {ex.Message}");
        }
    }

    private static void ProcessExcelFile()
    {
        if (!File.Exists(ExcelFilePath))
        {
            Console.WriteLine($"File not found: {Path.GetFullPath(ExcelFilePath)}");
            return;
        }

        Console.WriteLine("Found unique LaTeX commands from Excel file:");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            var foundCommands = new HashSet<string>();
            using (var stream = File.Open(ExcelFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // Skip header row
                reader.Read();

                while (reader.Read()) // Read one row at a time
                {
                    var cellValue = reader.GetValue(0)?.ToString(); // Get value from first column
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        ProcessText(new List<string> { cellValue }, foundCommands);
                    }
                }
            }

            SaveReport(foundCommands);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the Excel file: {ex.Message}");
        }
    }

    private static HashSet<string> ProcessText(IEnumerable<string> texts, HashSet<string> existingCommands = null)
    {
        var latexCommands = existingCommands ?? new HashSet<string>();
        foreach (var text in texts)
        {
            if (string.IsNullOrEmpty(text)) continue;
            var matches = LatexRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (latexCommands.Add(match.Value))
                {
                    Console.WriteLine(match.Value);
                }
            }
        }
        return latexCommands;
    }

    private static void SaveReport(HashSet<string> commands)
    {
        var sortedCommands = commands.ToList();
        sortedCommands.Sort();

        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputFileName = $"found_commands_{timestamp}.txt";
        var outputPath = Path.Combine(OutputDirectory, outputFileName);

        File.WriteAllLines(outputPath, sortedCommands);
        Console.WriteLine($"\nResults saved to {Path.GetFullPath(outputPath)}");
    }
}

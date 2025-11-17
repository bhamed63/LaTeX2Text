using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExcelDataReader;

class Program
{
    private static readonly string OutputDirectory = "sample-xml-files";
    private static readonly string XmlFilePath = "G:\\Minihub\\ContentBackup2025124_155117\\AllProbSol";
    private static readonly string SecondXmlFilePath = "G:\\Minihub\\ContentBackup2025124_155117\\AllProbs";
    private static readonly string ExcelFilePath = "G:\\Minihub\\Projects\\open-ai-project-backend\\test_project\\FilesCreated\\f004093e-0158-4ce8-a0a0-fe944ef531f0.xlsx";
    private static readonly Regex LatexRegex = new Regex(@"\\([a-zA-Z]+|.)");
    private static readonly HashSet<char> UsualChars = new HashSet<char>(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ,.?!\"'()[]{}<>:;-_=+|\\/"
    );

    static void Main(string[] args)
    {
        // To process Excel files, comment out the line below and uncomment the one after
        ProcessXmlFiles();

        //ProcessExcelFile();
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
            var commandCounts = new Dictionary<string, int>();

            var filePaths = Directory.GetFiles(XmlFilePath, "*.xml", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var text = File.ReadAllText(filePath);
                ProcessText(new List<string> { text }, commandCounts);
            }

            filePaths = Directory.GetFiles(SecondXmlFilePath, "*.xml", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var text = File.ReadAllText(filePath);
                ProcessText(new List<string> { text }, commandCounts);
            }

            SaveReport(commandCounts);
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
            var commandCounts = new Dictionary<string, int>();
            using (var stream = File.Open(ExcelFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // Skip header row
                reader.Read();
                int rowNum = 2; // Start from row 2 as we skipped the header

                while (reader.Read()) // Read one row at a time
                {
                    // Process LaTeX commands from the first column
                    var latexCellValue = reader.GetValue(0)?.ToString();
                    if (!string.IsNullOrEmpty(latexCellValue))
                    {
                        ProcessText(new List<string> { latexCellValue }, commandCounts);
                    }

                    // Process Unicode symbols from the second column
                    var unicodeCellValue = reader.GetValue(1)?.ToString();
                    if (!string.IsNullOrEmpty(unicodeCellValue))
                    {
                        int unconvertibleCount = 0;
                        foreach (char c in unicodeCellValue)
                        {
                            if (!UsualChars.Contains(c) && !LatexConverter.Dictionaries.ReverseHumanFriendlySymbolMap.ContainsKey(c.ToString()))
                            {
                                unconvertibleCount++;
                            }
                        }
                        if (unconvertibleCount > 0)
                        {
                            Console.WriteLine($"{unconvertibleCount} Unicode symbol(s) not converted in row: {rowNum}");
                        }
                    }
                    rowNum++;
                }
            }

            SaveReport(commandCounts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the Excel file: {ex.Message}");
        }
    }

    private static void ProcessText(IEnumerable<string> texts, Dictionary<string, int> commandCounts)
    {
        foreach (var text in texts)
        {
            if (string.IsNullOrEmpty(text)) continue;
            var matches = LatexRegex.Matches(text);
            foreach (Match match in matches)
            {
                var command = match.Value;
                if (!commandCounts.ContainsKey(command))
                {
                    commandCounts[command] = 0;
                    Console.WriteLine(command);
                }
                commandCounts[command]++;
            }
        }
    }

    private static void SaveReport(Dictionary<string, int> commands)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        // Alphabetical report
        var alphaSortedCommands = commands.OrderBy(kvp => kvp.Key)
                                          .Select(kvp => $"Count: {kvp.Value}\t, Command: {kvp.Key}");
        var alphaOutputFileName = $"found_commands_alpha_{timestamp}.txt";
        var alphaOutputPath = Path.Combine(OutputDirectory, alphaOutputFileName);
        File.WriteAllLines(alphaOutputPath, alphaSortedCommands);
        Console.WriteLine($"\nAlphabetical report saved to {Path.GetFullPath(alphaOutputPath)}");

        // Frequency report
        var freqSortedCommands = commands.OrderByDescending(kvp => kvp.Value)
                                         .ThenBy(kvp => kvp.Key)
                                         .Select(kvp => $"Count: {kvp.Value}\t, Command: {kvp.Key}");
        var freqOutputFileName = $"found_commands_freq_{timestamp}.txt";
        var freqOutputPath = Path.Combine(OutputDirectory, freqOutputFileName);
        File.WriteAllLines(freqOutputPath, freqSortedCommands);
        Console.WriteLine($"Frequency report saved to {Path.GetFullPath(freqOutputPath)}");
    }
}

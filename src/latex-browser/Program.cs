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
    private static readonly string XmlFilePath = "sample-xml-files";
    private static readonly string ExcelFilePath = "sample-xml-files/sample.xlsx";
    private static readonly Regex LatexRegex = new Regex(@"\\([a-zA-Z]+|.)");

    static void Main(string[] args)
    {
        // To process Excel files, comment out the line below and uncomment the one after
        ProcessXmlFiles();

        // ProcessExcelFile();
    }

    private static void ProcessXmlFiles()
    {
        if (!Directory.Exists(XmlFilePath))
        {
            Console.WriteLine($"Directory not found: {Path.GetFullPath(XmlFilePath)}");
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

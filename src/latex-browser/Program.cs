using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    private static readonly string FilePath = "sample-xml-files";

    static void Main(string[] args)
    {
        if (!Directory.Exists(FilePath))
        {
            Console.WriteLine($"Directory not found: {Path.GetFullPath(FilePath)}");
            Console.WriteLine("Please create the directory and add some XML files with LaTeX content.");
            return;
        }

        var latexCommands = new HashSet<string>();
        var latexRegex = new Regex(@"\\([a-zA-Z]+|.)");

        Console.WriteLine("Found unique LaTeX commands:");

        try
        {
            var files = Directory.GetFiles(FilePath, "*.xml", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var content = File.ReadAllText(file);
                    var matches = latexRegex.Matches(content);

                    foreach (Match match in matches)
                    {
                        if (latexCommands.Add(match.Value))
                        {
                            Console.WriteLine(match.Value);
                        }
                    }
                }
                catch (IOException)
                {
                    // Silently skip files that cannot be read
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing files: {ex.Message}");
            return;
        }
    }
}

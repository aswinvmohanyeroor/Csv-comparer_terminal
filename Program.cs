using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleFolderComparator
{
    class Program
    {
        static void Main()
        {

            // Get folder1 path from the user
            Console.Write("Enter the path of the first folder: ");
            string firstFolderPath = Console.ReadLine();

            // Get folder2 path from the user
            Console.Write("Enter the path of the second folder: ");
            string secondFolderPath = Console.ReadLine();

            // Create a folder comparator instance and compare folders
            FolderComparator comparator = new FolderComparator(firstFolderPath, secondFolderPath);

            // Allow the user to specify the output path and filename
            // Specify filename too
            Console.Write("Enter the path to save the result CSV file: ");
            string outputFilePath = Console.ReadLine();

            comparator.CompareFolders(outputFilePath);

            Console.WriteLine("Comparison completed. Press any key to exit.");
            Console.ReadKey();
        }
    }

    class FolderComparator
    {
        private readonly string firstFolderPath;
        private readonly string secondFolderPath;

        public FolderComparator(string firstFolderPath, string secondFolderPath)
        {
            this.firstFolderPath = firstFolderPath;
            this.secondFolderPath = secondFolderPath;
        }

        public void CompareFolders(string outputFilePath)
        {
            if (!Directory.Exists(firstFolderPath) || !Directory.Exists(secondFolderPath))
            {
                Console.WriteLine("One or both folders do not exist.");
                return;
            }

            string[] firstFolderFiles = Directory.GetFiles(firstFolderPath);
            int totalFiles = firstFolderFiles.Length;
            int filesProcessed = 0;

            List<string> differences = new List<string>(); // Store differences

            Console.WriteLine("Comparing folders...");
            Console.WriteLine("Progress:");

            foreach (string firstFilePath in firstFolderFiles)
            {
                string fileName = Path.GetFileName(firstFilePath);
                string secondFilePath = Path.Combine(secondFolderPath, fileName);

                if (File.Exists(secondFilePath))
                {
                    bool hasDifferences = CompareFileContent(firstFilePath, secondFilePath, differences);
                    if (hasDifferences)
                    {
                        Console.WriteLine($"=> {fileName} (Different Content)");
                    }
                    else
                    {
                        Console.WriteLine($"=> {fileName} (Same Content)");
                    }
                }
                else
                {
                    Console.WriteLine($"=> {fileName} (File not in both folder's)");
                }

                // Update progress
                filesProcessed++;
                UpdateProgressBar(filesProcessed, totalFiles);
            }

            // Save differences to the user-specified CSV file
            File.WriteAllLines(outputFilePath, differences);
            Console.WriteLine($"Differences saved to {outputFilePath}");
        }

        private static bool CompareFileContent(string firstFilePath, string secondFilePath, List<string> differences)
        {
            string[] firstFileLines = File.ReadAllLines(firstFilePath);
            string[] secondFileLines = File.ReadAllLines(secondFilePath);

            bool hasDifferences = false;

            if (firstFileLines.Length != secondFileLines.Length)
            {
                // Lines count is different; add both files' content to differences
                differences.Add($"--- {Path.GetFileName(firstFilePath)} ---");
                differences.AddRange(firstFileLines);
                differences.Add($"--- {Path.GetFileName(secondFilePath)} ---");
                differences.AddRange(secondFileLines);
                hasDifferences = true;
            }
            else
            {
                for (int i = 0; i < firstFileLines.Length; i++)
                {
                    if (!firstFileLines[i].Equals(secondFileLines[i], StringComparison.OrdinalIgnoreCase))
                    {
                        // Lines content is different; add both lines to differences
                        differences.Add($"--- {Path.GetFileName(firstFilePath)}, Line {i + 1} ---");
                        differences.Add(firstFileLines[i]);
                        differences.Add($"--- {Path.GetFileName(secondFilePath)}, Line {i + 1} ---");
                        differences.Add(secondFileLines[i]);
                        hasDifferences = true;
                    }
                }
            }

            return hasDifferences;
        }

        private static void UpdateProgressBar(int completed, int total)
        {
            Console.CursorLeft = 0;
            int barLength = Console.WindowWidth - 2;
            int progress = completed * barLength / total;
            Console.Write("[");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(new string(' ', progress));
            Console.ResetColor();
            Console.Write(new string(' ', barLength - progress) + "]");

        }
    }
}
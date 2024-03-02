using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVConverter
{
    sealed class Program
    {
        static bool RowHasData (List<string> cells)
        {
            return cells.Any(x => x.Length > 0);
        }

        static string CropString(string input, int startIndex, int length)
        {
            // Check if the startIndex is within the bounds of the string
            if (startIndex < 0 || startIndex >= input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Invalid startIndex");
            }

            // Adjust the length to avoid going beyond the end of the string
            length = Math.Min(length, input.Length - startIndex);

            // Use Substring to crop the string
            string cropped = input.Substring(startIndex, length);

            return cropped;
        }

        static void WriteRow(StreamWriter writer, string[] rowData)
        {
            // Calculate the maximum column width based on the longest string in each column
            int[] columnWidths = new int[rowData.Length];
            for (int i = 0; i < rowData.Length; i++)
            {
                columnWidths[i] = Math.Max(columnWidths[i], rowData[i].Length);
            }

            // Write each column with equal spacing
            for (int i = 0; i < rowData.Length; i++)
            {
                writer.Write(rowData[i].PadRight(columnWidths[i] + 2)); // Add padding for spacing
            }

            writer.WriteLine(); // Move to the next line after writing a row
        }

        static void WriteToFile(string filePath, string[] header, string[] values)
        {
            // Create or overwrite the text file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write header
                WriteRow(writer, header);

                // Write values
                WriteRow(writer, values);
            }
        }

        private static void WarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Operation(string[] retrievedCSVFiles, string flag)
        {
            foreach (var file in retrievedCSVFiles)
            {
                var csvFile = new CsvFile(file, flag);
                if (csvFile.IsConvertable())
                {
                    if (!csvFile.IsConvertedAndSaved())
                    {
                        WarningMessage($"Couldn't convert {file}");
                        //WarningMessage($"Please check {file}");
                    }
                }
                else
                {
                    WarningMessage($"Couldn't convert {file}");
                    //WarningMessage($"Please check {file}");
                }
            }
        }

        static void Main(string[] args)
        {
            {
                /*
            var csvPath = @"B:\Raw_Data\23110302.CSV";

            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.Worksheets.Clear();
            Spire.Xls.Worksheet worksheet = workbook.Worksheets.Add("23110302");

            
            List<string> allrows = new List<string>();
            using (var reader = new StreamReader(csvPath, System.Text.Encoding.UTF7))
            {
                while(reader.EndOfStream == false)
                {
                    var content = reader.ReadLine();
                    allrows.Add(content);
                }
            }

            for (int i = 0; i < allrows.Count; i++)
            {
                var cells = allrows[i].Split(';').ToList();
                for(int j = 0; j < cells.Count; j++)
                {
                    if (cells[j].Contains(","))
                    {
                        if (char.IsDigit(cells[j][(cells[j].IndexOf(',') - 1)]) && char.IsDigit(cells[j][(cells[j].IndexOf(',') + 1)]))
                        {
                            worksheet.Range[i + 1, j + 1].Text = cells[j].Replace(',', '.');
                        }
                        else
                        {
                            worksheet.Range[i + 1, j + 1].Text = cells[j];
                        }
                    }
                    else
                    {
                        worksheet.Range[i + 1, j + 1].Text = cells[j];
                    }
                }
            }

            worksheet.AllocatedRange.AutoFitColumns();
            workbook.SaveToFile(@"B:\23110302.xlsx", ExcelVersion.Version2016);
            */
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            if (!(args.Length == 2))
            {
                WarningMessage("Enter the correct .csv file folder and .csv file type.");
                return;
            }

            var retrievedCSVFiles = Directory.GetFiles(args[0], "*.csv");

            if (retrievedCSVFiles.Length == 0)
            {
                WarningMessage($"Couldn't find any .csv file in {args[0]}");
                return;
            }


            switch (args[1])
            {
                case "1":
                    Operation(retrievedCSVFiles, "1");
                    break; 
                case "2":
                    Operation(retrievedCSVFiles, "2");
                    break;
                default:
                    WarningMessage($"Couldn't find any correct .csv file type. {args[1]} is not correct file type.");
                    return;
            }

            if (Directory.Exists(args[0] + @"\Konvertierte_Dateien"))
            {
                if (Directory.GetFiles(args[0] + @"\Konvertierte_Dateien", "*.csv").Length > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("Converted CSV files are located in " + args[0] + @"\Konvertierte_Dateien");
                    Console.WriteLine("Press any key to exit.");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}

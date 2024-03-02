using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSVConverter
{
    internal class CsvFile
    {
        private readonly string _csvFile, _flag;
        private readonly List<string> _rawDataFromCsvFile = new List<string>();

        private readonly List<string> _headerOfColumns = new List<string>();
        private readonly Dictionary<string, List<string>> _completeCsvFileContents = new Dictionary<string, List<string>>();

        private bool _GetRawDataFromCSVFile(string file)
        {
            try
            {
                using (var reader = new StreamReader(file, System.Text.Encoding.UTF7))
                {
                    while (reader.EndOfStream == false)
                    {
                        //if (reader.ReadLine().ToString().Contains(';'))
                        //{
                        _rawDataFromCsvFile.Add(reader.ReadLine());
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string _CropString(string input, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Invalid startIndex");
            }
            length = Math.Min(length, input.Length - startIndex);

            string cropped = input.Substring(startIndex, length);

            return cropped;
        }

        private bool _GetCompleteCSVFileContents()
        {
            for(var i = 0; i < _headerOfColumns.Count; i++)
            {
                var tempValueList = new List<string>();
                for(var j = 1; j < _rawDataFromCsvFile.Count; j++)
                {
                    var row = _rawDataFromCsvFile[j].Split(';').ToList();
                    if (row.Count != _headerOfColumns.Count)
                    {
                        return false;
                    }
                    else
                    {                
                        //Error handling
                       
                        if (row[i].Contains(","))
                        {
                            try
                            {
                                if (char.IsDigit(row[i][(row[i].IndexOf(',') - 1)]) && char.IsDigit(row[i][(row[i].IndexOf(',') + 1)]))
                                {
                                    //tempValueList.Add(row[i].Replace(',', '.'));
                                    tempValueList.Add($"\"{row[i].Replace(',', '.')}\"");
                                }
                                else
                                {
                                    tempValueList.Add($"\"{row[i]}\"");
                                }
                            }
                            catch
                            {
                                try
                                {
                                    if (char.IsDigit(row[i][(row[i].IndexOf(',') + 1)]))
                                    {
                                        //tempValueList.Add(row[i].Replace(',', '.'));
                                        tempValueList.Add($"\"{row[i].Replace(',', '.')}\"");
                                    }
                                    else
                                    {
                                        tempValueList.Add($"\"{row[i]}\"");
                                    }
                                }
                                catch
                                {
                                    tempValueList.Add($"\"{row[i]}\"");
                                }
                                
                            }
                        }
                        else
                        {
                            //tempValueList.Add(row[i]);
                            tempValueList.Add($"\"{row[i]}\"");
                        }

                        //Error handling
                    }
                }
                _completeCsvFileContents.Add(_headerOfColumns[i], tempValueList);
            }
            if (_completeCsvFileContents.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public CsvFile(string file, string fileType)
        {
            _csvFile = file;
            _flag = fileType;
        }

        public bool IsConvertable()
        {
            if (!_GetRawDataFromCSVFile(_csvFile))
            {
                return false;
            }

            if (_rawDataFromCsvFile[0].Split(';').ToList().Count > 0)
            {
                foreach (var header in _rawDataFromCsvFile[0].Split(';').ToList())
                {
                    _headerOfColumns.Add(header);
                }
            }
            else
            {
                return false;
            }


            if (!_GetCompleteCSVFileContents())
            {
                return false;
            }

            if (_flag == "2")
            {
                if (_headerOfColumns.Contains("Text_1") && _headerOfColumns.Contains("Text_4") && _headerOfColumns.Contains("Text_5") && _headerOfColumns.Contains("Text_6") &&
                    _headerOfColumns.Contains("Text_7") && _headerOfColumns.Contains("Text_8") && _headerOfColumns.Contains("Text_9") && _headerOfColumns.Contains("Text_11") &&
                    _headerOfColumns.Contains("Text_12") && _headerOfColumns.Contains("Text_13") && _headerOfColumns.Contains("Text_14") && _headerOfColumns.Contains("Text_15") &&
                    _headerOfColumns.Contains("AuftrNr"))
                {
                         
                    try
                    {
                        _ConvertCSVFile();
                        //if (!(_completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("AuswArt")]].Count > 0))
                        //{
                        //    return false;
                        //}
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> _EmptyValues(string key, Dictionary<string, List<string>> csvFile)
        {
            var temp = new List<string>();

            for(var i = 0; i < csvFile[key].Count; i++)
            {
                temp.Add("");
            }
            return temp;
        }


        private void _ConvertCSVFile()
        {
            //V->Text_15 => L->Text_5
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_5")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_15")]];
            //S->Text_12 => U->Text_14
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_14")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_12")]];
            //R->Text_11 => T->Text_13
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_13")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_11")]];
            //P->Text_9 => T->Text_12
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_12")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_9")]];
            //N->Text_7 => N->keine Einträge
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_7")]] = _EmptyValues(_headerOfColumns[_headerOfColumns.IndexOf("Text_7")], _completeCsvFileContents);
            //O->Text_8 => O->keine Einträge
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_8")]] = _EmptyValues(_headerOfColumns[_headerOfColumns.IndexOf("Text_8")], _completeCsvFileContents);
            //M->Text_6 => P->Text_9
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_9")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_6")]];
            //K->Text_4 => V->Text_15
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_15")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_4")]];
            //H->Text_1 => B->AuftrNr
            _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("AuftrNr")]] = _completeCsvFileContents[_headerOfColumns[_headerOfColumns.IndexOf("Text_1")]];
        }

        private void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public bool IsConvertedAndSaved()
        {
            try
            {
                if (!Directory.Exists(_CropString(_csvFile, 0, _csvFile.LastIndexOf(@"\")) + @"\Konvertierte_Dateien"))
                {
                    Directory.CreateDirectory(_CropString(_csvFile, 0, _csvFile.LastIndexOf(@"\")) + @"\Konvertierte_Dateien");
                }

                using (var writer = new StreamWriter(_CropString(_csvFile, 0, _csvFile.LastIndexOf(@"\")) + @"\Konvertierte_Dateien" + _CropString(_csvFile, _csvFile.LastIndexOf(@"\"), _csvFile.Length - 1), false, Encoding.UTF8))
                {
                    for (var i = 0; i < _headerOfColumns.Count; i++)
                    {
                        if (i != (_headerOfColumns.Count - 1))
                        {
                            writer.Write(_headerOfColumns[i] + ",");
                        }
                        else
                        {
                            writer.Write(_headerOfColumns[i]);
                        }
                    }
                    writer.WriteLine();

                    for (var i = 1; i < _rawDataFromCsvFile.Count; i++)
                    {
                        for (var j = 0; j < _completeCsvFileContents.Count; j++)
                        {
                            if (j != (_completeCsvFileContents.Count - 1))
                            {
                                writer.Write(_completeCsvFileContents.ElementAt(j).Value[i - 1] + ",");
                            }
                            else
                            {
                                writer.Write(_completeCsvFileContents.ElementAt(j).Value[i - 1]);
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch
            {
                Message(_CropString(_csvFile, 0, _csvFile.LastIndexOf(@"\")) + @"\Konvertierte_Dateien" + _CropString(_csvFile, _csvFile.LastIndexOf(@"\"), _csvFile.Length - 1) + _CropString(_csvFile, _csvFile.LastIndexOf(@"\"), _csvFile.Length - 1) + " might be open already");
                return false;
            }


            return true;
        }
    }
}

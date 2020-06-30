namespace SearchInFileCSVLibrary
{
    using SearchInFileCSVLibrary.Interface;
    using SearchInFileCSVLibrary.Resource;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileWork : IFileWork
    {
        public FileWork()
        {
            Timer = new Stopwatch();
        }

        public Stopwatch Timer { get; private set; }

        public Encoding GetEncoding(string encoding)
        {
            var result = DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encoding).Value;
            return result;
        }

        public void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression)
        {
            var encoding = GetEncoding(encode);
            using (StreamReader sr = new StreamReader(pathFileIn, encoding))
            {
                var line = sr.ReadLine();
                Timer.Restart();
                var columnsNambers = ParseHeader(line, colName, expression);
                Timer.Stop();
                using (StreamWriter sw = new StreamWriter(pathFileOut, false, encoding))
                {
                    sw.WriteLine(line);
                }

                while ((line = sr.ReadLine()) != null)
                {
                    if (FindExpressionToRow(line, columnsNambers, expression))
                    {
                        using (StreamWriter sw = new StreamWriter(pathFileOut, true, encoding))
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }
        }

        public async void SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression)
        {
            await Task.Run(() => SearchInFileCSV(pathFileIn, pathFileOut, encode, colName, expression));
        }

        private int[] ParseHeader(string header, string colName, string expression)
        {
            var result = header.Split(';').AsParallel()
                                .Select((item, index) => new { Item = item, Index = index })
                                .Where(
                                    x =>
                                    {
                                        var column = x.Item.Trim().Split(' ');
                                        if (column[0].Trim() == colName)
                                        {
                                            switch (DictionaryLibrary.TypeExpressionDict.FirstOrDefault(y => y.Key == column[1].Trim()).Value)
                                            {
                                                case (byte)TypeExpressionEnum.StringExpression:
                                                    {
                                                        return true;
                                                    }
                                                case (byte)TypeExpressionEnum.DateTimeExpression:
                                                    {
                                                        try
                                                        {
                                                            Convert.ToDateTime(expression);
                                                            return true;
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                case (byte)TypeExpressionEnum.IntExpression:
                                                    {
                                                        try
                                                        {
                                                            Convert.ToInt32(expression);
                                                            return true;
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                case (byte)TypeExpressionEnum.FloatExpression:
                                                    {
                                                        try
                                                        {
                                                            Convert.ToSingle(expression);
                                                            return true;
                                                        }
                                                        catch (FormatException)
                                                        {
                                                            return false;
                                                        }
                                                    }
                                            }
                                        }

                                        return false;
                                    })
                                .Select(x => x.Index)
                                .ToArray();
            return result;
        }

        private List<string> ParseRow(string line, List<int> columnsNumbers)
        {
            throw new NotImplementedException();
        }

        private bool FindExpressionToRow(string line, int[] columnNumber, string expression)
        {
            var result = line.Split(';');
            var isFound = false;
            Timer.Restart();
            if (columnNumber.Length > 1000)
            {
                Parallel.ForEach(columnNumber, (item, loop) =>
                        {
                            if (result[item].Trim() == expression)
                            {
                                isFound = true;
                                loop.Break();
                            }
                        });
            }
            else
            {
                foreach (var item in columnNumber)
                {
                    if (result[item].Trim() == expression)
                    {
                        isFound = true;
                        break;
                    }
                }
            }
            Timer.Stop();
            return isFound;
        }
    }
}

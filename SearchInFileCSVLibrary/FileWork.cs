namespace SearchInFileCSVLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using SearchInFileCSVLibrary.Interface;
    using SearchInFileCSVLibrary.Resource;

    public class FileWork : IFileWork
    {
        private const char _delimeter = ';';
        private const char _delimeterColName = ' ';
        public FileWork()
        {
            Timer = new Stopwatch();
        }

        public Stopwatch Timer { get; private set; }

        public int CountColumns { get; private set; }

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
            Timer.Restart();
            var headers = header.Split(_delimeter);
            var result = headers.AsParallel()
                                .Select((item, index) => new { Item = item, Index = index })
                                .Where(
                                    x =>
                                    {
                                        var column = x.Item.Trim().Split(_delimeterColName);
                                        if (column[0].Trim() == colName)
                                        {
                                            try
                                            {
                                                switch (DictionaryLibrary.TypeExpressionDict.FirstOrDefault(y => y.Key == column[1].Trim()).Value)
                                                {
                                                    case (byte)TypeExpressionEnum.StringExpression:
                                                        {
                                                            return true;
                                                        }
                                                    case (byte)TypeExpressionEnum.DateTimeExpression:
                                                        {
                                                            Convert.ToDateTime(expression);
                                                            return true;
                                                        }
                                                    case (byte)TypeExpressionEnum.IntExpression:
                                                        {
                                                            Convert.ToInt32(expression);
                                                            return true;
                                                        }
                                                    case (byte)TypeExpressionEnum.FloatExpression:
                                                        {
                                                            Convert.ToSingle(expression);
                                                            return true;
                                                        }
                                                }
                                            }
                                            catch (FormatException)
                                            {
                                                return false;
                                            }
                                        }

                                        return false;
                                    })
                                .Select(x => x.Index)
                                .ToArray();
            Timer.Stop();
            CountColumns = headers.Length;
            return result;
        }

        private bool FindExpressionToRow(string line, int[] columnNumber, string expression)
        {
            Timer.Restart();
            var result = new Regex(@".(?:\u0022[^\u0022]* \u0022 |[^; \u0022])*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline).Matches(line);
            Timer.Stop();
            var isFound = false;
            Timer.Restart();
            if (columnNumber.Length > 1000)
            {
                Parallel.ForEach(columnNumber, (item, loop) =>
                        {
                            if (item < result.Count && result[item].Value.Trim().TrimStart(_delimeter) == expression)
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
                    if (item >= result.Count)
                    {
                        continue;
                    }

                    if (result[item].Value.Trim().TrimStart(_delimeter) == expression)
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

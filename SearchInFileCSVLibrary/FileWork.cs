namespace SearchInFileCSVLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ResourceLibrary;
    using SearchInFileCSVLibrary.Interface;

    public class FileWork : IFileWork
    {
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

        public void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken)
        {
            CanExecute(pathFileIn, pathFileOut, encode);
            var encoding = GetEncoding(encode);
            using (StreamReader sr = new StreamReader(pathFileIn, encoding))
            {
                var line = sr.ReadLine();
                Timer.Restart();
                var columnsNambers = ParseHeader(line, colName, expression, cancellationToken);
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
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
        }

        public async Task SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() => SearchInFileCSV(pathFileIn, pathFileOut, encode, colName, expression, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                throw new UserException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new UserException(ex.Message);
            }
        }

        private int[] ParseHeader(string header, string colName, string expression, CancellationToken cancellationToken = default)
        {
            Timer.Restart();
            var headers = header.Split(Constants.DELIMETER);
            var result = headers.AsParallel()
                                .WithCancellation(cancellationToken)
                                .Select((item, index) => new { Item = item, Index = index })
                                .Where(
                                    x =>
                                    {
                                        var column = x.Item.Trim().Split(Constants.DELIMETERCOLNAME);
                                        if (column[0].Trim() == colName)
                                        {
                                            try
                                            {
                                                switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Key == column[1].Trim()).Value)
                                                {
                                                    case (byte)TypeColumnEnum.StringColumn:
                                                        {
                                                            return true;
                                                        }
                                                    case (byte)TypeColumnEnum.DateTimeColumn:
                                                        {
                                                            Convert.ToDateTime(expression);
                                                            return true;
                                                        }
                                                    case (byte)TypeColumnEnum.IntColumn:
                                                        {
                                                            Convert.ToInt32(expression);
                                                            return true;
                                                        }
                                                    case (byte)TypeColumnEnum.FloatColumn:
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
                            if (item < result.Count && result[item].Value.Trim().TrimStart(Constants.DELIMETER) == expression)
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

                    if (result[item].Value.Trim().TrimStart(Constants.DELIMETER) == expression)
                    {
                        isFound = true;
                        break;
                    }
                }
            }
            Timer.Stop();
            return isFound;
        }

        private void CanExecute(string pathFileIn, string pathFileOut, string encode)
        {
            if (pathFileIn.EndsWith(Constants.CSV) || pathFileOut.EndsWith(Constants.CSV))
            {
                throw new UserException("В имени файла должно быть указано расширение .csv");
            }

            if (GetEncoding(encode) == null)
            {
                throw new UserException("Не верно указана кодировка");
            }
        }
    }
}

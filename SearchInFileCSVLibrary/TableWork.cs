namespace SearchInFileCSVLibrary
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ResourceLibrary;
    using SearchInFileCSVLibrary.Interface;

    public class TableWork : ITableWork
    {
        public TableWork()
        {
            Timer = new Stopwatch();
        }

        public Stopwatch Timer { get; private set; }

        public int[] FindNumbersColumnsHeader(string header, string colName, string expression, CancellationToken cancellationToken = default)
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
            return result;
        }

        public bool IsFindExpressionToRow(string line, int[] columnNumber, string expression)
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
    }
}

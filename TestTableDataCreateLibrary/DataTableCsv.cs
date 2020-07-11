namespace DataTableCreateLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataTableCreateLibrary.Interface;
    using DataTableCreateLibrary.Resource;

    public class DataTableCsv : IDataTableCsv
    {
        private const uint _limitCol = 1000000;
        private const char _delimeter = ';';
        private const char _delimeterColName = ' ';
        public DataTableCsv()
        {
            Timer = new Stopwatch();
        }

        public Random RandomColumn { get; private set; }

        public uint[] TypeColumns { get; private set; }

        public Stopwatch Timer { get; private set; }

        public Encoding GetEncoding(string encoding)
        {
            var result = DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encoding).Value;
            return result;
        }

        public void CreateDataTable(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut)
        {
            var encoding = GetEncoding(encode);
            using (StreamWriter sw = new StreamWriter(pathFileOut, false, encoding))
            {
                RandomColumn = new Random();
                TypeColumns = CreateTypeColumns(columns);
                for (int i = 0; i <= rows; i++)
                {
                    if (columns > _limitCol)
                    {
                        var batched = TypeColumns
                                    .Select((Value, Index) => new { Value, Index })
                                    .GroupBy(p => p.Index / _limitCol)
                                    .Select(g => g.Select(p => p.Value).ToArray());
                        foreach (var item in batched)
                        {
                            RandomColumn = new Random();
                            if (i == 0)
                            {
                                sw.Write(CreateHeaderTable(item, lenNameColumn));
                            }
                            else
                            {
                                sw.Write(CreateRowTable(item, len));
                            }
                        }

                        sw.WriteLine();
                    }
                    else
                    {
                        RandomColumn = new Random();
                        var typeColumns = CreateTypeColumns(columns);
                        if (i == 0)
                        {
                            sw.WriteLine(CreateHeaderTable(TypeColumns, lenNameColumn));
                        }
                        else
                        {
                            sw.WriteLine(CreateRowTable(TypeColumns, len));
                        }
                    }
                }
            }
        }

        public async void CreateDataTableAsinc(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut)
        {
            await Task.Run(() => CreateDataTable(columns, rows, len, lenNameColumn, encode, pathFileOut));
        }

        private string GetRandomString(uint len)
        {
            var res = string.Empty;
            for (int i = 0; i < len; i++)
            {
                res += (char)RandomColumn.Next('a', 'a' + 27);
            }

            return res;
        }

        private string GetRandomDate()
        {
            // случайный год в диапазоне от 1990 до 2020 включительно.
            var randomYear = RandomColumn.Next(1990, 2021);

            // случайный месяц от 1 до 12 включительно.
            var randomMonthNr = RandomColumn.Next(1, 13);

            // задает случайный день.
            var randomDayNr = RandomColumn.Next(1, DateTime.DaysInMonth(randomYear, randomMonthNr) + 1);

            return new DateTime(randomYear, randomMonthNr, randomDayNr).ToShortDateString();
        }

        private string GetRandomIntNumber()
        {
            return RandomColumn.Next().ToString();
        }

        private string GetRandomFloatNumber()
        {
            return ((float)RandomColumn.NextDouble()).ToString();
        }

        private string CreateHeaderTable(uint[] typeColumns, byte lenNameColumn)
        {
            Timer.Restart();
            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, item) =>
            {
                current.Append(GetRandomString(lenNameColumn));
                current.Append(_delimeterColName);
                current.Append(DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == item).Key);
                current.Append(_delimeter);
                return current;
            }).ToString().TrimEnd(_delimeter);
            Timer.Stop();
            return res;
        }

        private string CreateRowTable(uint[] typeColumns, uint len)
        {
            Timer.Restart();
            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, next) =>
            {
                switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == next).Value)
                {
                    case (byte)TypeColumnEnum.DateTimeColumn:
                        {
                            current.Append(GetRandomDate()).Append(_delimeter);
                            break;
                        }
                    case (byte)TypeColumnEnum.IntColumn:
                        {
                            current.Append(GetRandomIntNumber()).Append(_delimeter);
                            break;
                        }
                    case (byte)TypeColumnEnum.FloatColumn:
                        {
                            current.Append(GetRandomFloatNumber()).Append(_delimeter);
                            break;
                        }
                    default:
                        {
                            current.Append(GetRandomString(len)).Append(_delimeter);
                            break;
                        }
                }
                return current;
            }).ToString().TrimEnd(_delimeter);
            Timer.Stop();
            return res;
        }

        private uint[] CreateTypeColumns(uint columnsCount)
        {
            var res = new uint[columnsCount];
            Parallel.For(0, res.Length, i =>
            {
                res[i] = (uint)RandomColumn.Next(1, 5);
            });

            return res;
        }
    }
}

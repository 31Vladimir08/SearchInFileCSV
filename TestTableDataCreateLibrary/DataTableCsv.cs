namespace DataTableCreateLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
        public DataTableCsv()
        {
            RandomColumn = new Random();
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
                if (columns > _limitCol)
                {
                    throw new UserException("Максимальное допустимое кол-во колонок " + _limitCol);
                }
                else
                {
                    sw.WriteLine(CreateHeaderTable(columns, lenNameColumn));
                }
            }

            using (StreamWriter sw = new StreamWriter(pathFileOut, true, encoding))
            {
                for (int i = 0; i < rows; i++)
                {
                    sw.WriteLine(CreateRowTable(len));
                }
                /*Parallel.For(0, rows, i =>
                {
                    sw.WriteLine(CreateRowTable(len));
                });*/
            }

            Console.WriteLine("Выполнилось");
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

            return new DateTime(randomYear, randomMonthNr, randomDayNr).ToString();
        }

        private string GetRandomIntNumber()
        {
            return RandomColumn.Next().ToString();
        }

        private string GetRandomFloatNumber()
        {
            return ((float)RandomColumn.NextDouble()).ToString();
        }

        private string CreateHeaderTable(uint columnsCount, byte lenNameColumn)
        {
            TypeColumns = new uint[columnsCount];
            Parallel.For(0, TypeColumns.Length, i =>
                {
                    TypeColumns[i] = (uint)RandomColumn.Next(1, 5);
                });
            Timer.Restart();

            // Work faster.
            var res = TypeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, item) =>
            {
                current.Append(GetRandomString(lenNameColumn));
                current.Append(" ");
                current.Append(DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == item).Key);
                current.Append(";");
                return current;
            }).ToString();

            /*var res = string.Join(";", TypeColumns.Select(
                  x =>
                  {
                      var r = GetRandomString(lenNameColumn) + " " + DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == x).Key;
                      return r;
                  }));*/
            Timer.Stop();
            return res;
        }

        private string CreateRowTable(uint len)
        {
            Timer.Restart();
            var res = TypeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, next) =>
            {
                switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == next).Value)
                {
                    case (byte)TypeColumnEnum.DateTimeColumn:
                        {
                            current.Append(GetRandomDate()).Append(";");
                            break;
                        }
                    case (byte)TypeColumnEnum.IntColumn:
                        {
                            current.Append(GetRandomIntNumber()).Append(";");
                            break;
                        }
                    case (byte)TypeColumnEnum.FloatColumn:
                        {
                            current.Append(GetRandomFloatNumber()).Append(";");
                            break;
                        }
                    default:
                        {
                            current.Append(GetRandomString(len)).Append(";");
                            break;
                        }
                }
                return current;
            }).ToString();
            /*var res = string.Join(";", TypeColumns.Select(
                    x =>
                    {
                        switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == x).Value)
                        {
                            case (byte)TypeColumnEnum.DateTimeColumn:
                                {
                                    return GetRandomDate() + ";";
                                }
                            case (byte)TypeColumnEnum.IntColumn:
                                {
                                    return GetRandomIntNumber() + ";";
                                }
                            case (byte)TypeColumnEnum.FloatColumn:
                                {
                                    return GetRandomFloatNumber() + ";";
                                }
                            default:
                                {
                                    return GetRandomString(len) + ";";
                                }
                        }
                    }));*/
            Timer.Stop();
            return res;
        }
    }
}

﻿namespace DataTableCreateLibrary
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
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

        public void CreateDataTable(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken)
        {
            CanExecute(columns, rows, len, lenNameColumn, encode);
            var encoding = GetEncoding(encode);
            using (StreamWriter sw = new StreamWriter(pathFileOut, false, encoding))
            {
                RandomColumn = new Random();
                TypeColumns = CreateTypeColumns(columns);
                for (uint i = 0; i <= rows; i++)
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
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
        }

        public async Task<string> CreateDataTableAsinc(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() => CreateDataTable(columns, rows, len, lenNameColumn, encode, pathFileOut, cancellationToken), cancellationToken);
                return string.Empty;
            }
            catch (OperationCanceledException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetRandomString(uint len)
        {
            var res = string.Empty;
            for (uint i = 0; i < len; i++)
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

        private void CanExecute(uint columns, uint rows, uint len, byte lenNameColumn, string encode)
        {
            if (columns == 0)
            {
                throw new UserException("Кол-во колонок должно быть не меньше 1");
            }

            if (rows == 0)
            {
                throw new UserException("Кол-во строк должно быть не меньше 1");
            }

            if (len == 0 || len > 200)
            {
                throw new UserException("Кол-во символов в строковых столбцах должно быть от 1 до 200");
            }

            if (lenNameColumn == 0 || lenNameColumn > 50)
            {
                throw new UserException("Кол-во символов в именах колонок должно быть от 1 до 50");
            }

            if (GetEncoding(encode) == null)
            {
                throw new UserException("Не верно указана кодировка");
            }
        }
    }
}

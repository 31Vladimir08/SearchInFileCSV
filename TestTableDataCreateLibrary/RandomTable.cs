namespace DataTableCreateLibrary
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataTableCreateLibrary.Interface;
    using ResourceLibrary;

    public class RandomTable : IRandomTable
    {
        public RandomTable(uint columnsCount)
        {
            Timer = new Stopwatch();
            TypeColumns = CreateTypeColumns(columnsCount);
        }

        public uint[] TypeColumns { get; private set; }

        public Stopwatch Timer { get; private set; }

        public string CreateHeaderTable(byte lenNameColumn, Random random, uint[] typeColumns = default)
        {
            Timer.Restart();
            if (typeColumns == null)
            {
                typeColumns = TypeColumns;
            }

            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, item) =>
            {
                current.Append(GetRandomString(lenNameColumn, random));
                current.Append(Constants.DELIMETERCOLNAME);
                current.Append(DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == item).Key);
                current.Append(Constants.DELIMETER);
                return current;
            }).ToString().TrimEnd(Constants.DELIMETER);
            Timer.Stop();
            return res;
        }

        public string CreateRowTable(uint len, Random random, uint[] typeColumns = default)
        {
            Timer.Restart();
            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, next) =>
            {
                switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == next).Value)
                {
                    case (byte)TypeColumnEnum.DateTimeColumn:
                        {
                            current.Append(GetRandomDate(random)).Append(Constants.DELIMETER);
                            break;
                        }
                    case (byte)TypeColumnEnum.IntColumn:
                        {
                            current.Append(GetRandomIntNumber(random)).Append(Constants.DELIMETER);
                            break;
                        }
                    case (byte)TypeColumnEnum.FloatColumn:
                        {
                            current.Append(GetRandomFloatNumber(random)).Append(Constants.DELIMETER);
                            break;
                        }
                    default:
                        {
                            current.Append(GetRandomString(len, random)).Append(Constants.DELIMETER);
                            break;
                        }
                }
                return current;
            }).ToString().TrimEnd(Constants.DELIMETER);
            Timer.Stop();
            return res;
        }

        public uint[] CreateTypeColumns(uint columnsCount)
        {
            var random = new Random();
            var res = new uint[columnsCount];
            Parallel.For(0, res.Length, i =>
            {
                res[i] = (uint)random.Next(1, 5);
            });

            return res;
        }

        public string GetRandomString(uint len, Random random)
        {
            var res = string.Empty;
            for (uint i = 0; i < len; i++)
            {
                res += (char)random.Next('a', 'a' + 27);
            }

            return res;
        }

        public string GetRandomDate(Random random)
        {
            // случайный год в диапазоне от 1990 до 2020 включительно.
            var randomYear = random.Next(1990, 2021);

            // случайный месяц от 1 до 12 включительно.
            var randomMonthNr = random.Next(1, 13);

            // задает случайный день.
            var randomDayNr = random.Next(1, DateTime.DaysInMonth(randomYear, randomMonthNr) + 1);

            return new DateTime(randomYear, randomMonthNr, randomDayNr).ToShortDateString();
        }

        public string GetRandomIntNumber(Random random)
        {
            return random.Next().ToString();
        }

        public string GetRandomFloatNumber(Random random)
        {
            return ((float)random.NextDouble()).ToString();
        }
    }
}

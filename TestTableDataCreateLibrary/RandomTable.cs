namespace DataTableCreateLibrary
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataTableCreateLibrary.Interface;

    using ResourceLibrary;

    public class RandomTable : IRandomTable
    {
        public RandomTable(uint columnsCount)
        {
            TypeColumns = CreateTypeColumns(columnsCount);
        }

        public uint[] TypeColumns { get; private set; }

        public string CreateHeaderTable(byte lenNameColumn, Random random, uint[] typeColumns = default)
        {
            if (typeColumns == null)
            {
                typeColumns = TypeColumns;
            }

            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, item) =>
            {
                current.Append(GetRandomString(lenNameColumn, random));
                current.Append(Resource.DELIMETERCOLNAME);
                current.Append(DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == item).Key);
                current.Append(Resource.DELIMETER);
                return current;
            }).ToString().TrimEnd(Resource.DELIMETER);
            return res;
        }

        public string CreateRowTable(uint len, Random random, uint[] typeColumns = default)
        {
            if (typeColumns == null)
            {
                typeColumns = TypeColumns;
            }
            var res = typeColumns.AsParallel().AsOrdered().Aggregate(new StringBuilder(), (current, next) =>
            {
                switch (DictionaryLibrary.TypeColumnDict.FirstOrDefault(y => y.Value == next).Value)
                {
                    case (byte)TypeColumnEnum.DateTimeColumn:
                        {
                            current.Append(GetRandomDate(random)).Append(Resource.DELIMETER);
                            break;
                        }
                    case (byte)TypeColumnEnum.IntColumn:
                        {
                            current.Append(GetRandomIntNumber(random)).Append(Resource.DELIMETER);
                            break;
                        }
                    case (byte)TypeColumnEnum.FloatColumn:
                        {
                            current.Append(GetRandomFloatNumber(random)).Append(Resource.DELIMETER);
                            break;
                        }
                    default:
                        {
                            current.Append(GetRandomString(len, random)).Append(Resource.DELIMETER);
                            break;
                        }
                }
                return current;
            }).ToString().TrimEnd(Resource.DELIMETER);
            return res;
        }

        private uint[] CreateTypeColumns(uint columnsCount)
        {
            var random = new Random();
            var res = new uint[columnsCount];
            Parallel.For(0, res.Length, i =>
            {
                res[i] = (uint)random.Next(1, 5);
            });

            return res;
        }

        private string GetRandomString(uint len, Random random)
        {
            var res = string.Empty;
            for (uint i = 0; i < len; i++)
            {
                res += (char)random.Next('a', 'a' + 27);
            }

            return res;
        }

        private string GetRandomDate(Random random)
        {
            // случайный год в диапазоне от 1990 до 2020 включительно.
            var randomYear = random.Next(1990, 2021);

            // случайный месяц от 1 до 12 включительно.
            var randomMonthNr = random.Next(1, 13);

            // задает случайный день.
            var randomDayNr = random.Next(1, DateTime.DaysInMonth(randomYear, randomMonthNr) + 1);

            return new DateTime(randomYear, randomMonthNr, randomDayNr).ToShortDateString();
        }

        private string GetRandomIntNumber(Random random)
        {
            return random.Next().ToString();
        }

        private string GetRandomFloatNumber(Random random)
        {
            return ((float)random.NextDouble()).ToString();
        }
    }
}

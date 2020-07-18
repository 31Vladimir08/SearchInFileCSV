namespace DataTableCreateLibrary
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTableCreateLibrary.Interface;
    using ResourceLibrary;

    public class DataTableCsv : IDataTableCsv
    {
        public void CreateDataTable(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken)
        {
            CanExecute(columns, rows, len, lenNameColumn, encode, pathFileOut);
            var encoding = DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encode).Value;
            using (StreamWriter sw = new StreamWriter(pathFileOut, false, encoding))
            {
                var random = new Random();
                var randomTable = new RandomTable(columns);
                for (uint i = 0; i <= rows; i++)
                {
                    if (columns > Constants.LIMITETCOL)
                    {
                        var batched = randomTable.TypeColumns
                                    .Select((Value, Index) => new { Value, Index })
                                    .GroupBy(p => p.Index / Constants.LIMITETCOL)
                                    .Select(g => g.Select(p => p.Value).ToArray());
                        foreach (var item in batched)
                        {
                            random = new Random();
                            if (i == 0)
                            {
                                sw.Write(randomTable.CreateHeaderTable(lenNameColumn, random, item));
                            }
                            else
                            {
                                sw.Write(randomTable.CreateRowTable(len, random, item));
                            }
                        }

                        sw.WriteLine();
                    }
                    else
                    {
                        random = new Random();
                        if (i == 0)
                        {
                            sw.WriteLine(randomTable.CreateHeaderTable(lenNameColumn, random));
                        }
                        else
                        {
                            sw.WriteLine(randomTable.CreateRowTable(len, random));
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
        }

        public async Task CreateDataTableAsinc(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() => CreateDataTable(columns, rows, len, lenNameColumn, encode, pathFileOut, cancellationToken), cancellationToken);
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

        private void CanExecute(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut)
        {
            if (columns == 0)
            {
                throw new UserException("Кол-во колонок должно быть не меньше 1 и не более " + uint.MaxValue);
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

            if (DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encode).Value == null)
            {
                throw new UserException("Не верно указана кодировка");
            }

            if (!pathFileOut.EndsWith(Constants.CSV))
            {
                throw new UserException("В имени файла должно быть указано расширение .csv");
            }
        }
    }
}

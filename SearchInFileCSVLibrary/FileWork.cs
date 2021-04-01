namespace SearchInFileCSVLibrary
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using ResourceLibrary;

    using SearchInFileCSVLibrary.Interface;

    public class FileWork : IFileWork
    {
        public void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken = default)
        {
            CanExecute(pathFileIn, pathFileOut, encode);
            var encoding = DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encode).Value;
            using (StreamReader sr = new StreamReader(pathFileIn, encoding))
            {
                var tableWork = new TableWork();
                var line = sr.ReadLine();
                var columnsNambers = tableWork.FindNumbersColumnsHeader(line, colName, expression, cancellationToken);
                using (StreamWriter sw = new StreamWriter(pathFileOut, false, encoding))
                {
                    sw.WriteLine(line);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                while ((line = sr.ReadLine()) != null)
                {
                    if (tableWork.IsFindExpressionToRow(line, columnsNambers, expression))
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

        private void CanExecute(string pathFileIn, string pathFileOut, string encode)
        {
            if (!pathFileIn.EndsWith(Resource.CSV) || !pathFileOut.EndsWith(Resource.CSV))
            {
                throw new UserException("В имени файла должно быть указано расширение .csv");
            }

            if (DictionaryLibrary.EncodingDict.FirstOrDefault(x => x.Key == encode).Value == null)
            {
                throw new UserException("Не верно указана кодировка");
            }
        }
    }
}

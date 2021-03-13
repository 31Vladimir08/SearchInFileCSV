namespace SearchInFileCSV
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTableCreateLibrary;
    using ResourceLibrary;
    using SearchInFileCSVLibrary;

    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
#if RELEASE
                CanExecute(args);
#endif
                using (CancellationTokenSource cancellationToken = new CancellationTokenSource())
                {
                    var token = cancellationToken.Token;
                    Task task;
#if DEBUG
                    int i = 2;
                    uint columns = 10;
                    uint rows = 4;
                    uint len = 10;
                    byte lenName = 3;
                    string input = @"E:\TestBigData.csv";
                    string output = @"E:\TestOutput.csv";
                    string encode = "UTF8";
                    string columname = @"zyivxmrrop";
                    string expression = "0,4046684";
                    Task taskKey = Task.Run(() => GetConsoleKey(cancellationToken));
                    if (i == 1)
                    {
                        task = Task.Run(async () => await new FileWork().SearchInFileCSVAsync(input, output, encode, columname, expression, token), token);
                    }
                    else
                    {
                        task = Task.Run(async () => await new DataTableCsv().CreateDataTableAsinc(columns, rows, len, lenName, encode, input, token), token);
                    }
#endif

#if RELEASE
                    if (args[0] == "1")
                    {
                        task = Task.Run(async () => await new FileWork().SearchInFileCSVAsync(args[1], args[2], args[3], args[4], args[5], token), token);
                    }
                    else
                    {
                        task = Task.Run(
                            async () => await new DataTableCsv().CreateDataTableAsinc
                                (
                                    Convert.ToUInt32(args[1]),
                                    Convert.ToUInt32(args[2]),
                                    Convert.ToUInt32(args[3]),
                                    Convert.ToByte(args[4]),
                                    args[5],
                                    args[6],
                                    token),
                                token);
                    }
#endif
                    task.Wait();
                    cancellationToken.Cancel();
                    Console.WriteLine("Программа успешно завершила работу, для выхода из программы, нажмите любую клавишу.");
                }
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message);
                foreach (var item in ex.InnerExceptions)
                {
                    Console.WriteLine(item.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void GetConsoleKey(CancellationTokenSource cancellationToken)
        {
            try
            {
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine(key.Key + " клавиша была нажата");
                        cancellationToken.Cancel();
                    }
                }
                while (!cancellationToken.IsCancellationRequested && key.Key != ConsoleKey.Escape);
            }
            catch (Exception ex)
            {
                throw new UserException(ex.Message);
            }
        }

        private static void CanExecute(string[] args)
        {
            if (args == null)
            {
                throw new UserException("Вы не передали параметры");
            }

            if (args.Length != 6 || args.Length != 5)
            {
                throw new UserException("Не верное кол-во переданных параметров");
            }

            if (args[0] != "1" || args[0] != "2")
            {
                throw new UserException("Первым параметром должно быть число 1 - Для поиска в файле или 2 - Для генерации тестового файла");
            }

            foreach (var item in args)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    throw new UserException("Один из передаваемых параметров содержит пустую строку");
                }
            }
        }
    }
}

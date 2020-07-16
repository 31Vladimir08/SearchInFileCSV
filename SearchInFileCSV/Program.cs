namespace SearchInFileCSV
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTableCreateLibrary;
    using ResourceLibrary;
    using SearchInFileCSVLibrary;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
#if RELEASE
                CanExecute(args);
#endif
                using (CancellationTokenSource cancellationToken = new CancellationTokenSource())
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(GetConsoleKey));
                    thread.IsBackground = true;
                    thread.Start(cancellationToken);
#if DEBUG
                    int i = 1;
                    uint columns = 2000000;
                    uint rows = 5;
                    uint len = 100;
                    byte lenName = 30;
                    string input = @"C:\Users\Admin\Desktop\ТЕСТ\TestBigData.csv";
                    string output = @"C:\Users\Admin\Desktop\ТЕСТ\TestOutput.csv";
                    string encode = "UTF8";
                    string columname = @"cfhwd";
                    string expression = "02.02.2011";
                    if (i == 1)
                    {
                        await new FileWork().SearchInFileCSVAsync(input, output, encode, columname, expression, cancellationToken.Token);
                    }
                    else
                    {
                        await new DataTableCsv().CreateDataTableAsinc(columns, rows, len, lenName, encode, output, cancellationToken.Token);
                    }
#endif

#if RELEASE
                    if (args[0] == "1")
                    {
                        await new FileWork().SearchInFileCSVAsync(args[1], args[2], args[3], args[4], args[5], cancellationToken.Token);
                    }
                    else if (args[0] == "2")
                    {
                        await new DataTableCsv().CreateDataTableAsinc(Convert.ToUInt32(args[1]), Convert.ToUInt32(args[2]), Convert.ToUInt32(args[3]), Convert.ToByte(args[4]), args[5], args[6], cancellationToken.Token);
                    }
#endif
                    cancellationToken.Cancel();
                    Console.WriteLine("Программа успешно завершила работу, для выхода из программы, нажмите любую клавишу.");
                    thread.Join();
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

        private static void GetConsoleKey(object cancellationToken)
        {
            try
            {
                ConsoleKeyInfo key;
                var x = (CancellationTokenSource)cancellationToken;
                do
                {
                    key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine(key.Key + " клавиша была нажата");
                        x.Cancel();
                    }
                }
                while (!x.Token.IsCancellationRequested && key.Key != ConsoleKey.Escape);
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

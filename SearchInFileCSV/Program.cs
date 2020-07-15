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
        private static async Task Main(string[] args)
        {
            try
            {
                CanExecute(args);
                using (CancellationTokenSource cancellationToken = new CancellationTokenSource())
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(GetConsoleKey));
                    thread.IsBackground = true;
                    thread.Start(cancellationToken);
                    /*string input = @"C:\Users\Admin\Desktop\ТЕСТ\TestBigData.csv";
                    string output = @"C:\Users\Admin\Desktop\ТЕСТ\TestOutput.csv";
                    string encode = "UTF8";
                    string columname = @"cfhwd";
                    string expression = "02.02.2011";
                    Console.WriteLine("Программа начала выполняться, для досрочного завершения работы программы, нажмите Esc");
                    await new DataTableCsv().CreateDataTableAsinc(10000000, 5, 10, 4, encode, input, cancellationToken.Token);*/
                    if (args[0] == "1")
                    {
                        await new FileWork().SearchInFileCSVAsync(args[1], args[2], args[3], args[4], args[5], cancellationToken.Token);
                    }
                    else if (args[0] == "2")
                    {
                        await new DataTableCsv().CreateDataTableAsinc(Convert.ToUInt32(args[1]), Convert.ToUInt32(args[2]), Convert.ToUInt32(args[3]), Convert.ToByte(args[4]), args[5], args[6], cancellationToken.Token);
                    }
                    else
                    {
                        throw new UserException("Первым параметром должно быть число 1 - Для поиска в файле или 2 - Для генерации тестового файла");
                    }

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

namespace SearchInFileCSV
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTableCreateLibrary;
    using SearchInFileCSVLibrary;

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Task<string> task;
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                if (args[0] == "1")
                {
                    task = new FileWork().SearchInFileCSVAsync(args[1], args[2], args[3], args[4], args[5], cancellationToken.Token);
                }
                else if (args[0] == "2")
                {
                    task = new DataTableCsv().CreateDataTableAsinc(Convert.ToUInt32(args[1]), Convert.ToUInt32(args[2]), Convert.ToUInt32(args[3]), Convert.ToByte(args[4]), args[5], args[6], cancellationToken.Token);
                }
                else
                {
                    throw new UserException("Первым параметром должно быть число 1 - Для поиска в файле или 2 - Для генерации тестового файла");
                }

                Console.WriteLine("Программа начала выполняться, для досрочного завершения работы программы, нажмите Esc");
                Thread thread = new Thread(new ParameterizedThreadStart(GetConsoleKey));
                thread.IsBackground = true;
                thread.Start(cancellationToken);

                if (await task != string.Empty)
                {
                    throw new Exception(task.Result);
                }

                Console.WriteLine("Программа успешно завершила работу");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void GetConsoleKey(object cancellationToken)
        {
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine(key.Key + " клавиша была нажата");
                    var x = (CancellationTokenSource)cancellationToken;
                    x.Cancel();
                }
            }
            while (key.Key != ConsoleKey.Escape);
        }
    }
}

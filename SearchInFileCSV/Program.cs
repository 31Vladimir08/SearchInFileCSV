namespace SearchInFileCSV
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTableCreateLibrary;
    using SearchInFileCSVLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                /*if (args[0] == "1")
                {
                    new FileWork().SearchInFileCSVAsync(args[1], args[2], args[3], args[4], args[5]);
                }
                else if (args[0] == "2")
                {
                    new DataTableCsv().CreateDataTableAsinc(Convert.ToUInt32(args[1]), Convert.ToUInt32(args[2]), Convert.ToUInt32(args[3]), Convert.ToByte(args[4]), args[5], args[6]);
                }
                else
                {

                }*/
                string input = @"C:\Users\Admin\Desktop\ТЕСТ\TestBigData.csv";
                string output = @"C:\Users\Admin\Desktop\ТЕСТ\TestOutput.csv";
                string encode = "UTF8";
                string columname = @"cfhwd";
                string expression = "02.02.2011";
                /*new FileWork().SearchInFileCSVAsync(input, output, encode, columname, expression, cancellationToken);*/
                new DataTableCsv().CreateDataTableAsinc(1000000, 5000, 10, 4, encode, input, cancellationToken);
                while (Console.ReadLine() == "n")
                {
                    cancellationToken.Cancel();
                    Console.WriteLine("Операция отменена");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

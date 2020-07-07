namespace SearchInFileCSV
{
    using System;
    using System.Text.RegularExpressions;
    using DataTableCreateLibrary;
    using SearchInFileCSVLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string input = @"C:\Users\Admin\Desktop\ТЕСТ\TestBigData.csv";
                string output = @"C:\Users\Admin\Desktop\ТЕСТ\TestOutput.csv";
                string encode = "UTF8";
                string columname = @"awrsr";
                string expression = "vbudnoneie";
                new FileWork().SearchInFileCSVAsync(input, output, encode, columname, expression);
                new DataTableCsv().CreateDataTableAsinc(10, 5, 10, 5, encode, input);
                Console.WriteLine("Тест");
                Console.ReadKey();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

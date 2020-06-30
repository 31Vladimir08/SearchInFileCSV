namespace SearchInFileCSV
{
    using DataTableCreateLibrary;
    using SearchInFileCSVLibrary;
    using System;
    using System.Data;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string input = @"C:\Users\Admin\Desktop\ТЕСТ\TestBigData.csv";
                string output = @"C:\Users\Admin\Desktop\ТЕСТ\TestOutput.csv";
                string encode = "UTF8";
                string columname = @"grudrfvgny";
                string expression = "0,5547456";
                /*new FileWork().SearchInFileCSVAsync(input, output, encode, columname, expression);*/
                new DataTableCsv().CreateDataTableAsinc(1000, 1000000, 10, 5, encode, input);
                Console.WriteLine("Тест");
                Console.ReadKey();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

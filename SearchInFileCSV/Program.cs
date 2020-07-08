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
            string[] str = new string[6] { "1", "2", "3", "4", "5", null };
            new StartProgram(str[0], str[1], str[2], str[3], str[4], str[5]);
        }
    }
}

namespace SearchInFileCSVLibrary.Interface
{
    using System.Diagnostics;
    using System.Text;

    public interface IFileWork
    {
        Stopwatch Timer { get; }

        Encoding GetEncoding(string encoding);

        void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression);

        void SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression);
    }
}

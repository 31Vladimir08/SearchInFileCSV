namespace SearchInFileCSVLibrary.Interface
{
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFileWork
    {
        int CountColumns { get; }

        Stopwatch Timer { get; }

        Encoding GetEncoding(string encoding);

        void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken);

        void SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationTokenSource cancellationToken);
    }
}

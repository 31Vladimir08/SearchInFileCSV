namespace SearchInFileCSVLibrary.Interface
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFileWork
    {
        void SearchInFileCSV(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken);

        Task SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken);
    }
}

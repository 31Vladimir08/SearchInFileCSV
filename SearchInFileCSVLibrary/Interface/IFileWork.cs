namespace SearchInFileCSVLibrary.Interface
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFileWork
    {
        Task SearchInFileCSVAsync(string pathFileIn, string pathFileOut, string encode, string colName, string expression, CancellationToken cancellationToken);
    }
}

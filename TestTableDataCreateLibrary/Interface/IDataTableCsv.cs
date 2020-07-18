namespace DataTableCreateLibrary.Interface
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDataTableCsv
    {
        void CreateDataTable(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken);

        Task CreateDataTableAsinc(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken);
    }
}

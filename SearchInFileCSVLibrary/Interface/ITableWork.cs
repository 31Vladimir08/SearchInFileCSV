namespace SearchInFileCSVLibrary.Interface
{
    using System.Threading;

    public interface ITableWork
    {
        int[] FindNumbersColumnsHeader(string header, string colName, string expression, CancellationToken cancellationToken = default);

        bool IsFindExpressionToRow(string line, int[] columnNumber, string expression);
    }
}

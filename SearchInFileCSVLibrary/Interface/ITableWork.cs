namespace SearchInFileCSVLibrary.Interface
{
    using System.Diagnostics;
    using System.Threading;

    public interface ITableWork
    {
        public Stopwatch Timer { get; }

        int[] FindNumbersColumnsHeader(string header, string colName, string expression, CancellationToken cancellationToken = default);

        bool IsFindExpressionToRow(string line, int[] columnNumber, string expression);
    }
}

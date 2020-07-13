﻿namespace DataTableCreateLibrary.Interface
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDataTableCsv
    {
        Random RandomColumn { get; }

        Stopwatch Timer { get; }

        uint[] TypeColumns { get; }

        Encoding GetEncoding(string encoding);

        void CreateDataTable(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken);

        Task CreateDataTableAsinc(uint columns, uint rows, uint len, byte lenNameColumn, string encode, string pathFileOut, CancellationToken cancellationToken);
    }
}

namespace DataTableCreateLibrary.Interface
{
    using System;

    public interface IRandomTable
    {
        uint[] TypeColumns { get; }

        string CreateHeaderTable(byte lenNameColumn, Random random, uint[] typeColumns = default);

        string CreateRowTable(uint len, Random random, uint[] typeColumns = default);
    }
}

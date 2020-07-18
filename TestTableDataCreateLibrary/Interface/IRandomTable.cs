namespace DataTableCreateLibrary.Interface
{
    using System;

    public interface IRandomTable
    {
        string CreateHeaderTable(byte lenNameColumn, Random random, uint[] typeColumns = default);

        string CreateRowTable(uint len, Random random, uint[] typeColumns = default);

        uint[] CreateTypeColumns(uint columnsCount);

        string GetRandomString(uint len, Random random);

        string GetRandomDate(Random random);

        string GetRandomIntNumber(Random random);

        string GetRandomFloatNumber(Random random);
    }
}

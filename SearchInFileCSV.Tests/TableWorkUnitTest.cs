namespace SearchInFileCSV.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SearchInFileCSVLibrary;

    [TestClass]
    public class TableWorkUnitTest
    {
        [DataTestMethod]
        [DataRow("test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string", "test2", "25", new int[] { 1, 4, 7 })]
        [DataRow("test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string", "test2", "15 ноября 2017", new int[] { 5, 7, 8 })]
        [DataRow("test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string", "test2", "15.01.2017", new int[] { 5, 7, 8 })]
        [DataRow("test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string", "test2", "12,654", new int[] { 4, 5, 7, 8 })]
        [DataRow("test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string", "testTest", "12,654", new int[] { })]
        public void FindNumbersColumnsHeaderTest(string header, string colName, string expression, int[] expected)
        {
            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("01.02.1986;hhfhsd;162,6453;82531;fggfg;543,955;vbcb;24.09.1999;5644554;test;473434;test;9387434;4734,4634", new int[] { 1, 4, 10 }, "543,955", false)]
        [DataRow("01.02.1986;hhfhsd;162,6453;82531;fggfg;543,955;vbcb;24.09.1999;5644554;test;473434;test;9387434;4734,4634", new int[] { 1, 5, 6, 10 }, "543,955", true)]
        public void IsFindExpressionToRowTest(string line, int[] columnNumber, string expression, bool expected)
        {
            //act
            var actual = new TableWork().IsFindExpressionToRow(line, columnNumber, expression);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}

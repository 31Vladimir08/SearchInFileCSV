namespace DataTableCreateLibraryTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SearchInFileCSVLibrary;

    [TestClass]
    public class TableWorkUnitTest
    {
        [TestMethod]
        public void FindNumbersColumnsHeaderTest1()
        {
            // arrange
            string header = "test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string";
            string colName = "test2";
            string expression = "25";
            int[] expected = new int[] { 1, 4, 7 };

            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindNumbersColumnsHeaderTest2()
        {
            // arrange
            string header = "test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string";
            string colName = "test2";
            string expression = "15 но€бр€ 2017";
            int[] expected = new int[] { 5, 7, 8 };

            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindNumbersColumnsHeaderTest3()
        {
            // arrange
            string header = "test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string";
            string colName = "test2";
            string expression = "15.01.2017";
            int[] expected = new int[] { 5, 7, 8 };

            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindNumbersColumnsHeaderTest4()
        {
            // arrange
            string header = "test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string";
            string colName = "test2";
            string expression = "12,654";
            int[] expected = new int[] { 4, 5, 7, 8 };

            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindNumbersColumnsHeaderTest5()
        {
            // arrange
            string header = "test1 string;test2 int;test3 DateTime;test4 float;test2 float;test2 DateTime;test5 string;test2 string;test2 DateTime; test6 string";
            string colName = "testTest";
            string expression = "12,654";
            int[] expected = new int[0];

            //act
            var actual = new TableWork().FindNumbersColumnsHeader(header, colName, expression);

            // assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFindExpressionToRowTest1()
        {
            // arrange
            string line = "01.02.1986;hhfhsd;162,6453;82531;fggfg;543,955;vbcb;24.09.1999;5644554;test;473434;test;9387434;4734,4634";
            int[] columnNumber = new int[] { 1, 4, 10};
            string expression = "543,955";
            bool expected = false;

            //act
            var actual = new TableWork().IsFindExpressionToRow(line, columnNumber, expression);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsFindExpressionToRowTest2()
        {
            // arrange
            string line = "01.02.1986;hhfhsd;162,6453;82531;fggfg;543,955;vbcb;24.09.1999;5644554;test;473434;test;9387434;4734,4634";
            int[] columnNumber = new int[] { 1, 5, 6, 10 };
            string expression = "543,955";
            bool expected = true;

            //act
            var actual = new TableWork().IsFindExpressionToRow(line, columnNumber, expression);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}

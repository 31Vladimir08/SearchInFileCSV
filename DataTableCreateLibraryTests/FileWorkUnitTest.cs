namespace DataTableCreateLibrary.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SearchInFileCSVLibrary;

    [TestClass]
    public class FileWorkUnitTest
    {
        [TestMethod]
        public void SearchInFileCSVTest()
        {
            // arrange
            string colname = "test";
            string expression = "611858614";

            var exePath = AppDomain.CurrentDomain.BaseDirectory;//path to exe file
            var pathTestDataInput = Path.Combine(exePath, "TestFiles\\TestData.csv");
            var pathTestDataOutput = Path.Combine(exePath, "TestFiles\\TestDataOutput.csv");
            //act
            new FileWork().SearchInFileCSV(pathTestDataInput, pathTestDataOutput, "UTF8", colname, expression);
            // assert
        }
    }
}

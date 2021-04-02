namespace SearchInFileCSV.Tests
{
    using System;
    using System.IO;

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
            var pathTestDataInput = Path.Combine(exePath, "Resourses\\TestDataInput.csv");
            var pathTestDataOutput = Path.Combine(exePath, "Resourses\\TestDataOutput.csv");
            //act
            new FileWork().SearchInFileCSV(pathTestDataInput, pathTestDataOutput, "UTF8", colname, expression);
            // assert
        }
    }
}

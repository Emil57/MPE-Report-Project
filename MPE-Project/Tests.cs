using Microsoft.VisualBasic.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Data;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

[TestFixture]
public class TestsMethods
{

	public TestsMethods()
    {
    }
    [TestMethod]
    public void Test_LoadExcelFileWithDateTestedAsDateFormat()
    {
        string[] headers = { "Lot Code", "Date Code", "Date Tested", "Test - Volume In" };
        DataTable dataTableExpected = new();
        foreach(string header in headers)
        {
            if(header.Equals("Date Tested"))
            {
                dataTableExpected.Columns.Add(header,typeof(DateTime));
            }
            dataTableExpected.Columns.Add(header);
        }

        //Assert 
        //Assert.That(actualSum, Is.EqualTo(expected));
        //Assert.AreEqual(expected, actualSum);
    }
}

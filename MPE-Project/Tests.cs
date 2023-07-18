using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.Data;
using static ExcelStructure;
using Assert = NUnit.Framework.Assert;

[TestFixture]
public class TestsMethods
{
    public TestsMethods() {
        Test_LoadExcelFileWithDateTestedAsDateFormat();
    }

    [Test]
    public void Test_LoadExcelFileWithDateTestedAsDateFormat()
    {

        string[] headers = { "Lot Code", "Date Tested", "Qty In" };
        DataTable dataTableExpected = new(), dataTable = new();
        foreach(string header in headers)
        {
            if(header.Equals("Date Tested"))
            {
                dataTableExpected.Columns.Add(header,typeof(DateTime));
            }
            dataTableExpected.Columns.Add(header);
        }
        DataRow dataRow = dataTableExpected.NewRow();
        dataRow["Lot Code"] = "32107584.1";
        dataRow["Date Tested"] = new DateTime(2023,6,19,12,0,0);
        dataRow["Qty In"] = "4694";
        dataRow = dataTableExpected.NewRow();
        dataRow["Lot Code"] = "32077540.1";
        dataRow["Date Tested"] = new DateTime(2023, 6, 22, 12, 0, 0);
        dataRow["Qty In"] = "11470";

        string file = "\\mexhome03\\Data\\Test Engineering\\Public\\Product Engineering\07_PDSE Files\\Dimas Emiliano\\MPE Reports\\Pruebas MPE\test metodos\tablaExpected.xlsx";
        dataTable = LoadExcelFileWithDateTestedAsDateFormat(file);
        
        //Assert 
        bool equal = dataTable == dataTableExpected;
        Assert.IsTrue(equal);
    }
}

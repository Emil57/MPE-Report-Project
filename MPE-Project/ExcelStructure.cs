using NUnit.Framework;
using OfficeOpenXml;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

/// <summary>
/// This is the class to contains the methods to load and export excel files using epplus library
/// </summary>
public class ExcelStructure
{
    /*
    public static DataTable LoadExcelFile(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage package = new ExcelPackage(new FileInfo(filePath));
        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet
        DataTable dataTable = new();

        // Load headers
        int totalColumns = worksheet.Dimension.Columns;
        for (int col = 1; col <= totalColumns; col++)
        {
            string? headerText = worksheet.Cells[1, col].Value?.ToString();
            dataTable.Columns.Add(headerText);
        }

        // Load data rows
        int totalRows = worksheet.Dimension.Rows;
        for (int row = 2; row <= totalRows; row++)
        {
            DataRow dataRow = dataTable.NewRow();
            for (int col = 1; col <= totalColumns; col++)
            {
                dataRow[col - 1] = worksheet.Cells[row, col].Value?.ToString();
            }
            dataTable.Rows.Add(dataRow);
        }
        return dataTable;
    }
    */
    /// <summary>
    /// Method to load the offshore reports to a list of datatables
    /// </summary>
    /// <param name="filePath">Path to load the excel report</param>
    /// <param name="targetSheetNames">array of part numbers to target in the sheetnames of the report</param>
    /// <returns></returns>
    public static List<DataTable> LoadOffshoreFile(string filePath, string[] targetSheetNames)
    {
        bool sheetEmpty = false;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        List<DataTable> dataTables = new List<DataTable>();
        using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorkbook workbook = excelPackage.Workbook;

            foreach (string sheetName in targetSheetNames)
            {
                ExcelWorksheet worksheet = workbook.Worksheets[sheetName];

                if (worksheet != null)
                {
                    DataTable dataTable = new DataTable(sheetName);

                    int totalColumns = worksheet.Dimension.Columns-1;
                    int totalRows = worksheet.Dimension.Rows;
                    for (int col = 1; col < totalColumns; col++)
                    {
                        string? headerText = worksheet.Cells[1, col].Value?.ToString();
                        dataTable.Columns.Add(headerText);
                        if (string.IsNullOrEmpty(headerText))
                        {
                            sheetEmpty = true;
                            break;
                        }
                        if (headerText.Contains("BIN"))
                        {
                            for (int i = 2; i <=3; i++)
                            {
                                dataTable.Columns.Add(headerText + i.ToString());
                            }
                            col += 2;
                        }
                    }
                    //PrintDataTable(dataTable);
                    if(sheetEmpty)
                    {
                        break;
                    }
                    // Read the data from the worksheet and populate the DataTable
                    for (int row = 2; row <= totalRows; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int col = 1; col < totalColumns; col++)
                        {
                            dataRow[col - 1] = worksheet.Cells[row, col].Value?.ToString();
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                    dataTables.Add(dataTable);
                }
            }
        }
        return dataTables;
    }
    /// <summary>
    /// method to print data tables
    /// </summary>
    /// <param name="dataTable">datatable to print</param>
    public static void PrintDataTable(DataTable dataTable)
    {
        foreach (DataColumn column in dataTable.Columns)
        {
            Debug.WriteLine($"{column.ColumnName}\t");
        }

        foreach (DataRow row in dataTable.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Debug.WriteLine($"{item}\t");
            }
        }
    }

    /// <summary>
    /// Load excel file to a datatable
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>datatable with the values of the excel file</returns>
    public static DataTable LoadExcelFile(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage package = new(new FileInfo(filePath));
        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet
        DataTable dataTable = new DataTable();

        // Load headers
        int totalColumns = worksheet.Dimension.Columns;
        for (int col = 1; col <= totalColumns; col++)
        {
            string? headerText = worksheet.Cells[1, col].Value?.ToString();
            //-------------------------- Under test --------------
            if (headerText.Equals("Date Tested"))
            {
                dataTable.Columns.Add(headerText, typeof(DateTime));
                dataTable.Columns[dataTable.Columns.Count - 1].DateTimeMode = DataSetDateTime.UnspecifiedLocal;
            }
            //-------------------------- ends here --------------
            else
            {
                dataTable.Columns.Add(headerText);
            }
        }

        // Load data rows
        int totalRows = worksheet.Dimension.Rows;
        for (int row = 2; row <= totalRows; row++)
        {
            DataRow dataRow = dataTable.NewRow();
            for (int col = 1; col <= totalColumns; col++)
            {
                var cell = worksheet.Cells[row, col].Value?.ToString();
                //-------------------------- Under test --------------
                if (col - 1 == dataTable.Columns.IndexOf("Date Tested"))
                {
                    try
                    {
                        dataRow[col - 1] = Convert.ToDateTime(cell).ToShortDateString();
                        dataRow[col - 1] = Convert.ToDateTime(cell).Date;
                    }
                    catch (InvalidCastException ex)
                    {
                        Debug.WriteLine(ex.Message + "\nNo se puede convertir");
                    }
                }
                //-------------------------- ends here --------------
                else
                {
                    dataRow[col - 1] = cell;
                }
            }
            dataTable.Rows.Add(dataRow);
        }
        return dataTable;
    }
}

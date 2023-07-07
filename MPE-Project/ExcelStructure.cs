using OfficeOpenXml;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

public class ExcelStructure
{
    public static DataTable LoadExcelFile(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet
            DataTable dataTable = new DataTable();

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
    }
    public static List<DataTable> LoadOfshoreFile(string filePath, string[] targetSheetNames)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        List<DataTable> dataTables = new List<DataTable>();
        using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorkbook workbook = excelPackage.Workbook;
            int headerRow = 1; // Set the header row number width. This is based on the current GIGA report format of headers

            foreach (string sheetName in targetSheetNames)
            {
                ExcelWorksheet worksheet = workbook.Worksheets[sheetName];

                if (worksheet != null)
                {
                    DataTable dataTable = new DataTable(sheetName);
                    //Merged cells
                    var mergedCells = worksheet.MergedCells;
                    
                    foreach(var mergedCell in mergedCells)
                    {
                        var mergedRange = new ExcelAddressBase(mergedCell);

                        //Check if the merged range overlaps with the header row
                        if(mergedRange.Start.Row <= headerRow && mergedRange.End.Row >= headerRow)
                        {
                            // Get the value from the start cells of the merged range
                            var headerText = worksheet.Cells[mergedRange.Start.Row, mergedRange.Start.Column].Value.ToString();
                            dataTable.Columns.Add(headerText);
                        }
                    }

                    DataRow dataRow = dataTable.Rows.Add();
                    PrintDataTable(dataTable);


                    int totalRows = worksheet.Dimension.Rows;
                    int totalColumns = worksheet.Dimension.Columns;
                    /*
                    // Read the data from the worksheet and populate the DataTable
                    for (int row = 1; row <= totalRows; row++)
                    {
                        DataRow dataRow = dataTable.Rows.Add();

                        for (int col = 1; col <= totalColumns; col++)
                        {
                            if (row == 1)
                            {
                                // Set column names in the first row
                                try
                                {
                                    dataTable.Columns.Add(worksheet.Cells[1, col].Value.ToString());
                                }
                                catch (NullReferenceException ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                // Add data to subsequent rows
                                try
                                {
                                    dataRow[col - 1] = worksheet.Cells[row, col].Value;
                                }
                                catch (IndexOutOfRangeException ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }
                            }
                            }
                    }
                    */



                    dataTables.Add(dataTable);
                }
            }
        }

        return dataTables;
    }
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


}

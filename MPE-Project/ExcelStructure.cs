using OfficeOpenXml;
using System;
using System.Data;
using System.Diagnostics;

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

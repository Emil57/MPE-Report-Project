using CsvHelper;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Globalization;
using OfficeOpenXml;

public class CsvFiles
{
    public static DataTable LoadCsvFile(string filePath)
    {
        DataTable dataTable = new DataTable();

        using (var reader  = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // Configure CsvHelper
            //csv.Configuration.HasHeaderRecord = true;

            // Read CSV headers
            csv.Read();
            csv.ReadHeader();
            foreach (string header in csv.HeaderRecord)
            {
                dataTable.Columns.Add(header);
            }

            // Read CSV records
            while (csv.Read())
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < csv.HeaderRecord.Length; i++)
                {
                    dataRow[i] = csv.GetField(i);
                }
                dataTable.Rows.Add(dataRow);
            }
        }
        Debug.WriteLine("CSV File succesfully loaded to DataTable!");
        return dataTable;
    }

    public static void WriteCsvFile(ExcelWorksheet MPEws, string NewMpeFilePath)
    {
        var formatOut = new ExcelOutputTextFormat
        {
            Delimiter = ',',
        };
        if (File.Exists(NewMpeFilePath))
        {
            File.Delete(NewMpeFilePath);
        }
        var file = new FileInfo(NewMpeFilePath);
        MPEws.Cells[1, 1, MPEws.Dimension.Rows, MPEws.Dimension.Columns].SaveToText(file, formatOut);
        Debug.WriteLine("File " + Path.GetFileName(NewMpeFilePath) + " Closed!");
    }

    public static void ExportCsvFile(DataRow[] dataRows, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write header row
            writer.WriteLine(string.Join(",", dataRows[0].Table.Columns.Cast<DataColumn>().Select(col => col.ColumnName)));

            // Write data rows
            foreach (DataRow row in dataRows)
            {
                writer.WriteLine(string.Join(",", row.ItemArray));
            }
        }

        Debug.WriteLine("CSV file exported successfully.");
    }
}
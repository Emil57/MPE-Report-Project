using CsvHelper;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Globalization;

public class CsvFiles
{
    //Structure of each row for csv file
    public string SupplierName = "Skyworks Solutions";
    public string ComponentType = "ASIC";
    public string? APN;
    public string? MPN;
    public string? ProgramName;
    public string? Lot;
    public string? DateCode;
    public string TestStep = "ATE";
    public string? TesterPlatform;
    public string? TesterProgramName;   
    public string? ManufacturingFlow;   
    public string? DateTested;
    public int? LotQty;
    public double? Yield;
    public int? SYL;

    public string? BinX2_Number;
    public string? BinX2_Name;
    public string? BinX2_P;
    public string? BinX2_SBL;

    public static void CSVTable(string filePath)
    {
        Debug.WriteLine("Creating CSV table" + Path.GetFullPath(filePath));
        DataTable mpeRows = LoadCsvFile(Path.GetFullPath(filePath));
        /*
        // Process the loaded rows
        foreach (var row in mpeRows)
        {
            Debug.WriteLine(count + $": {row.SupplierName}, {row.ComponentType}, {row.APN}, {row.MPN}, {row.ProgramName}, {row.Lot}");
            count++;
        }
        */
    }

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
}
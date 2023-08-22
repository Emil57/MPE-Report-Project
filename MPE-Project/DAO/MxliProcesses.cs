using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MPE_Project.Form1;

namespace MPE_Project.DAO
{
    internal class MxliProcesses
    {
        /// <summary>
        /// Start the process for executing MPE Report based on PowerBI files
        /// </summary>
        /// <param name="MpeDataTableReference"></param>
        /// <returns></returns>
        public static DataTable StartMxliSequenceReport(DataTable MpeDataTableReference)
        {
            //MPE Process
            var MpeListOfBins = MpeProcess(MpeDataTableReference);

            //PowerBI Process
            DataTable PowerBIDataTable = ExcelModel.LoadExcelFile(MPEProcesses.FilesPathList["PowerBI FilePath"]);
            PowerBIDataTable = PowerBIProcess(MpeDataTableReference);
            //Search GMAVs in PowerBI
            DataRow[] PowerBIFilteredRowsByPnAndWeek = SearchForGmavsInPowerBIFile(MpeDataTableReference, PowerBIDataTable, MpeListOfBins);
            //Delete duplicates from PowerBI File, deprecated, no longer needed
            //RemoveDuplicates();
            PowerBIFilteredRowsByPnAndWeek = RenameColumnsOfPowerBI(PowerBIDataTable, PowerBIFilteredRowsByPnAndWeek);
            //Sort descending PowerBI file by 'date tested' column 
            //PowerBIFilteredRowsByPnAndWeek = SortDescendingByDateTestedColumn(PowerBIFilteredRowsByPnAndWeek);

            DataTable MxliReport = PowerBIFilteredRowsByPnAndWeek.CopyToDataTable<DataRow>();

            return MxliReport;
        }

        /// <summary>
        /// This method is used to load mpe format base to a data table an extract important features to add in the new report
        /// </summary>
        private static IEnumerable<DataColumn> MpeProcess(DataTable MpeDataTableReference)
        {
            //Step 1: Load MPE file 
            /*//Just for view/debug
            foreach (DataRow row in MpeDataTable.Rows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }*/

            //Step 2: Extract GMAV bins
            // Filter the DataRow's columns to get "Bin" columns
            var MpeListOfBins = MpeDataTableReference.Rows[0].Table.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.EndsWith("_Number"));
            return MpeListOfBins;
        }

        /// <summary>
        /// Process PowerBi File method
        /// </summary>
        private static DataTable PowerBIProcess(DataTable MpeDataTableReference)
        {
            string[] ColumnsToDeleteInPowerBiFile = new String[]
             {
                "CPN", "Test - Volume In", "Test - Volume Out", "Test Yield", "SAP - Volume In", "SAP - Volume Out", "SAP Yield", "Equipment", "SummaryUsed"
             };

            // Step 1: Load PowerBI file
            DataTable PowerBIDataTable = ExcelModel.LoadExcelFile(MPEProcesses.FilesPathList["PowerBI FilePath"]);
            //PowerBIDataTable = LoadExcelFileWithDateTestedAsDateFormat(FilesPathList["PowerBI FilePath"]);

            //Step 2: Remove columns that we don't need for the report
            //These columns of PowerBI file are either empty or not necessary
            foreach (string columnName in ColumnsToDeleteInPowerBiFile)
            {
                try
                {
                    PowerBIDataTable.Columns.Remove(columnName);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            //Step 3: Insert APN and XCode columns in PowerBI file (later exported as MPE report)
            byte ApnColumnIndex = 2, XcodeColumnIndex = 4;
            PowerBIDataTable.Columns.Add("APN", typeof(string)).SetOrdinal(ApnColumnIndex);
            foreach (DataRow row in PowerBIDataTable.Rows)
            {
                row.SetField(ApnColumnIndex, MpeDataTableReference.Rows[0]["APN"].ToString());
                row.SetField(XcodeColumnIndex, MpeDataTableReference.Rows[0]["Program Name"].ToString());
            }
            PowerBIDataTable.AcceptChanges();

            return PowerBIDataTable;
        }
        /// <summary>
        /// This method is used to search GMAVs in the PowerBI file using a list of gmavs of the part number extracted by the MPE file
        /// </summary>
        private static DataRow[] SearchForGmavsInPowerBIFile(DataTable MpeDataTableReference, DataTable PowerBIDataTable, IEnumerable<DataColumn> MpeListOfBins)
        {
            //Step 4: Filter PowerBI file rows by PN and week
            string filter = "[MPN] LIKE '%" + partNumberComboBox.Text + "%' AND [Date Code] LIKE '%" + weeekNumberComboBox.Text + "%'";
            DataRow[] PowerBIFilteredRowsByPnAndWeek = PowerBIDataTable.Select(filter);

            //Step 6: Remove unused bins and keep the needed ones from PowerBI File
            /*For this process, we will look into PowerBI file and delete columns which don't contain the necessary information for the pn report */
            var PowerBIGmavList = PowerBIDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.Contains("Bin")); //Get columns containing 'Bin' word in the header name

            var MpeBinNumberValues = MpeListOfBins
                .Select(column => MpeDataTableReference.Rows[0][column])
                .Where(column => !string.IsNullOrEmpty(column.ToString())); //Get bin_number and bin_name columns with data from mpe file 
            //var alo = MpeBinNumberValues.Where(column => !string.IsNullOrEmpty(column.ToString()));

            //Get the columns to keep in a list. This for PowerBI File
            string Bnumber = "", Bname = "", Bporcentage = "", Btrigger = "";
            List<string> BinColumnsToKeepInPowerBIFile = new();
            foreach (string column in MpeBinNumberValues.Cast<string>())
            {
                Bnumber = "Bin" + column + "_Number";
                BinColumnsToKeepInPowerBIFile.Add(Bnumber);
                Bname = "Bin" + column + "_Name";
                BinColumnsToKeepInPowerBIFile.Add(Bname);
                Bporcentage = "Bin" + column + "_%";
                BinColumnsToKeepInPowerBIFile.Add(Bporcentage);
                Btrigger = "Bin" + column + "_SBL";
                BinColumnsToKeepInPowerBIFile.Add(Btrigger);
            }

            //Get the columns to delete from PowerBi
            var BinColumnsToDeleteInPowerBIFile = PowerBIGmavList
                .Select(column => column.ColumnName)
                .Except(BinColumnsToKeepInPowerBIFile).ToList();

            foreach (string column in BinColumnsToDeleteInPowerBIFile)
            {
                PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(column);
            }
            PowerBIFilteredRowsByPnAndWeek[0].Table.AcceptChanges();
            return PowerBIFilteredRowsByPnAndWeek;
        }
        /// <summary>
        /// Method to remove duplicated lots (deprecated).
        /// </summary>
        /// <param name="PowerBIFilteredRowsByPnAndWeek"></param>
        /// <returns></returns>
        private static DataRow[] RemoveDuplicates(DataTable PowerBIDataTable, DataRow[] PowerBIFilteredRowsByPnAndWeek)
        {
            var duplicateGroupsToDelete = PowerBIFilteredRowsByPnAndWeek.AsEnumerable()
                .GroupBy(row => row.Field<string>("Lot Code"))
                .Where(group => group.Count() > 1);

            foreach (var group in duplicateGroupsToDelete)
            {
                var duplicateRows = group.Skip(1);
                foreach (DataRow rowToDelete in duplicateRows)
                {
                    rowToDelete.Delete();

                    int rowIndex = PowerBIDataTable.Rows.IndexOf(rowToDelete);
                    if (rowIndex >= 0)
                    {
                        PowerBIDataTable.Rows.RemoveAt(rowIndex);
                    }

                }
                PowerBIDataTable.AcceptChanges();
            }
            /*
            //Commit changes on the original DataTable to reflect on the rest of subtables
            foreach(DataRow row in PowerBIFilteredRowsByPnAndWeek)
            {
                if(row.RowState == DataRowState.Detached)
                {
                    row.Delete();
                    row.AcceptChanges();
                }
            }
            */
            return PowerBIFilteredRowsByPnAndWeek;
        }
        /// <summary>
        /// Method to rename bin columns in the power bi datatable and its children
        /// </summary>
        private static DataRow[] RenameColumnsOfPowerBI(DataTable PowerBIDataTable, DataRow[] PowerBIFilteredRowsByPnAndWeek)
        {
            var BinHeaderColumns = PowerBIDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.Contains("Bin")); //Get columns containing '_Number' word in the header name
            short BinNumber = 2;

            for (short BinStart = 0; BinStart < BinHeaderColumns.Count(); BinStart++)
            {
                string bin = "BinX" + BinNumber + "_Number";
                BinHeaderColumns.ElementAt(BinStart).ColumnName = bin;
                BinStart++;
                bin = "BinX" + BinNumber + "_Name";
                BinHeaderColumns.ElementAt(BinStart).ColumnName = bin;
                BinStart++;
                bin = "BinX" + BinNumber + "_%";
                BinHeaderColumns.ElementAt(BinStart).ColumnName = bin;
                BinStart++;
                bin = "BinX" + BinNumber + "_SBL";
                BinHeaderColumns.ElementAt(BinStart).ColumnName = bin;
                BinNumber++;
            }

            //Once we have renamed columns according to the report, we need to create and add new columns with the remaining bins (goes from bin 2 to bin 20)
            for (int i = PowerBIDataTable.Columns.Count; i < 92; i += 4)
            {
                string header = "BinX" + BinNumber + "_Number";
                //PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_Name";
                //PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_%";
                //PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_SBL";
                //PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                BinNumber++;
            }
            PowerBIFilteredRowsByPnAndWeek[0].Table.Columns["Comment"].SetOrdinal(PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Count - 1);
            PowerBIFilteredRowsByPnAndWeek[0].Table.AcceptChanges();

            //Step 2: Round values for yield % column and _% and _SBL columns to 2 decimals
            var firstRowPowerBI = PowerBIFilteredRowsByPnAndWeek[0];
            var IndexInPowerBIDataTable = PowerBIFilteredRowsByPnAndWeek[0].Table.Rows.IndexOf(firstRowPowerBI);
            var ListOfBinsToConvert = PowerBIFilteredRowsByPnAndWeek[0].Table.Columns
                .Cast<DataColumn>()
                .Where(column => ((column.ColumnName.Contains("_%") || column.ColumnName.Contains("_SBL")) && !string.IsNullOrEmpty(PowerBIDataTable.Rows[IndexInPowerBIDataTable][column.ColumnName]
                .ToString())));

            short a = 0;
            foreach (DataRow row in PowerBIFilteredRowsByPnAndWeek)
            {
                object value = row["Yield %"];
                double valueRounded = 0.0;
                try
                {
                    valueRounded = Math.Round(Convert.ToDouble(value.ToString()), 2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    valueRounded = 0.0;
                }
                row["Yield %"] = valueRounded;

                foreach (DataColumn column in ListOfBinsToConvert)
                {
                    value = row[column];
                    try
                    {
                        valueRounded = Math.Round(Convert.ToDouble(value.ToString()), 2);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        valueRounded = 0.0;
                    }
                    row[column] = valueRounded;
                }
                a++;
            }

            return PowerBIFilteredRowsByPnAndWeek;
        }
        /// <summary>
        /// Sort the rows by date tested column from newer to older
        /// </summary>
        /// <param name="PowerBIFilteredRowsByPnAndWeek"></param>
        /// <returns></returns>
        private static DataRow[] SortDescendingByDateTestedColumn(DataRow[] PowerBIFilteredRowsByPnAndWeek)
        {
            List<DataRow> PowerBiReadyToExport = PowerBIFilteredRowsByPnAndWeek.OrderBy(row => (DateTime)row["Date Tested"]).ToList();
            PowerBiReadyToExport.Sort((row1, row2) =>
            {
                DateTime date1 = (DateTime)row1["Date Tested"];
                DateTime date2 = (DateTime)row2["Date Tested"];
                return DateTime.Compare(date2, date1);
            });
            PowerBiReadyToExport.CopyTo(PowerBIFilteredRowsByPnAndWeek, 0);
            return PowerBIFilteredRowsByPnAndWeek;
        }
    }
}

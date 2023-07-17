using System.Diagnostics;
using static ExcelStructure;
using System.Data;
using static CsvFiles;
using System.Drawing.Text;
using System.Collections;
using System.Linq;
using System.Data.Common;
using System.Numerics;
using OfficeOpenXml;
using System.Drawing;
using System.CodeDom;

///<summary>
///This is the main project create MPE reports base on current format used
/// </summary>
namespace MPE_Project
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> FilesPathList = new(); // list of path files involved in the project
        DataTable PowerBIDataTable = new();
        DataTable MpeDataTable = new();
        readonly String[] ListOfColumnsToDelete = new String[]
        {
            "CPN", "Test - Volume In", "Test - Volume Out", "Test Yield", "SAP - Volume In", "SAP - Volume Out", "SAP Yield", "Equipment", "SummaryUsed"
        };
        object xcode = " ", apn = "";
        readonly List<DataTable> OffshoreDataTable = new();
        IEnumerable<DataColumn> MpeListOfBins = new List<DataColumn>();  //variable to support on mpe process
        DataRow[] PowerBIFilteredRowsByPnAndWeek = new DataRow[1];             //variable to support on mpe process
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Button1_Click(object sender, EventArgs e)
        {
            //Call web sites
            //CallWebSite();
            //-------------------------------------------------Main code-------------------------------------------------------//
            //Check the action to perform
            if (radioButton1.Checked)
            {
                //Create MPE
                Debug.WriteLine("Create");
                CreateReport();

                //string[] targets = { comboBox1.Text, comboBox1.Text + "A", comboBox1.Text + "B" };
                //OffshoreDataTable = LoadOfshoreFile(FilesPathList["Offshore FilePath"], targets);
            }
            else
            {
                //Validate
                Debug.WriteLine("Check");
            }
            //-----------------------------------------------------------------------------------------------------------------//
        }
        private void Button7_Click(object sender, EventArgs e)
        {
            //MPE File
            SelectFiles("Select MPE File", "CSV File|*.csv*", "MPE FilePath");
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            // Power BI file
            SelectFiles("Select Power BI File", "Excel File|*.xlsx*", "PowerBI FilePath");
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            //Ofshore file 
            SelectFiles("Select Ofshore File", "Excel File|*.xlsx*", "Ofshore FilePath");
        }
        public void SelectFiles(string title, string filter, string fileType)
        {
            OpenFileDialog ofd = new()
            {
                Title = title,
                Filter = filter
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FilesPathList.Add(fileType, ofd.FileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Catched the error vro and I fixed: " + ex.Message);
                    FilesPathList.Remove(fileType);
                    FilesPathList.Add(fileType, ofd.FileName);
                }
                Debug.WriteLine(fileType + ": " + FilesPathList[fileType]);
                switch (fileType)
                {
                    case "MPE FilePath":
                        textBox3.Text = Path.GetFileName(ofd.FileName);
                        break;
                    case "PowerBI FilePath":
                        textBox2.Text = Path.GetFileName(ofd.FileName);
                        break;
                    case "Offshore FilePath":
                        textBox1.Text = Path.GetFileName(ofd.FileName);
                        break;
                }
            }
        }

        private void CreateReport()
        {
            //MPE Process
            MpeProcess();

            //PowerBI Process
            PowerBIProcess();

            //Merge Process 
            SearchForGmavsInPowerBIFile();

            //Delete duplicates from PowerBI File 
            //No longer needed
            //RemoveDuplicates();

            RenameColumnsOfPowerBI();

            //Export MPE file
            //Get week number 
            string weekNumber = GetWeekNumber();
            string path = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "\\", MpeDataTable.Rows[0]["Program Name"], "_", MpeDataTable.Rows[0]["MPN"], "_", MpeDataTable.Rows[0]["APN"], "_WW", weekNumber, "-mpe-raw.csv");
            ExportCsvFile(PowerBIFilteredRowsByPnAndWeek, path);
            MessageBox.Show("MPE Report Succesfully Done!" + "\n" + "New path: " + path, "Results", MessageBoxButtons.OK);
        }

        private void MpeProcess()
        {
            //Step 1: Load MPE file 
            MpeDataTable = LoadCsvFile(FilesPathList["MPE FilePath"]);
            /*//Just for view/debug
            foreach (DataRow row in MpeDataTable.Rows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }
            //CSVTable(csv); */

            //Step 2: Extract GMAV bins
            DataRow firstRow = MpeDataTable.Rows[0];
            // Filter the DataRow's columns to get "Bin" columns
            MpeListOfBins = firstRow.Table.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.EndsWith("_Number"));
            // Retrieve the values of the filtered columns
            // Output the column values
            /*
            foreach (var value in MpeListOfBins)
            {
                Debug.WriteLine(value);
            }
            */

            //Step 3: Get XCode and APN
            try
            {
                xcode = MpeDataTable.Rows[0]["Program Name"];
                apn = MpeDataTable.Rows[0]["APN"];
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void PowerBIProcess()
        {
            //Here is where we call the main process for PowerBI steps

            // Step 1: Load PowerBI file
            //This file contains all lots data for GMAV part numbers
            PowerBIDataTable = LoadExcelFile(FilesPathList["PowerBI FilePath"]);
            //PowerBIDataTable.Columns["Date Tested"].Expression = "CONVERT('Date Tested', 'System.DateTime')";

            //PrintDataTable(dataTable);

            //Step 2: Filter PowerBI file rows by PN and week
            string filter = "[MPN] LIKE '%" + comboBox1.Text + "%' AND [Date Code] LIKE '%" + comboBox2.Text + "%'";
            PowerBIFilteredRowsByPnAndWeek = PowerBIDataTable.Select(filter);
            /* Just for view/debug
            foreach(DataRow row in PowerBIFilteredRows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }
            */

            //Step 3: Remove columns that we don't need for the report
            //These columns of PowerBI file are either empty or not necessary
            foreach (string columnName in ListOfColumnsToDelete)
            {
                try
                {
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(columnName);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            //Step 4: Insert APN and XCode columns in PowerBI file (later exported as MPE report)
            byte ApnColumnIndex = 2, XcodeColumnIndex = 4;
            PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Add("APN", typeof(string)).SetOrdinal(ApnColumnIndex);
            foreach (DataRow row in PowerBIFilteredRowsByPnAndWeek)
            {
                row.SetField(ApnColumnIndex, apn.ToString());
                row.SetField(XcodeColumnIndex, xcode.ToString());
            }
        }
        /// <summary>
        /// This method is used to search GMAVs in the PowerBI file using a list of gmavs of the part number extracted by the MPE file
        /// </summary>
        private void SearchForGmavsInPowerBIFile()
        {
            //Step 6: Remove unused bins and keep the needed ones from PowerBI File
            /*For this process, we will look into PowerBI file and delete columns which don't contain the necessary information for the pn report */
            var PowerBIGmavList = PowerBIDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.Contains("Bin")); //Get columns containing 'Bin' word in the header name
            var PowerBIGmavValues = PowerBIGmavList.Select(column => PowerBIFilteredRowsByPnAndWeek[0][column]); //Get values from the Bin columns

            var MpeListOfBinNumbers = MpeDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.EndsWith("_Number") && !string.IsNullOrEmpty(MpeDataTable.Rows[0][column].ToString()));  //Header names where there is a gmav for Bin_Number (no empty nor null values)
            var MpeBinNumberValues = MpeListOfBins
                .Select(column => MpeDataTable.Rows[0][column])
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
                PowerBIDataTable.Columns.Remove(column);
            }

        }
        private void RemoveDuplicates()
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
        }
        private void RenameColumnsOfPowerBI()
        {
            //Step 1: Rename bin column names in power bi file
            var BinHeaderColumns = PowerBIDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.Contains("Bin")); //Get columns containing '_Number' word in the header name
            byte BinNumber = 2;

            for (byte BinStart = 0; BinStart < BinHeaderColumns.Count(); BinStart++)
            {
                ; string bin = "BinX" + BinNumber + "_Number";
                //var column = PowerBIFilteredRowsByPnAndWeek[0].Table.Columns;
                BinHeaderColumns.ElementAt(BinStart).ColumnName = bin;
                //column[BinStart].ColumnName = bin;
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
            // Once we have renamed columns according to the report, we need to create and add new columns with the remaining bins (goes from bin 2 to bin 20)
            for (int i = PowerBIDataTable.Columns.Count; i < 92; i += 4)
            {
                string header = "BinX" + BinNumber + "_Number";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(i - 1);
                header = "BinX" + BinNumber + "_Name";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(i - 1);
                header = "BinX" + BinNumber + "_%";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(i - 1);
                header = "BinX" + BinNumber + "_SBL";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(i - 1);
                BinNumber++;
            }

            //Step 2: Round values for yield % column and _% and _SBL columns to 2 decimals
            var firstRowPowerBI = PowerBIFilteredRowsByPnAndWeek[0];
            var IndexInPowerBIDataTable = PowerBIDataTable.Rows.IndexOf(firstRowPowerBI);
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
        }
        private string GetWeekNumber()
        {
            string weekNumber = "";
            foreach (char character in comboBox2.Text)
            {
                if (char.IsNumber(character))
                {
                    weekNumber = string.Concat(weekNumber, character);
                }
            }
            return weekNumber;
        }
        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //When Validate button is checked
            //Disable PowerBi elements in GUI
            label6.Enabled = !radioButton2.Checked;
            textBox2.Enabled = !radioButton2.Checked;
            button5.Enabled = !radioButton2.Checked;
            label3.Enabled = !radioButton2.Checked;
            label4.Enabled = !radioButton2.Checked;
            label5.Enabled = !radioButton2.Checked;
            comboBox1.Enabled = !radioButton2.Checked;
            comboBox2.Enabled = !radioButton2.Checked;
            comboBox3.Enabled = !radioButton2.Checked;
            label2.Enabled = !radioButton2.Checked;
            textBox1.Enabled = !radioButton2.Checked;
            button2.Enabled = !radioButton2.Checked;

        }

    }
}
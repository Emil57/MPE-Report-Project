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
        DataTable PowerBIDataTable = new DataTable();
        DataTable MpeDataTable = new DataTable();
        String[] ListOfColumnsToDelete = new String[]
        {
            "CPN", "Program Name", "Test - Volume In", "Test - Volume Out", "Test Yield", "SAP - Volume In", "SAP - Volume Out", "SAP Yield"
        };
        string xcode,apn = "";
        List<DataTable> OffshoreDataTable = new();
        IEnumerable<DataColumn> MpeBinsFilteredRows = new List<DataColumn>();  //variable to support on mpe process
        DataRow[] PowerBIFilteredRowsByPnAndWeek = new DataRow[1];                          //variable to support on mpe process
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Call web sites
            //CallWebSite();
            //-------------------------------------------------Main code-------------------------------------------------------//
            //Check the action to performe
            if (radioButton1.Checked)
            {
                //Create MPE
                Debug.WriteLine("Create");
                CreateReport();

                //string[] targets = { comboBox1.Text, comboBox1.Text + "A", comboBox1.Text + "B" };
                //OffshoreDataTable = LoadOfshoreFile(FilesPathList["Ofshore Path"], targets);
            }
            else
            {
                //Validate
                Debug.WriteLine("Check");
            }
            //-----------------------------------------------------------------------------------------------------------------//
        }
        private void button7_Click(object sender, EventArgs e)
        {
            //MPE File
            SelectFiles("Select MPE File", "CSV File|*.csv*", "MPE Path");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            // Power BI file
            SelectFiles("Select Power BI File", "Excel File|*.xlsx*", "PowerBI Path");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Ofshore file 
            SelectFiles("Select Ofshore File", "Excel File|*.xlsx*", "Ofshore Path");
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
                FilesPathList.Add(fileType, ofd.FileName);
                Debug.WriteLine(fileType + FilesPathList[fileType]);
                switch (fileType)
                {
                    case "MPE Path":
                        textBox3.Text = Path.GetFileName(ofd.FileName);
                        break;
                    case "PowerBI Path":
                        textBox2.Text = Path.GetFileName(ofd.FileName);
                        break;
                    case "Ofshore Path":
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
            MpeDataTable = LoadCsvFile(FilesPathList["MPE Path"]);
            /*//Just for view/debug
            foreach (DataRow row in MpeDataTable.Rows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }
            //CSVTable(csv); */

            //Step 2: Extract GMAV bins
            DataRow firstRow = MpeDataTable.Rows[0];
            // Filter the DataRow's columns to get "Bin" columns
            MpeBinsFilteredRows = firstRow.Table.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.EndsWith("_Number") || column.ColumnName.EndsWith("_Name"));
            // Retrieve the values of the filtered columns
            // Output the column values
            /*
            foreach (var value in MpeBinsFilteredRows)
            {
                Debug.WriteLine(value);
            }
            */
        }
        private void PowerBIProcess()
        {
            //Here is where we call the main process for PowerBI steps

            // Step 3: Load PowerBI file
            //This file contains all lots data for GMAV part numbers
            PowerBIDataTable = LoadExcelFile(FilesPathList["PowerBI Path"]);
            //PrintDataTable(dataTable);

            //Step 4: Filter PowerBI file rows by PN and week
            string filter = "[MPN] LIKE '%" + comboBox1.Text + "%' AND [Date Code] LIKE '%" + comboBox2.Text + "%'";
            PowerBIFilteredRowsByPnAndWeek = PowerBIDataTable.Select(filter);
            /* Just for view/debug
            foreach(DataRow row in PowerBIFilteredRows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }
            */

            //Step 5: Remove columns that we don't need for the report
            //These columns of PowerBI file are either empty or not necessary
            foreach(string columnName in ListOfColumnsToDelete)
            {
                try
                {
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(columnName);
                }
                catch(IndexOutOfRangeException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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

            var MpeListOfGmavs = MpeDataTable.Columns.Cast<DataColumn>()
               .Where(column => (column.ColumnName.EndsWith("_Number") && MpeDataTable.AsEnumerable()
                   .All(row => !row.IsNull(column) && !string.IsNullOrEmpty(row[column].ToString())))); //Header names where there is a gmav for Bin_Number and Bin_Name (no empty nor null values)
            var MpeBinNumberValues = MpeBinsFilteredRows
                .Select(column => MpeDataTable.Rows[0][column])
                .Where(column => !string.IsNullOrEmpty(column.ToString())); //Get bin_number and bin_name columns with data from mpe file 
            //var alo = MpeBinNumberValues.Where(column => !string.IsNullOrEmpty(column.ToString()));
            int count = 0;

            //Iterate over Gmav column header names
            foreach (var bin in MpeListOfGmavs.Where(column => column.ColumnName.EndsWith("_Number")))
            {

                Debug.WriteLine("MPE column reference: " + bin);
                Debug.WriteLine(MpeDataTable.Rows[0][bin]);
                string BinNumberConcatenationReference = "Bin" + MpeDataTable.Rows[0][bin] + "_Number";

                string BinNumber = PowerBIGmavList.ElementAt(count).ToString();
                string BinName = PowerBIGmavList.ElementAt(count + 1).ToString();
                string BinFailureRate = PowerBIGmavList.ElementAt(count + 2).ToString();
                string BinSbl = PowerBIGmavList.ElementAt(count + 3).ToString();
                //Delete the columns on PowerBI that we don't need 
                while (!BinNumber.Equals(BinNumberConcatenationReference))
                {
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(BinNumber);
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(BinName);
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(BinFailureRate);
                    PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(BinSbl);
                    BinNumber = PowerBIGmavList.ElementAt(count).ToString();
                    BinName = PowerBIGmavList.ElementAt(count + 1).ToString();
                    BinFailureRate = PowerBIGmavList.ElementAt(count + 2).ToString();
                    BinSbl = PowerBIGmavList.ElementAt(count + 3).ToString();
                }
                count += 4;
            }
            //Delete the rest of the columns that we don't need
            count = MpeListOfGmavs.Count() * 4;
            while (count < PowerBIGmavList.Count())
            {
                string columnName = PowerBIGmavList.ElementAt(count).ToString();
                PowerBIFilteredRowsByPnAndWeek[0].Table.Columns.Remove(columnName);
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
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
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
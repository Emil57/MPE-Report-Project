using System.Diagnostics;
using static ExcelStructure;
using System.Data;
using static CsvFiles;
using System.Drawing.Text;
using System.Collections;
using System.Linq;
using System.Data.Common;

namespace MPE_Project
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> FilesPathList = new(); // list of path files involved in the project
        private readonly Dictionary<string, List<GMAVsStructure>> ListOfGMAVs = new Dictionary<string, List<GMAVsStructure>>(); // list of GMAVs by pn
        List<string> Gmavs = new List<string>();
        DataTable PowerBIDataTable = new DataTable();
        DataTable MpeDataTable = new DataTable();
        IEnumerable<DataColumn> MpeBinsFilteredRows = new List<DataColumn>();  //variable to support on mpe process
        IEnumerable<object> MpeGMAVsFilteredValues = new List<object>();        //variable to support on mpe process
        DataRow[] PowerBIFilteredRows = new DataRow[1];                          //variable to support on mpe process
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
            SelectFiles("Select MPE File", "CSV File|*.csv*", "MPE Path", false);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            // Power BI file
            SelectFiles("Select Power BI File", "Excel File|*.xlsx*", "PowerBI Path", true);
        }
        public void SelectFiles(string title, string filter, string fileType, bool op)
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
                if (!op)
                {
                    textBox3.Text = Path.GetFileName(ofd.FileName);
                }
                else
                {
                    textBox2.Text = Path.GetFileName(ofd.FileName);
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
            
            foreach (var value in MpeBinsFilteredRows)
            {
                Debug.WriteLine(value);
            }
            
        }

        private void PowerBIProcess()
        {
            //Here is we call the main process for PowerBI steps

            // Step 3: Load PowerBI file
            //This file contains all lots data for GMAV part numbers
            PowerBIDataTable = LoadExcelFile(FilesPathList["PowerBI Path"]);
            //PrintDataTable(dataTable);

            //Step 4: Filter PowerBI file by PN and week
            string filter = "[MPN] LIKE '%" + comboBox1.Text + "%' AND [Date Code] LIKE '%" + comboBox2.Text + "%'";
            PowerBIFilteredRows = PowerBIDataTable.Select(filter);
            /* Just for view/debug
            foreach(DataRow row in PowerBIFilteredRows)
            {
                Debug.WriteLine(row["Lot Code"]);
            }
            //CSVTable(csv); 
            */

            //Step 5: Remove columns 'CPN' and 'Program Name' from filtered rows
            //These columns of PowerBI file are empty and are not necessary
            PowerBIFilteredRows[0].Table.Columns.Remove("CPN");
            PowerBIFilteredRows[0].Table.Columns.Remove("Program Name");
        }

        private void SearchForGmavsInPowerBIFile()
        {
            //Step 6: Remove unused bins and keep the needed ones from PowerBI File
            /*For this process, we will look into PowerBI file and delete columns which don't contain the necessary information for the pn report */
            var PowerBIGmavList = PowerBIDataTable.Columns
                .Cast<DataColumn>()
                .Where(column => column.ColumnName.Contains("_Number")); //Get columns containing 'Bin' word in the header name
            var PowerBIGmavValues = PowerBIGmavList.Select(column => PowerBIFilteredRows[0][column]); //Get values from the Bin columns

            var MpeListOfGmavs = MpeDataTable.Columns.Cast<DataColumn>()
               .Where(column => (column.ColumnName.EndsWith("_Number") || column.ColumnName.EndsWith("_Name")) && MpeDataTable.AsEnumerable()
                   .All(row => !row.IsNull(column) && !string.IsNullOrEmpty(row[column].ToString()))); //Header names where there is a gmav for Bin_Number and Bin_Name (no empty nor null values)
            var MpeBinNumberValues = MpeBinsFilteredRows
                .Select(column => MpeDataTable.Rows[0][column])
                .Where(column => !string.IsNullOrEmpty(column.ToString())); //Get bin_number and bin_name columns with data from mpe file 
            //var alo = MpeBinNumberValues.Where(column => !string.IsNullOrEmpty(column.ToString()));

            //Iterate over Gmav column header names
            foreach (var bin in MpeListOfGmavs.Where(column => column.ColumnName.EndsWith("_Number")))
            {
                Debug.WriteLine("MPE column reference: " + bin);
                Debug.WriteLine(MpeDataTable.Rows[0][bin]);
                string BinNumberConcatenation = "Bin" + MpeDataTable.Rows[0][bin] + "_Number";
                string BinNameConcatenation = "Bin" + MpeDataTable.Rows[0][bin] + "_Name";
                string BinFailureRateConcatenation = "Bin" + MpeDataTable.Rows[0][bin] + "_%";
                string BinSblConcatenation = "Bin" + MpeDataTable.Rows[0][bin] + "_SBL";
                int count = 0;
                while(count<=PowerBIGmavList.Count())
                {
                    if (PowerBIGmavList.ElementAt(count).Equals(BinNumberConcatenation))
                    {
                        break; 
                    }
                    else
                    {
                        PowerBIFilteredRows[0].Table.Columns.Remove(BinNumberConcatenation);
                        PowerBIFilteredRows[0].Table.Columns.Remove(BinNameConcatenation);
                        PowerBIFilteredRows[0].Table.Columns.Remove(BinFailureRateConcatenation);
                        PowerBIFilteredRows[0].Table.Columns.Remove(BinSblConcatenation);
                    }
                    count++;
                }
            }

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

        }
    }
}
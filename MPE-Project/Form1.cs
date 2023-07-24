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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

///<summary>
///This is the main project create MPE reports base on current format used
/// </summary>
namespace MPE_Project
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> FilesPathList = new(); // list of path files involved in the project

        public Form1()
        {
            InitializeComponent();
            label9.Text = "Version " + Application.ProductVersion;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Method to call either the generating process or validation process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            //Call web sites
            //CallWebSite();
            //-------------------------------------------------Main code-------------------------------------------------------//
            //Check the action to perform
            if (radioButton1.Checked)
            {
                DataTable MpeDataTableReference = new();
                //Create MPE
                if (checkBox2.Checked && checkBox2.Checked)
                {
                    //Mxli & Offshore
                    Debug.WriteLine("Creating MPE Report for Mxli and Offshore");
                    DataTable MxliReport = MxliProcess(MpeDataTableReference);
                    DataTable OffshoreReport = OffshoreProcess(MpeDataTableReference);
                    MxliReport.Merge(OffshoreReport);

                    //Get week number 
                    string weekNumber = GetWeekNumber();
                    string path = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "\\", MpeDataTableReference.Rows[0]["Program Name"], "_", MpeDataTableReference.Rows[0]["MPN"], "_", MpeDataTableReference.Rows[0]["APN"], "_WW", weekNumber, "-mpe-raw.csv");

                    //To export file
                    ExportCsvFile(MxliReport, path);

                    MessageBox.Show("MPE Report Succesfully Done!" + "\n" + "New path: " + path, "Results", MessageBoxButtons.OK);
                }
                else if(checkBox2.Checked) 
                {
                    //Mxli
                    Debug.WriteLine("Creating MPE Report for Mxli");
                    DataTable MxliReport = MxliProcess(MpeDataTableReference);
                }
                else if(checkBox1.Checked)
                {
                    //Offshore
                    Debug.WriteLine("Creating MPE Report for Offshore");
                    DataTable OffshoreReport = OffshoreProcess(MpeDataTableReference);
                }
            }
            else
            {
                //Validate
                Debug.WriteLine("Check");
            }
            //-----------------------------------------------------------------------------------------------------------------//
        }
        /// <summary>
        /// Method to call the select files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            SelectFiles("Select Ofshore File", "Excel File|*.xlsx*", "Offshore FilePath");
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

        private DataTable MxliProcess(DataTable MpeDataTableReference)
        {
            //MPE Process
            var MpeListOfBins = MpeProcess(MpeDataTableReference);

            //PowerBI Process
            DataTable PowerBIDataTable = LoadExcelFile(FilesPathList["PowerBI FilePath"]);
            DataRow[] PowerBIFilteredRowsByPnAndWeek = PowerBIProcess(MpeDataTableReference);
            //Search GMAVs in PowerBI
            PowerBIFilteredRowsByPnAndWeek = SearchForGmavsInPowerBIFile(MpeDataTableReference, PowerBIDataTable, PowerBIFilteredRowsByPnAndWeek, MpeListOfBins);
            //Delete duplicates from PowerBI File, deprecated, no longer needed
            //RemoveDuplicates();
            PowerBIFilteredRowsByPnAndWeek = RenameColumnsOfPowerBI(PowerBIDataTable, PowerBIFilteredRowsByPnAndWeek);
            //Sort descending PowerBI file by 'date tested' column 
            PowerBIFilteredRowsByPnAndWeek = SortDescendingByDateTestedColumn(PowerBIFilteredRowsByPnAndWeek);

            DataTable MxliReport = PowerBIFilteredRowsByPnAndWeek.CopyToDataTable<DataRow>();

            return MxliReport; 
        }
        /// <summary>
        /// This method is used to load mpe format base to a data table an extract important features to add in the new report
        /// </summary>
        private IEnumerable<DataColumn> MpeProcess(DataTable MpeDataTableReference)
        {
            //Step 1: Load MPE file 
            MpeDataTableReference = LoadCsvFile(FilesPathList["MPE FilePath"]);
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
        private DataRow[] PowerBIProcess(DataTable MpeDataTableReference)
        {
            string[] ColumnsToDeleteInPowerBiFile = new String[]
             {
                "CPN", "Test - Volume In", "Test - Volume Out", "Test Yield", "SAP - Volume In", "SAP - Volume Out", "SAP Yield", "Equipment", "SummaryUsed"
             };

            // Step 1: Load PowerBI file
            DataTable PowerBIDataTable = LoadExcelFile(FilesPathList["PowerBI FilePath"]);
            //PowerBIDataTable = LoadExcelFileWithDateTestedAsDateFormat(FilesPathList["PowerBI FilePath"]);

            //Step 2: Filter PowerBI file rows by PN and week
            string filter = "[MPN] LIKE '%" + comboBox1.Text + "%' AND [Date Code] LIKE '%" + comboBox2.Text + "%'";
            DataRow[] PowerBIFilteredRowsByPnAndWeek = PowerBIDataTable.Select(filter);

            //Step 3: Remove columns that we don't need for the report
            //These columns of PowerBI file are either empty or not necessary
            foreach (string columnName in ColumnsToDeleteInPowerBiFile)
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
                row.SetField(ApnColumnIndex, MpeDataTableReference.Rows[0]["APN"].ToString());
                row.SetField(XcodeColumnIndex, MpeDataTableReference.Rows[0]["Program Name"].ToString());
            }
            return PowerBIFilteredRowsByPnAndWeek;
        }
        /// <summary>
        /// This method is used to search GMAVs in the PowerBI file using a list of gmavs of the part number extracted by the MPE file
        /// </summary>
        private DataRow[] SearchForGmavsInPowerBIFile(DataTable MpeDataTableReference, DataTable PowerBIDataTable,DataRow[] PowerBIFilteredRowsByPnAndWeek, IEnumerable<DataColumn> MpeListOfBins)
        {
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
                PowerBIDataTable.Columns.Remove(column);
            }
            return PowerBIFilteredRowsByPnAndWeek;
        }
        /// <summary>
        /// Method to remove duplicated lots (deprecated).
        /// </summary>
        /// <param name="PowerBIFilteredRowsByPnAndWeek"></param>
        /// <returns></returns>
        private DataRow[] RemoveDuplicates(DataTable PowerBIDataTable, DataRow[] PowerBIFilteredRowsByPnAndWeek)
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
        private DataRow[] RenameColumnsOfPowerBI(DataTable PowerBIDataTable, DataRow[] PowerBIFilteredRowsByPnAndWeek)
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
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_Name";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_%";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                header = "BinX" + BinNumber + "_SBL";
                PowerBIDataTable.Columns.Add(header).SetOrdinal(PowerBIDataTable.Columns.Count - 1);
                BinNumber++;
            }
            PowerBIDataTable.Columns["Comment"].SetOrdinal(PowerBIDataTable.Columns.Count - 1);

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

            return PowerBIFilteredRowsByPnAndWeek; 
        }
        /// <summary>
        /// Sort the rows by date tested column from newer to older
        /// </summary>
        /// <param name="PowerBIFilteredRowsByPnAndWeek"></param>
        /// <returns></returns>
        private DataRow[] SortDescendingByDateTestedColumn(DataRow[] PowerBIFilteredRowsByPnAndWeek)
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
        private DataTable OffshoreProcess(DataTable MpeDataTable)
        {
            //Get the datatables of each worksheets
            string[] sheetNameTargets = { comboBox1.Text, comboBox1.Text + "A", comboBox1.Text + "B" };
            MpeDataTable = LoadCsvFile(FilesPathList["MPE FilePath"]);
            List<DataTable> ListOfDataTables = LoadOffshoreFile(FilesPathList["Offshore FilePath"], sheetNameTargets);

            DataTable NewMpeReport = MpeDataTable.Clone(); //copy header names
            string[] offshoreColumnNames =
            {
                "TESTDATE", "PRODUCT", "SWKS LOTNO", "GS LOTNO", "TestProgram", "INPUT QTY", "PASS QTY", "YIELD"
            };

            foreach (DataTable OffshoreDataTableCurrent in ListOfDataTables)
            {
                //Move data from each offshore datatable to mpe report
                foreach (DataRow dataRowOffshoreToIterate in OffshoreDataTableCurrent.Rows)
                {
                    if (dataRowOffshoreToIterate.Table.Rows.IndexOf(dataRowOffshoreToIterate) != 0)
                    {
                        DataRow dataRowNewMpe = NewMpeReport.NewRow();

                        //Different data in rows
                        dataRowNewMpe["Date Tested"] = dataRowOffshoreToIterate[offshoreColumnNames[0]];
                        dataRowNewMpe["Lot Code"] = dataRowOffshoreToIterate[offshoreColumnNames[2]].ToString();
                        dataRowNewMpe["Test Program Name"] = dataRowOffshoreToIterate[offshoreColumnNames[4]].ToString();
                        dataRowNewMpe["Lot Qty"] = dataRowOffshoreToIterate[offshoreColumnNames[5]].ToString();

                        double yield = Math.Round(Convert.ToDouble(dataRowOffshoreToIterate[offshoreColumnNames[7]].ToString()) * 100, 2);
                        dataRowNewMpe["Yield %"] = yield.ToString();

                        //Fill in the non-bin empty columns the information that is duplicated
                        dataRowNewMpe["Supplier Name"] = "Skyworks Solutions";
                        dataRowNewMpe["Component Type"] = "ASIC";
                        dataRowNewMpe["APN"] = MpeDataTable.Rows[0]["APN"];
                        dataRowNewMpe["Program Name"] = MpeDataTable.Rows[0]["Program Name"];
                        dataRowNewMpe["Test Step"] = MpeDataTable.Rows[0]["Test Step"];
                        dataRowNewMpe["Manufacturing Flow"] = MpeDataTable.Rows[0]["Manufacturing Flow"];
                        dataRowNewMpe["SYL"] = MpeDataTable.Rows[0]["SYL"];

                        //get data from bins
                        var MpeListOfBinNumbers = MpeDataTable.Columns
                            .Cast<DataColumn>()
                            .Where(column => column.ColumnName.EndsWith("_Number") && !string.IsNullOrEmpty(MpeDataTable.Rows[0][column].ToString()));  //Header names where there is a gmav for Bin_Number (no empty nor null values)

                        var OffshoreBinColumns = NewMpeReport.Columns
                            .Cast<DataColumn>()
                            .Where(column => column.ColumnName.Contains("BIN") && column.ColumnName.EndsWith("3") && !string.IsNullOrEmpty(MpeDataTable.Rows[0][column].ToString()));  //Header names where there is a gmav for Bin_Number (no empty nor null values)

                        foreach (DataColumn MpeBinNumberValue in MpeListOfBinNumbers)
                        {
                            string BinNumber = "BIN" + MpeDataTable.Rows[0][MpeBinNumberValue.ColumnName];
                            var offshoreColumnNumber = OffshoreDataTableCurrent.Columns
                                .Cast<DataColumn>()
                                .Where(column => column.ColumnName.StartsWith(BinNumber)); //get the columns with the bin number

                            string filter = "[SWKS LOTNO] LIKE '%" + dataRowNewMpe["Lot Code"] + "%'"; //filter on offshore datatable
                            DataRow dataRowToGetDataFromOffshore = OffshoreDataTableCurrent.Select(filter).Single<DataRow>(); //get datarow of offshore datatable by lot

                            short indexOfstring = (short)MpeBinNumberValue.ColumnName.IndexOf("_");
                            string substringBinX = MpeBinNumberValue.ColumnName.Substring(0, indexOfstring + 1);
                            string concat; double doubleToConvert;

                            foreach (DataColumn dataColumn in offshoreColumnNumber)
                            {
                                if (dataColumn.ColumnName.EndsWith("]"))
                                {
                                    concat = substringBinX + "SBL";
                                    doubleToConvert = Math.Round(Convert.ToDouble(dataRowToGetDataFromOffshore[dataColumn.ColumnName].ToString()) * 100, 2);
                                    dataRowNewMpe[concat] = doubleToConvert.ToString();
                                }
                                else if (dataColumn.ColumnName.EndsWith("3"))
                                {
                                    concat = substringBinX + "%";
                                    doubleToConvert = Math.Round(Convert.ToDouble(dataRowToGetDataFromOffshore[dataColumn.ColumnName].ToString()) * 100, 2);
                                    dataRowNewMpe[concat] = doubleToConvert.ToString();
                                }
                            }

                            //add bin number
                            dataRowNewMpe[MpeBinNumberValue.ColumnName] = MpeDataTable.Rows[0][MpeBinNumberValue.ColumnName].ToString();
                            //add bin name
                            concat = substringBinX + "Name";
                            dataRowNewMpe[concat] = MpeDataTable.Rows[0][concat].ToString();
                        }
                        NewMpeReport.Rows.Add(dataRowNewMpe);
                    }
                }
            }

            return NewMpeReport;
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
            label2.Enabled = !radioButton2.Checked;
            textBox1.Enabled = !radioButton2.Checked;
            button2.Enabled = !radioButton2.Checked;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label6.Enabled = checkBox2.Checked;
            textBox2.Enabled = checkBox2.Checked;
            button5.Enabled = checkBox2.Checked;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = checkBox1.Checked;
            textBox1.Enabled = checkBox1.Checked;
            button2.Enabled = checkBox1.Checked;
        }
    }
}
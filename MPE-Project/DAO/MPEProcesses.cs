
using System.Data;
using System.Diagnostics;
using static MPE_Project.Form1;
using static ExcelModel;
using static CsvModel;

namespace MPE_Project.DAO
{
    internal class MPEProcesses
    {
        /// <summary>
        /// Variable to store excel and csv files for execution of the program
        /// </summary>
        public static readonly Dictionary<string, string> FilesPathList = new(); 

        /// <summary>
        /// Load, identify and store files used to execute the program.
        /// </summary>
        /// <param name="title">Title of the file dialog</param>
        /// <param name="filter">Filters the type of file</param>
        /// <param name="fileType">The key to store in FilesPathList</param>
        public static void SelectFiles(string title, string filter, string fileType)
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

        public static void StartMPEProgram()
        {
            //Call web sites
            //CallWebSite();
            //-------------------------------------------------Main code-------------------------------------------------------//
            //Check the action to perform
            if (createRadioButton.Checked)
            {
                DataTable MpeDataTableReference = CsvModel.LoadCsvFile(FilesPathList["MPE FilePath"]);
                DataTable DataReport = new DataTable();
                //Create MPE
                if (offshoreCheckBox.Checked && mxliCheckBox.Checked)
                {
                    //Mxli & Offshore
                    Debug.WriteLine("Creating MPE Report for Mxli and Offshore");
                    DataReport = MxliProcesses.StartMxliSequenceReport(MpeDataTableReference);
                    DataTable OffshoreReport = GigaOffshoreProcesses.OffshoreProcess(MpeDataTableReference);
                    DataReport.Merge(OffshoreReport);
                }
                else if (mxliCheckBox.Checked)
                {
                    //Mxli
                    Debug.WriteLine("Creating MPE Report for Mxli");
                    DataReport = MxliProcesses.StartMxliSequenceReport(MpeDataTableReference);
                }
                else if (offshoreCheckBox.Checked)
                {
                    //Offshore
                    Debug.WriteLine("Creating MPE Report for Offshore");
                    DataReport = GigaOffshoreProcesses.OffshoreProcess(MpeDataTableReference);
                }

                //Get week number 
                string weekNumber =  GetWeekNumber();
                string path = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "\\", MpeDataTableReference.Rows[0]["Program Name"], "_", MpeDataTableReference.Rows[0]["MPN"], "_", MpeDataTableReference.Rows[0]["APN"], "_WW", weekNumber, "-mpe-raw.csv");
                
                if (DataReport.Rows.Count > 0)
                {
                    //To export file
                    ExportCsvFile(DataReport, path);
                    MessageBox.Show("MPE Report Succesfully Done!" + "\n" + "New path: " + path, "Results", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("No data available in this file to create a report!\n" + "Try another either offshore or power bi file", "Results", MessageBoxButtons.OK);
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
        /// Extract the week number from the comboBox.
        /// </summary>
        /// <returns></returns>
        private static string GetWeekNumber()
        {
            string weekNumber = "";
            foreach (char character in weekNumberComboBox.Text)
            {
                if (char.IsNumber(character))
                {
                    weekNumber = string.Concat(weekNumber, character);
                }
            }
            return weekNumber;
        }
    }
}

using System.Diagnostics;
using System.Data;
using System.Drawing.Text;
using System.Collections;
using System.Linq;
using System.Data.Common;
using System.Numerics;
using OfficeOpenXml;
using System.Drawing;
using System.CodeDom;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using static ExcelModel;
using static CsvModel;
using static MPE_Project.DAO.MPEProcesses;
using MPE_Project.DAO;

///<summary>
///This is the main project create MPE reports base on current format used
/// </summary>
namespace MPE_Project
{
    public partial class Form1 : Form
    {
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
            MPEProcesses.StartMPEProgram();
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
            SelectFiles("Select Offshore File", "Excel File|*.xlsx*", "Offshore FilePath");
        }
        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //When Validate button is checked
            //Disable PowerBi elements in GUI
            label6.Enabled = !validateRadioButton.Checked;
            textBox2.Enabled = !validateRadioButton.Checked;
            button5.Enabled = !validateRadioButton.Checked;
            label3.Enabled = !validateRadioButton.Checked;
            label4.Enabled = !validateRadioButton.Checked;
            label5.Enabled = !validateRadioButton.Checked;
            partNumberComboBox.Enabled = !validateRadioButton.Checked;
            weeekNumberComboBox.Enabled = !validateRadioButton.Checked;
            label2.Enabled = !validateRadioButton.Checked;
            textBox1.Enabled = !validateRadioButton.Checked;
            button2.Enabled = !validateRadioButton.Checked;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label6.Enabled = mxliCheckBox.Checked;
            textBox2.Enabled = mxliCheckBox.Checked;
            button5.Enabled = mxliCheckBox.Checked;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = offshoreCheckBox.Checked;
            textBox1.Enabled = offshoreCheckBox.Checked;
            button2.Enabled = offshoreCheckBox.Checked;
        }
    }
}
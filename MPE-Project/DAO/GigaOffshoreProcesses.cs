using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MPE_Project.Form1;

namespace MPE_Project.DAO
{
    internal class GigaOffshoreProcesses
    {
        public static DataTable OffshoreProcess(DataTable MpeDataTable)
        {
            //Get the datatables of each worksheets
            DataTable NewMpeReport = MpeDataTable.Clone(); //copy header names
            string[] sheetNameTargets = { partNumberComboBox.Text, partNumberComboBox.Text + "A", partNumberComboBox.Text + "B" };
            List<DataTable> ListOfDataTables = ExcelModel.LoadOffshoreFile(MPEProcesses.FilesPathList["Offshore FilePath"], sheetNameTargets);
            if (ListOfDataTables.Count == 0)
            {
                return NewMpeReport;
            }
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
                        dataRowNewMpe["Tester Platform"] = GetTestPlatform(dataRowNewMpe["Test Program Name"].ToString());
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
                        dataRowNewMpe["MPN"] = MpeDataTable.Rows[0]["MPN"];

                        //get data from bins
                        var MpeListOfBinNumbers = MpeDataTable.Columns
                            .Cast<DataColumn>()
                            .Where(column => column.ColumnName.EndsWith("_Number") && !string.IsNullOrEmpty(MpeDataTable.Rows[0][column].ToString()));  //Header names where there is a gmav for Bin_Number (no empty nor null values)

                        var OffshoreBinColumns = NewMpeReport.Columns
                            .Cast<DataColumn>()
                            .Where(column => column.ColumnName.Contains("BIN") && column.ColumnName.EndsWith("3") && !string.IsNullOrEmpty(MpeDataTable.Rows[0][column].ToString()));  //Header names where there is a gmav for Bin_Number (no empty nor null values)

                        foreach (DataColumn MpeBinNumberValue in MpeListOfBinNumbers)
                        {
                            string BinNumber = "BIN" + MpeDataTable.Rows[0][MpeBinNumberValue.ColumnName] + "[";
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
                                if (dataColumn.ColumnName.EndsWith("]1"))
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

        private static string GetTestPlatform(string testProgram)
        {
            string platform = "";
            if (testProgram.Contains("A93"))
            {
                platform = "A93";
            }
            else if (testProgram.Contains("DRG") | testProgram.Contains("RED") | testProgram.Contains("RDB"))
            {
                platform = "DRAGON";
            }
            else if (testProgram.Contains("FIT") | testProgram.Contains("FAST"))
            {
                platform = "FAST";
            }
            else
            {
                platform = "0";
            }
            return platform;
        }

    }
}

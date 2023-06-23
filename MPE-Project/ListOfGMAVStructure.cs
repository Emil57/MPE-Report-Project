using System;

public class GMAVsStructure
{
	public string Bin_Number;
    public string Bin_Name;
    public string Bin_FailRate;
    public string Bin_SBL;


    public GMAVsStructure(string Bin_Number, string Bin_Name, string Bin_FailRate, string Bin_SBL)
    {
        this.Bin_Number = Bin_Number;
        this.Bin_Name = Bin_Name;
        this.Bin_FailRate = Bin_FailRate;
        this.Bin_SBL = Bin_SBL;

    }

    public string getBinNumber()
    {
        return this.Bin_Number;
    }

    public string getNameOfGMAV()
    {
        return this.Bin_Name;
    }

}

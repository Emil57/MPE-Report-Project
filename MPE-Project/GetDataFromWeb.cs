using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Threading;

public class WebDriver
{
	public WebDriver()
	{
	}

    public static void CallWebSite()
    {
        Debug.WriteLine("me llamaste vro");
        // Set the path to the ChromeDriver/Edge executable
        string driverPath = "C:\\Users\\trejode\\Desktop";
        //Debug.WriteLine(driverPath);
        // Create a new instance of the ChromeDriver/EdgeDriver
        IWebDriver driver = new EdgeDriver(driverPath);

        //DownloadSBLs(driver);
        //DownloadPowerBI(driver);
        GetSYL(driver, "SKY58440-11");

    }
    public static void DownloadSBLs(IWebDriver driver)
    {
        try
        {
            // Use the driver to automate browser actions
            //Browse to SBL page - Mexicali Metrics Portal
            driver.Navigate().GoToUrl("http://mexshrsqlprod05/ReportServer/Pages/ReportViewer.aspx?%2fMavericksReports%2fMavericksList&rs:Command=Render");
            Thread.Sleep(5000);

            // Find and interact with web elements
            //Dropdown menu
            IWebElement webElement = driver.FindElement(By.Id("ReportViewerControl_ctl05_ctl04_ctl00"));
            webElement.Click();
            
            // Download excel file
            webElement = driver.FindElement(By.LinkText("Excel 2003"));
            webElement.Click();

            Thread.Sleep(5000);
        }
        finally
        {
            // Quit the browser and dispose of the driver instance
            driver.Quit();
            driver.Dispose();
        }
    }
    public static void DownloadPowerBI(IWebDriver driver)
    {
        try
        {
            //Browse to PowerBI Report to extract data file
            driver.Navigate().GoToUrl("https://app.powerbi.com/groups/8009f87f-82b7-4915-9239-dccc5de2f7ce/reports/58ce2dd9-c259-4311-8d00-3938d61d29dc/ReportSection?experience=power-bi");
            Thread.Sleep(8000); //wait to load completely the website
            // Find and interact with web elements
            //Dropdown menu
            IWebElement webElement = driver.FindElement(By.Id("exportMenuBtn"));
            webElement.Click();

            webElement = driver.FindElement(By.CssSelector(".mat-menu-item.appBarMatMenu"));
            webElement.Click();
        }
        finally
        {
            // Quit the browser and dispose of the driver instance
            driver.Quit();
            driver.Dispose();
        }
    }
    public static void GetSYL(IWebDriver driver, string partnumber)
    {
        try
        {
            //Browse to TheWeb
            driver.Navigate().GoToUrl("http://mexvmprodapps01:83/Engineering/DevEng/ProductInformation/");
            Thread.Sleep(8000); //wait to load completely the website
            // Find and interact with web elements
            //Search Bar
            IWebElement webElement = driver.FindElement(By.Name("SearchStr"));
            webElement.SendKeys(partnumber);
            webElement.SendKeys(OpenQA.Selenium.Keys.Enter);

            webElement = driver.FindElement(By.LinkText("Products"));
            webElement.SendKeys(OpenQA.Selenium.Keys.Enter);

            //PartNumber
            webElement = driver.FindElement(By.LinkText(partnumber));
            webElement.SendKeys(OpenQA.Selenium.Keys.Enter);
        }
        finally
        {
            // Quit the browser and dispose of the driver instance
            driver.Quit();
            driver.Dispose();
        }
    }
}
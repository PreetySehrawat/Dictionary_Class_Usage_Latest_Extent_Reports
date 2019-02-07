using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using SPWorkApp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using static SPWorkApp.Global.CommonMethods;
using AventStack.ExtentReports.Reporter;

namespace SPWorkApp.Global
{
    public class Base
    {
        #region To access Path from resource file
        public static int Browser = Int32.Parse(SPResources.Browser);
        public static string ExcelPath = SPResources.ExcelPath;
        public static string ScreenshotPath = SPResources.ScreenShotPath;
        public static string ReportPath = SPResources.ReportPath;
        #endregion


        #region reports
        public static ExtentTest test;
        public static ExtentReports extent;
        public static ExtentHtmlReporter htmlReporter;
        #endregion

        #region CompareResults
        public static void CompareResults(string actualValue, string expectedValue, string validationText)
        {
            if (actualValue == expectedValue)
            {
                Base.test.Log(Status.Pass, validationText + " validation is successful");
            }
            else
            {
                Base.test.Log(Status.Fail, validationText + " validation is not successful");
                SaveScreenShotClass.SaveScreenshot(Global.Driver.driver, validationText);
            }
        }

        public static void LogPageValidation(string validationText, string result)
        {
            String img = SaveScreenShotClass.SaveScreenshot(Global.Driver.driver, validationText);
            if (result == "Pass")
            {
                Base.test.Log(Status.Pass, validationText + " validation is successful, Screenshot: " + img);
            }
            else
            {
                Base.test.Log(Status.Fail, validationText + " validation is not successful, Screenshot: " + img);
            }
        }

        public static void LogPageValidation(string validationText, string result, Exception e)
        {
            Base.test.Log(Status.Fail, validationText + " validation is not successful with error message as " + e.Message);
        }

        #endregion

        #region setup and tear down
        [SetUp]
        public void Inititalize()
        {


            switch (Browser)
            {
                case 1:
                    Driver.driver = new FirefoxDriver();
                    break;

                case 2:
                    var options = new ChromeOptions();
                    options.AddArguments("--disable-extensions --disable-extensions-file-access-check --disable-extensions-http-throttling --disable-infobars --enable-automation --start-maximized");
                    options.AddUserProfilePreference("credentials_enable_service", false);
                    options.AddUserProfilePreference("profile.password_manager_enabled", false);
                    Driver.driver = new ChromeDriver(options);
                    break;

            }

            extent = new ExtentReports();
            htmlReporter = new ExtentHtmlReporter(SPResources.ReportPath);
            extent.AttachReporter(htmlReporter);

        }


        [TearDown]
        public void TearDown()
        {
            // Screenshot
            String img = SaveScreenShotClass.SaveScreenshot(Driver.driver, "TearDown");
            test.Log(Status.Info, "Screen shot: " + img);

            // end test. (Reports)
            extent.RemoveTest(test);
            
            // calling Flush writes everything to the log file (Reports)
            extent.Flush();

            // Close the driver            
            Driver.driver.Close();

        }
        #endregion
    }
}
using OpenQA.Selenium;
using SPWorkApp.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SPWorkApp.Pages
{
    class JobCompletionForm
    {

        string ExcelPath = @"C:\Users\Preety.Sehrawat\Desktop\SPWorkApp\Test Data\Test_Completion_Code.xlsx";

        IDictionary<string, string> completionRule = new Dictionary<string, string>();
        IDictionary<string, string> breakdowncodeRule = new Dictionary<string, string>();

        string completionCode = null, breakdownCategory = null, breakdownCode = null;

        public void Login()
        {
            string url = "https://cadtest.experieco.com/SPWorkApp/Login/";

            Driver.driver.Navigate().GoToUrl(url);
            Driver.driver.Manage().Window.Maximize();

            string username = "1111";
            string password = "1111";

            Driver.driver.FindElement(By.XPath("//*[@id=\"SPLogin\"]")).SendKeys(username);
            Driver.driver.FindElement(By.XPath("//*[@id=\"Password\"]")).SendKeys(password);
            Driver.driver.FindElement(By.XPath("//*[@id=\"loginbutton\"]")).Click();
            Thread.Sleep(10000);

            Driver.driver.FindElement(By.XPath("//*[@id=\"jobComplete\"]")).Click();
        }

        public void ReadTestData()
        {
            Driver.driver.FindElement(By.XPath("//*[@id=\"completioncodes\"]")).Click();

            //Read test data from excel sheet
            Global.CommonMethods.ExcelLib.CollectionFromExcel(ExcelPath, "TestData");
            completionCode = Global.CommonMethods.ExcelLib.ReadExcelData(2, "Completion Code");
            breakdownCategory = Global.CommonMethods.ExcelLib.ReadExcelData(2, "Breakdown Category");
            breakdownCode = Global.CommonMethods.ExcelLib.ReadExcelData(2, "Breakdown Code 1");
        }

        public void EnterCompCodeandBrdwnCtgry()
        {
            //Select Completion Code 
            string gridXpath = "//*[@id=\"grid_completioncodes\"]";
            string bXpath = "//*[@id=\"gridItem_completioncodes_";
            string aXPath = "\"]/div/div/h1";
            bool skipbyone = true;
            SelectFromGrid(gridXpath, completionCode, bXpath, aXPath, skipbyone);

            //Select break down category
            gridXpath = "//*[@id=\"grid_breakdowncategory1\"]";
            bXpath = "//*[@id=\"gridItem_breakdowncategory1_";
            aXPath = "\"]";
            skipbyone = false;
            Thread.Sleep(2000);
            Driver.driver.FindElement(By.XPath("//*[@id=\"breakdowncategory1\"]")).Click();
            Thread.Sleep(2000);
            SelectFromGrid(gridXpath, breakdownCategory, bXpath, aXPath, skipbyone);
            Thread.Sleep(5000);
        }

        public void Check_MandFldInRule_VrfyAlert_EditFld()
        {
            bool IsAlertRaised = false, checkforbreakdownrule = false, matchfound = false;
            string alertText = null;
            string mode = "Edit";

            //Read each field in the rule and look for Mandatory field
            for (int i = 0; i < completionRule.Count; i++)
            {
                if (completionRule[completionRule.Keys.ElementAt(i)].Contains("Mand"))
                {
                    //If Mandatory field then click on Send button to check whether an alert is raised or not
                    ClickOnSendBtn();

                    IsAlertRaised = CheckForAlert(out alertText);

                    //If alert is raised then compare the field name with alert text
                    if (IsAlertRaised)
                    {
                        string str = completionRule.Keys.ElementAt(i).Substring(0, 5).ToLower();

                        if (alertText.Contains(str))
                        {
                            //if alert is raised for the mandatory field then edit the field and provide input data
                            if (completionRule.Keys.ElementAt(i) == "Breakdown Code 1")
                                checkforbreakdownrule = true;
                            Check_EditMandFldOnScreen(completionRule.Keys.ElementAt(i), breakdownCode, mode);
                            matchfound = true;
                        }
                        else
                        {
                            //Else part means Alert is raised, but not for selected mandatory field
                            //Verify if Mandatory field has some data
                            mode = "Read";
                            Check_EditMandFldOnScreen(completionRule.Keys.ElementAt(i), breakdownCode, mode);
                            matchfound = false;
                        }
                        if (matchfound == true)
                            continue;
                    }
                    if (checkforbreakdownrule)
                    {
                        ReadBreakdownCodeRules();
                        ClickOnSendBtn();
                        IsAlertRaised = CheckForAlert(out alertText);
                        checkforbreakdownrule = false;
                    }

                    for (int j = 0; j < breakdowncodeRule.Count; j++)
                    {
                        string str3 = breakdowncodeRule.Keys.ElementAt(j).Substring(0, 5).ToLower();
                        if (alertText.Contains(str3))
                        {
                            mode = "Edit";
                            matchfound = true;
                            Check_EditMandFldOnScreen(breakdowncodeRule.Keys.ElementAt(j), breakdownCode, mode);
                            break;
                        }
                        else
                        {
                            //Else part means Alert is raised, but not for selected mandatory field
                            //Verify if Mandatory field has some data
                            mode = "Read";
                            Check_EditMandFldOnScreen(completionRule.Keys.ElementAt(i), breakdownCode, mode);
                            matchfound = false;
                        }
                    }
                }
            }
        }

        public void ClickOnSendBtn()
        {
            //Click on Send Button
            Driver.driver.FindElement(By.XPath("//*[@id=\"sendAndCompleteJob\"]")).Click();
        }

        public bool CheckForAlert(out string MessageText)
        {
            bool IsAlertRaised = false;
            MessageText = null;
            IsAlertRaised = Driver.ElementVisible(Driver.driver, "XPath", "/html[1]/body[1]/div[3]/div[1]/div[2]");
            if (IsAlertRaised)
            {
                MessageText = Driver.driver.FindElement(By.XPath("/html[1]/body[1]/div[3]/div[1]/div[2]")).Text.ToLower();
            }

            Driver.driver.FindElement(By.XPath("/html[1]/body[1]/div[3]/div[1]/div[2]")).Click();
            return IsAlertRaised;
        }

        public void SelectFromGrid(string gridXpath, string exceldata, string bxPath, string axPath, bool skipbyone)
        {
            Thread.Sleep(500);
            string allgridtext = Driver.driver.FindElement(By.XPath(gridXpath)).Text;
            string[] gridtextList = Regex.Split(allgridtext, "\r\n");

            int j = 1;

            for (int i = 0; i < gridtextList.Length; i++)
            {
                if (gridtextList[i] == exceldata)
                {
                    Driver.driver.FindElement(By.XPath(bxPath + j + axPath)).Click();
                    break;
                }
                if (skipbyone)
                    i = i + 1;
                j = j + 1;
            }
            Thread.Sleep(3000);
        }


        public void ReadCompletionRule()
        {
            int noofrows = 0, noofcolumns = 0;
            Global.CommonMethods.ExcelLib.CollectionFromExcel(ExcelPath, "CompletionCode", out noofrows, out noofcolumns);

            for (int i = 2; i <= noofrows; i++)
            {
                string cc = Global.CommonMethods.ExcelLib.ReadExcelData(i, "Completion Code");
                if (cc == completionCode)
                {
                    completionRule.Add("Update Job location", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Update Job location"));
                    completionRule.Add("Breakdown Code 1", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Breakdown Code 1"));
                    completionRule.Add("Breakdown Code 2", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Breakdown Code 2"));
                    completionRule.Add("Receipt #", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Receipt #"));
                    completionRule.Add("Battery Test Barcode", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Battery Test Barcode"));
                    completionRule.Add("Part # 1", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Part # 1"));
                    completionRule.Add("Part # 2", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Part # 2"));
                    completionRule.Add("Tow Km's", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Tow Km's"));
                    completionRule.Add("Job Comment", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Job Comment"));
                    completionRule.Add("Canned Comment", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Canned Comment"));
                    completionRule.Add("Pay All", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Pay All"));
                    completionRule.Add("Tow Type", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Tow Type"));
                    completionRule.Add("# of Passengers", Global.CommonMethods.ExcelLib.ReadExcelData(i, "# of Passengers"));
                    completionRule.Add("*Repairer Address", Global.CommonMethods.ExcelLib.ReadExcelData(i, "*Repairer Address"));
                    completionRule.Add("Payment Taken", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Payment Taken"));
                    completionRule.Add("Vehicle Make", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Vehicle Make"));
                    completionRule.Add("Vehicle Model", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Vehicle Model"));
                    completionRule.Add("Year of Manufacture", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Year of Manufacture"));
                    completionRule.Add("Vehicle Variant", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Vehicle Variant"));
                    completionRule.Add("Vehicle Colour", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Vehicle Colour"));
                    completionRule.Add("Engine Size", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Engine Size"));
                    completionRule.Add("Fuel Type", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Fuel Type"));
                    completionRule.Add("Transmission", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Transmission"));
                    completionRule.Add("Vehicle GVM", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Vehicle GVM"));
                    break;
                }
            }
        }

        public void ReadBreakdownCodeRules()
        {
            int noofrows = 0, noofcolumns = 0;
            Global.CommonMethods.ExcelLib.CollectionFromExcel(ExcelPath, "BreakdownCode", out noofrows, out noofcolumns);

            for (int i = 2; i <= noofrows; i++)
            {
                string bc = Global.CommonMethods.ExcelLib.ReadExcelData(i, "Breakdown Code");
                if (bc == breakdownCode)
                {
                    breakdowncodeRule.Add("Description", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Description"));
                    breakdowncodeRule.Add("Receipt #", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Receipt #"));
                    breakdowncodeRule.Add("Part # 1", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Part # 1"));
                    breakdowncodeRule.Add("Payment Taken", Global.CommonMethods.ExcelLib.ReadExcelData(i, "Payment Taken"));
                    break;
                }
            }
        }

        public void Check_EditMandFldOnScreen(string fieldname, string testdata, string mode)
        {
            string mainXpath = null, gridXpath = null, beforeXpath = null, afterXpath = null;
            bool skipByOne = false, fromGrid = false;

            if (fieldname == "Breakdown Code 1")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }

            if (fieldname == "Receipt #")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }

            if (fieldname == "Update Job location")
            {
                mainXpath = "//*[@id=\"updatedJobLocation\"]";
                testdata = breakdownCode;
                fromGrid = false;
            }
            if (fieldname == "Breakdown Code 2")
            {
                mainXpath = "//*[@id=\"breakdowncategory2\"]"; 
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }

            if (fieldname == "Battery Test Barcode")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Part # 1")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Part # 2")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Tow Km's")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Job Comment")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Canned Comment")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Pay All")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Tow Type")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "# of Passengers")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "*Repairer Address")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Payment Taken")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Vehicle Make")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Vehicle Model")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Year of Manufacture")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Vehicle Variant")
            {
                mainXpath = "//*[@id=\"ReceiptNumber\"]";
                testdata = "12345";
                fromGrid = false;
            }
            if (fieldname == "Vehicle Colour")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Engine Size")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Fuel Type")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Transmission")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }
            if (fieldname == "Vehicle GVM")
            {
                mainXpath = "//*[@id=\"breakdowncode1\"]";
                gridXpath = "//*[@id=\"grid_breakdowncode1\"]";
                beforeXpath = "//*[@id=\"gridItem_breakdowncode1_";
                afterXpath = "\"]";
                skipByOne = false;
                testdata = breakdownCode;
                fromGrid = true;
            }


            if (mode == "Read")
            {
                CheckDataInMandFld(mainXpath, fieldname);
            }

                Driver.driver.FindElement(By.XPath(mainXpath)).Click();

                Thread.Sleep(2000);
                if (fromGrid)
                    SelectFromGrid(gridXpath, testdata, beforeXpath, afterXpath, skipByOne);
                else
                    Driver.driver.FindElement(By.XPath(mainXpath)).SendKeys(testdata);

        }

        private void CheckDataInMandFld(string fieldname, string bdcd1Xpath)
        {
            string txt = Driver.driver.FindElement(By.XPath(bdcd1Xpath)).Text;

            if (string.IsNullOrEmpty(txt))
                Global.Base.LogPageValidation("Alert is not raised for mandatory field " + fieldname, "Fail");
            else
            { 
                Global.Base.LogPageValidation("Data exists in " + fieldname + "as" + txt, "Info");
                //Clear the text to enter test data
                Driver.driver.FindElement(By.XPath(bdcd1Xpath)).Clear();
            }

        }
    }
}

using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPWorkApp.Global
{
    class Driver
    {
        //Initialize the driver
        public static IWebDriver driver { get; set; }

        //Generic Methods
        //Identifying the Textbox
        public static void SendText(IWebDriver driver, string Locator, string LocatorValue, string InputValue)
        {
            By by = GetFindByString(Locator, LocatorValue);
            driver.FindElement(by).Clear();
            Thread.Sleep(500);
            driver.FindElement(by).SendKeys(InputValue);
        }

        //Identifying the ActionButtons or Dropdowns
        public static void ClickButton(IWebDriver driver, string Locator, string LocatorValue)
        {
            By by = GetFindByString(Locator, LocatorValue);
            Thread.Sleep(500);
            driver.FindElement(by).Click();
        }

        //Get the text from the IWebElements
        public static string GetTextValue(IWebDriver driver, string Locator, string LocatorValue)
        {
            By by = GetFindByString(Locator, LocatorValue);
            return driver.FindElement(by).Text;
        }


        //To clear the text
        public static void GetClear(IWebDriver driver, string Locator, string LocatorValue)
        {
            By by = GetFindByString(Locator, LocatorValue);
            driver.FindElement(by).Clear();
        }


        public static bool ElementVisible(IWebDriver driver, string Locator, string LocatorValue)
        {
            try
            {
                By by = GetFindByString(Locator, LocatorValue);
                return driver.FindElement(by).Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;

            }
        }

        //Function to get By.<Locator>("<LValue>") expression
        internal static By GetFindByString(string Locator, string Lvalue)
        {
            By by = null;

            switch (Locator)
            {
                case "Id":
                    by = By.Id(Lvalue);
                    break;
                case "XPath":
                    by = By.XPath(Lvalue);
                    break;
                case "CSSSelector":
                    by = By.CssSelector(Lvalue);
                    break;
                case "ClassName":
                    by = By.ClassName(Lvalue);
                    break;
                case "TagName":
                    by = By.TagName(Lvalue);
                    break;
                case "Name":
                    by = By.Name(Lvalue);
                    break;
                case "LinkText":
                    by = By.LinkText(Lvalue);
                    break;
                case "PartialLinkText":
                    by = By.PartialLinkText(Lvalue);
                    break;
            }

            return by;
        }

        /* ToClickAsYes - When there is a need to find an element using for loop which also needs to be clicked, then ToClickAsYes will be "Yes" else it will be "No".
         * Need for AfterXPath variable - In case there is a need to identify and click an element in for loop where XPath can be split on the basis of i variable, 
         * then complete XPath will be 
         * Lvalue will have the Before XPath + i variable + AfterXPath */
        public static string FindElementsUsingForLoop(IWebDriver driver, string Locator, string Lvalue)
        {
            try
            {

                //Trigger the function to get By.<Locator>("<LValue>") expression
                By by = GetFindByString(Locator, Lvalue);

                ReadOnlyCollection<IWebElement> elementlist = driver.FindElements(by);

                int i = 1;
                string result = null;

                foreach (var element in elementlist)
                {
                    result = driver.FindElement(By.XPath(Lvalue + "/div[" + i.ToString() + "]")).Text;
                    //    //if After XPath exists then 
                    //    if (Locator == "XPath" && AfterXpath != "" && Inputvalue == element.Text && ToClickAsYes == "Yes")
                    //    {
                    //        driver.FindElement(By.XPath(Lvalue + "[" + (i - 1) + "]" + AfterXpath)).Click();
                    //        return true;
                    //    }

                    //    if (AfterXpath == "" && Inputvalue == element.Text && ToClickAsYes == "Yes")
                    //    {
                    //        element.Click();
                    //        return true;
                    //    }
                    //    if (AfterXpath == "" && Inputvalue == element.Text && ToClickAsYes == "No")
                    //    {
                    //        return true;
                    //    }
                    //    i++;
                }
                return result;

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static void SendDateWithOutCalendar(IWebDriver driver, string Locator, string Lvalue, string Inputvalue)
        {
            By by = GetFindByString(Locator, Lvalue);

            driver.FindElement(by).Clear();
            Thread.Sleep(500);
            driver.FindElement(by).SendKeys(Inputvalue);
            driver.FindElement(by).SendKeys(Keys.Tab);
        }

        public static void IntroduceWait(IWebDriver driver, int timeinms)
        {
            Thread.Sleep(timeinms);
        }
    }
}
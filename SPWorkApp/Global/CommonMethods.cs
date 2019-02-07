using ExcelDataReader;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPWorkApp.Global
{
    class CommonMethods
    {
        #region Excel
        public class ExcelLib
        {
            static List<Datacollection> dataCol = new List<Datacollection>();
            static int noofrows, noofcolumns;

            public class Datacollection
            {
                public int rowNumber { get; set; }
                public string colName { get; set; }
                public string colValue { get; set; }
            }


            public static void ClearData()
            {
                dataCol.Clear();
            }


            private static DataTable ExcelToDataTable(string fileName, string SheetName)
            {
                // Open file and return as Stream
                using (System.IO.FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        //To use first row as header
                        DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        //Get all the tables
                        DataTableCollection table = result.Tables;

                        // store it in data table
                        DataTable resultTable = table[SheetName];

                        return resultTable;
                    }
                }
            }

            public static string ReadExcelData(int rowNumber, string columnName)
            {
                try
                {
                    //Retriving Data using LINQ to reduce much of iterations

                    rowNumber = rowNumber - 1;
                    string data = (from colData in dataCol
                                   where colData.colName == columnName && colData.rowNumber == rowNumber
                                   select colData.colValue).SingleOrDefault();

                    //var datas = dataCol.Where(x => x.colName == columnName && x.rowNumber == rowNumber).SingleOrDefault().colValue;


                    return data.ToString();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Exception occurred in ExcelLib Class ReadData Method!" + Environment.NewLine + e.Message.ToString());
                    return null;
                }
            }

            public static void CollectionFromExcel(string fileName, string SheetName)
            {
                ExcelLib.ClearData();
                DataTable table = ExcelToDataTable(fileName, SheetName);

                //Added on 31 Jan
                noofrows = table.Rows.Count;
                noofcolumns = table.Columns.Count;

                //Iterate through the rows and columns of the Table
                for (int row = 1; row <= table.Rows.Count; row++)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        Datacollection dtTable = new Datacollection()
                        {
                            rowNumber = row,
                            colName = table.Columns[col].ColumnName,
                            colValue = table.Rows[row - 1][col].ToString()
                        };


                        //Add all the details for each row
                        dataCol.Add(dtTable);

                    }
                }
            }

            public static void CollectionFromExcel(string fileName, string SheetName, out int noofrows, out int noofcolumns)
            {
                ExcelLib.ClearData();
                DataTable table = ExcelToDataTable(fileName, SheetName);

                //Added on 31 Jan
                noofrows = table.Rows.Count;
                noofcolumns = table.Columns.Count;

                //Iterate through the rows and columns of the Table
                for (int row = 1; row <= table.Rows.Count; row++)
                {
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        Datacollection dtTable = new Datacollection()
                        {
                            rowNumber = row,
                            colName = table.Columns[col].ColumnName,
                            colValue = table.Rows[row - 1][col].ToString()
                        };


                        //Add all the details for each row
                        dataCol.Add(dtTable);

                    }
                }
            }


        }




        #endregion

        #region screenshots
        public class SaveScreenShotClass
        {
            public static string SaveScreenshot(IWebDriver driver, string ScreenShotFileName) // Definition
            {
                var folderLocation = (Base.ScreenshotPath);

                if (!System.IO.Directory.Exists(folderLocation))
                {
                    System.IO.Directory.CreateDirectory(folderLocation);
                }

                var screenShot = ((ITakesScreenshot)driver).GetScreenshot();
                var fileName = new StringBuilder(folderLocation);

                fileName.Append(ScreenShotFileName);
                fileName.Append(DateTime.Now.ToString("_dd-mm-yyyy_mss"));

                fileName.Append(".jpeg");
                screenShot.SaveAsFile(fileName.ToString(), ScreenshotImageFormat.Jpeg);
                return fileName.ToString();
            }
        }
        #endregion
    }
}

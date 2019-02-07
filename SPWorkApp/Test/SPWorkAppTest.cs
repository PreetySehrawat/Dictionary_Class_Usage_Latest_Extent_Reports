using NUnit.Framework;
using SPWorkApp.Global;
using SPWorkApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPWorkApp
{
    public class SPWorkAppTest
    {
        [TestFixture]
        class SPWorkForm : SPWorkApp.Global.Base
        {
            [Test]
            public void CheckMandatoryValidation()
            {
                test = extent.CreateTest("Mandatory Field Validations");

                JobCompletionForm jcf = new JobCompletionForm();

                //Login into the application and click on "Complete" button for an ongoing job
                jcf.Login();

                //Read the test data
                jcf.ReadTestData();

                //Enter Completion Code and Breakdown category
                jcf.EnterCompCodeandBrdwnCtgry();

                //Read the Completion Rule from excel for completion code given in test data
                jcf.ReadCompletionRule();

                //Read the Breakdown Rule from excel for breakdown code given in test data
                jcf.ReadBreakdownCodeRules();

                //Check for Mandatory field in the rule then
                //click on Send Button and alert will be raised then
                //Verify the Alert is for Mandatory field
                //Repeat for all mandatory fields in the rule
                jcf.Check_MandFldInRule_VrfyAlert_EditFld();
            }
        }
    }
}

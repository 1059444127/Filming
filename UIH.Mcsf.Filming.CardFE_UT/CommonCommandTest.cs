using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for CommonCommandTest and is intended
    ///to contain all CommonCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommonCommandTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SaveEFilmsCommand
        ///</summary>
        //[TestMethod()]
        //public void SaveEFilmsCommandTest()
        //{
        //    string sImagePathList = string.Empty; // 
        //    CommonCommand.SaveEFilmsCommand(sImagePathList);
        //    //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for DoCreateNewViewerController
        ///</summary>
        //[TestMethod()]
        //public void DoCreateNewViewerControllerTest()
        //{
        //    string index = "0"; // 
        //    CommonCommand.DoCreateNewViewerController(index);
        //    //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for CommonCommand Constructor
        ///</summary>
        [TestMethod()]
        public void CommonCommandConstructorTest()
        {
            CommonCommand target = new CommonCommand();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}

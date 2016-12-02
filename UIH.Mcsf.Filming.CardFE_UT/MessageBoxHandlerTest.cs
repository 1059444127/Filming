using UIH.Mcsf.Filming.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.MHC;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for MessageBoxHandlerTest and is intended
    ///to contain all MessageBoxHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessageBoxHandlerTest
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
        ///A test for ShowError
        ///</summary>
        [TestMethod()]
        public void ShowErrorTest()
        {
            //string key = string.Empty; // 
            ////MessageBoxType messageBoxType = new MessageBoxType(); // 
            ////string titleKey = string.Empty; // 
            ////MessageBoxResponse expected = new MessageBoxResponse(); // 
            //MessageBoxHandler.Instance.ShowError(key);
            ////Assert.AreEqual(expected, actual);
            ////Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowInfo
        ///</summary>
        [TestMethod()]
        public void ShowInfoTest()
        {
            //string key = string.Empty; // 
            ////MessageBoxType messageBoxType = new MessageBoxType(); // 
            ////string titleKey = string.Empty; // 
            ////MessageBoxResponse expected = new MessageBoxResponse(); // 
            //MessageBoxHandler.Instance.ShowInfo(key);
            ////Assert.AreEqual(expected, actual);
            ////Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowQuestion
        ///</summary>
        [TestMethod()]
        public void ShowQuestionTest()
        {
            //string key = string.Empty; // 
            ////MessageBoxType messageBoxType = new MessageBoxType(); // 
            ////string titleKey = string.Empty; // 
            ////MessageBoxResponse expected = new MessageBoxResponse(); // 
            //MessageBoxHandler.Instance.ShowQuestion(key, null);
            ////Assert.AreEqual(expected, actual);
            ////Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ShowWarning
        ///</summary>
        [TestMethod()]
        public void ShowWarningTest()
        {
            //string key = string.Empty; // 
            ////MessageBoxType messageBoxType = new MessageBoxType(); // 
            ////string titleKey = string.Empty; // 
            ////MessageBoxResponse expected = new MessageBoxResponse(); // 
            //MessageBoxHandler.Instance.ShowWarning(key);
            ////Assert.AreEqual(expected, actual);
            ////Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

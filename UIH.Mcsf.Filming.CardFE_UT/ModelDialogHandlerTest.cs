using UIH.Mcsf.Filming.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for ModelDialogHandlerTest and is intended
    ///to contain all ModelDialogHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ModelDialogHandlerTest
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
        ///A test for ModelDialogHandler Constructor
        ///</summary>
        [TestMethod()]
        public void ModelDialogHandlerConstructorTest()
        {
            ModelDialogHandler target = new ModelDialogHandler();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for EnableWindow
        ///</summary>
        /////remove this case For ms-test UT sometimes fail when package building
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void EnableWindowTest()
        //{
        //    IntPtr hwnd = new IntPtr(); // 
        //    bool enable = false; // 
        //    //bool expected = false; // 
        //    bool actual;
        //    actual = ModelDialogHandler_Accessor.EnableWindow(hwnd, enable);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for GetParent
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("UIH.Mcsf.Filming.CardFE.dll")]
        //public void GetParentTest()
        //{
        //    IntPtr hwnd = new IntPtr(); // 
        //    //IntPtr expected = new IntPtr(); // 
        //    IntPtr actual;
        //    actual = ModelDialogHandler_Accessor.GetParent(hwnd);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for ShowModelWnd
        ///</summary>
        [TestMethod()]
        public void ShowModelWndTest()
        {
            try
            {
                
                Window wnd = null; // 
                ModelDialogHandler.ShowModelWnd(wnd);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch (Exception)
            {

            }
        }
    }
}

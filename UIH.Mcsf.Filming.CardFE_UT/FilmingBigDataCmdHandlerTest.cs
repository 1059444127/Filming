using UIH.Mcsf.Filming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UIH.Mcsf.Filming.Command;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingBigDataCmdHandlerTest and is intended
    ///to contain all FilmingBigDataCmdHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingBigDataCmdHandlerTest
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
        ///A test for HandleDataTrans
        ///</summary>
        //[TestMethod()]
        //public void HandleDataTransTest()
        //{
        //    FilmingBigDataCmdHandler target = new FilmingBigDataCmdHandler(); // 
        //    byte[] buffer = null; // 
        //    int len = 0; // 
        //    //int expected = 0; // 
        //    int actual;
        //    actual = target.HandleDataTrans(buffer, len);
        //    //Assert.AreEqual(expected, actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for FilmingBigDataCmdHandler Constructor
        ///</summary>
        [TestMethod()]
        public void FilmingBigDataCmdHandlerConstructorTest()
        {
            //FilmingBigDataCmdHandler target = new FilmingBigDataCmdHandler();
            ////Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}

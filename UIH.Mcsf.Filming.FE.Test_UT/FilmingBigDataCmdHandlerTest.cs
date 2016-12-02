using UIH.Mcsf.Filming.Card.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingBigDataCmdHandlerTest and is intended
    ///to contain all FilmingBigDataCmdHandlerTest Unit Tests
    ///</summary>
    [TestClass]
    public class FilmingBigDataCmdHandlerTest
    {
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


        #region [--unused--]

///// <summary>
/////A test for DataHandler Constructor
/////</summary>
//[TestMethod]
//public void FilmingBigDataCmdHandlerConstructorTest()
//{
//    var target = new DataHandler();
//}

        /// <summary>
        ///A test for HandleDataTrans
        ///</summary>
        [TestMethod]
        public void HandleDataTransTest()
        {
            var target = new DataHandler();
            int len = 0; 
            int expected = -1; 
            int actual;
            actual = target.HandleDataTrans(null, len);
            Assert.AreEqual(expected, actual);
        }

        #endregion [--unused--]

    }
}

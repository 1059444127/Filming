using System;
using UIH.Mcsf.Filming.Card.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIH.Mcsf.Filming.FE.Test_UT
{
    
    
    /// <summary>
    ///This is a test class for FilmingDbWrapperTest and is intended
    ///to contain all FilmingDbWrapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FilmingDbWrapperTest
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

        /// <summary>
        ///A test for Lock
        ///</summary>
        [TestMethod]
        public void LockTest()
        {
            try
            {
                DbWrapper.Instance.Lock("0");
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        ///A test for UnLock
        ///</summary>
        [TestMethod]
        public void UnLockTest()
        {
            try
            {
                DbWrapper.Instance.UnLock();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }

///// <summary>
/////A test for Instance
/////</summary>
//[TestMethod()]
//public void InstanceTest()
//{
//    DbWrapper actual;
//    actual = DbWrapper.Instance;
//}

        #endregion [--unused--]

    }
}

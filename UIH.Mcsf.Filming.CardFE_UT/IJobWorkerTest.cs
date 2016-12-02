using UIH.Mcsf.Filming.ImageManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    using UIH.Mcsf.Filming.ImageManager;

    /// <summary>
    ///This is a test class for IJobWorkerTest and is intended
    ///to contain all IJobWorkerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IJobWorkerTest
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


        internal virtual IWorkFlow CreateIJobWorker()
        {
            IWorkFlow target = new DataHeaderJobWorker();
            return target;
        }

        /// <summary>
        ///A test for Preprocess
        ///</summary>
        [TestMethod()]
        public void PreprocessTest()
        {
            try
            {
                IWorkFlow target = CreateIJobWorker(); 
                object originalJob = new object(); 
                object expected = null;
                object actual;
                actual = target.Preprocess(originalJob);
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }
    }
}
